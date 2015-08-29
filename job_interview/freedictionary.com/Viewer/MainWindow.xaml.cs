using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region LoadType Enumeration

        private enum LoadType
        {
            Current,
            Previous,
            Next
        }

        #endregion

        #region HTML Templates

        private const String HtmlStart =
            @"<html>
					<head>
						<link rel='stylesheet' type='text/css' href='http://img.tfd.com/t.css'>
						<style type='text/css'>
							TD {font-size:10pt}
						</style>
						<script type='text/javascript'>function extLink(url){location.assign(url);}</script>
					</head>
					<body>
						<table>";

        private const String HtmlTemplate = "<tr><td>{0}</td></tr>";

        private const String HtmlEnd =
                        @"</table>
					</body>
				</html>";

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Load Method

        private void LoadEntry(LoadType loadType = LoadType.Current)
        {
            var headwordToSearch = TbHeadword.Text;

            using (var entities = new DictionaryEntities())
            {
                var entries = entities.Entries
                                    .Where(i =>
                                        (loadType == LoadType.Current && String.Compare(i.Headword, headwordToSearch, StringComparison.Ordinal) >= 0) ||
                                        (loadType == LoadType.Previous && String.Compare(i.Headword, headwordToSearch, StringComparison.Ordinal) < 0) ||
                                        (loadType == LoadType.Next && String.Compare(i.Headword, headwordToSearch, StringComparison.Ordinal) > 0))
                                    .OrderBy(i => i.Headword);

                var entry = loadType == LoadType.Previous ?
                    entries.OrderByDescending(i => i.Headword).FirstOrDefault() : entries.FirstOrDefault();

                if (entry == null)
                    return;

                var similarEntries = entities
                    .Entries
                    .Where(i => String.Compare(i.Headword, entry.Headword, StringComparison.Ordinal) == 0)
                    .ToList();

                TbHeadword.Text = entry.Headword;

                var html = new StringBuilder(HtmlStart);
                foreach (var similarEntry in similarEntries)
                {
                    html.AppendFormat(HtmlTemplate, similarEntry.Article);
                }

                html.Append(HtmlEnd);

                WbMain.NavigateToString(html.ToString());

                LbAliases.Items.Clear();

                foreach (var similarEntry in similarEntries)
                {
                    foreach (var alias in similarEntry.EntryAliases)
                    {
                        LbAliases.Items.Add(alias.Alias);
                    }

                    LbAliases.Items.Add(String.Empty);
                }
            }
        }

        #endregion

        #region Event Handlers

        private void WebBrowserNavigatingEventHandler(Object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri == null || e.Uri.AbsolutePath == "blank")
                return;

            e.Cancel = true;
            TbHeadword.Text = Uri.UnescapeDataString(e.Uri.AbsolutePath.Replace("%2B", " "));
            LoadEntry();
        }

        private void TbHeadwordKeyDownEventHandler(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoadEntry();
        }

        private void LbAliasesSelectionChangedEventHandler(Object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0 || String.IsNullOrWhiteSpace(e.AddedItems[0].ToString()))
                return;

            TbHeadword.Text = e.AddedItems[0].ToString();
            LoadEntry();
        }

        private void BtnOpenClickEventHandler(Object sender, RoutedEventArgs e)
        {
            LoadEntry();
        }

        private void BtnPreviousClickEventHandler(Object sender, RoutedEventArgs e)
        {
            LoadEntry(LoadType.Previous);
        }

        private void BtnNextClickEventHandler(Object sender, RoutedEventArgs e)
        {
            LoadEntry(LoadType.Next);
        }

        #endregion
    }
}

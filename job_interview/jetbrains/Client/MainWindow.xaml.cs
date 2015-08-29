using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using RestSharp;
using WinForms = System.Windows.Forms;

namespace TextIndexing.Client
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void ChooseFileButtonClickEvenHandler(Object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog { InitialDirectory = Path.GetFullPath(@"../../../Books") };
			if (!dialog.ShowDialog(this).Value)
				return;

			IndexObjectTextBox.Text = dialog.FileName;
			IndexObjectTextBox.Focus();
		}

		private void ChooseDirectoryButtonClickEventHandler(Object sender, RoutedEventArgs e)
		{
			var dialog = new WinForms.FolderBrowserDialog();
			if (dialog.ShowDialog() != WinForms.DialogResult.OK)
				return;

			IndexObjectTextBox.Text = dialog.SelectedPath;
			IndexObjectTextBox.Focus();
		}

		private void SendIndexObjectButtonClickEventHandler(Object sender, RoutedEventArgs e)
		{
			try
			{
				var client = new RestClient(HostUrlTextBox.Text);
				var path = IndexObjectTextBox.Text;

				String resource;
				if (File.Exists(path))
				{
					resource = "watch_file";
				}
				else if (Directory.Exists(path))
				{
					resource = "watch_directory";
				}
				else
				{
					LogView.Items.Insert(0, "Invalid object name to index");
					return;
				}

				var request = new RestRequest(resource, Method.PUT);
				request.AddParameter("text/plain", Path.GetFullPath(path), ParameterType.RequestBody);

				var stopwatch = Stopwatch.StartNew();
				client.ExecuteAsync(request, response =>
				{
					stopwatch.Stop();
					Dispatcher.InvokeAsync(() =>
					{
						Log(request, response, stopwatch.ElapsedMilliseconds);
					});
				});
			}
			catch (Exception exception)
			{
				LogView.Items.Insert(0, String.Format("Failed to send request: {0}", exception));
			}
		}

		private void SendQueryButtonClickEventHandler(Object sender, RoutedEventArgs e)
		{
			try
			{
				var client = new RestClient(HostUrlTextBox.Text);
				var request = new RestRequest("get_files", Method.POST);
				request.AddParameter("text/plain", QueryTextBox.Text, ParameterType.RequestBody);

				var stopwatch = Stopwatch.StartNew();
				client.ExecuteAsync(request, response =>
				{
					stopwatch.Stop();
					Dispatcher.InvokeAsync(() =>
					{
						Log(request, response, stopwatch.ElapsedMilliseconds);
					});
				});
			}
			catch (Exception exception)
			{
				LogView.Items.Insert(0, String.Format("Failed to send request: {0}", exception));
			}
		}

		private void IndexObjectTextBoxKeyUpEventHandler(Object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SendIndexObjectButton.Focus();
				SendIndexObjectButtonClickEventHandler(SendIndexObjectButton, null);
			}
		}

		private void QueryTextBoxKeyUpEventHandler(Object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SendQueryButton.Focus();
				SendQueryButtonClickEventHandler(SendQueryButton, null);
			}
		}

		private void Log(IRestRequest request, IRestResponse response, Int64 time)
		{
			LogView.Items.Insert(0, String.Format("{0}ms, Response: {1}, Request: {2} ({3})", 
				time,
				String.IsNullOrWhiteSpace(response.ErrorMessage) ? response.Content : response.ErrorMessage,
				request.Resource,
				request.Parameters[0].Value));
		}
	}
}

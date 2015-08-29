using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;

namespace DictionaryParser.Tools
{
	internal class ParserManager
	{
		#region Constants

		private const String XmlSearchPattern = "*.xml";
		private const String DocumentsFolder = "XmlData";
		private const String ProcessedDocumentsFile = "Processed.txt";

		private const String ConnectionStringName = "Dictionary";

		private const String ClearTablesQuery = @"
			TRUNCATE TABLE [WoltersKluwer_Stedman_Medical]
			TRUNCATE TABLE [WoltersKluwer_Stedman_Medical_Aliases]";

		private const String InsertEntryQuery = @"
			INSERT INTO [WoltersKluwer_Stedman_Medical]([Word], [Txt], [WordOriginal])
			VALUES (@Headword, @Article, @OriginalHeadword);

			SELECT SCOPE_IDENTITY();";

		private const String InsertAliasQuery = @"
			INSERT INTO [WoltersKluwer_Stedman_Medical_Aliases]([WordID], [Word], [RelType])
			VALUES ({0}, @Alias, {1})";

		#endregion

		#region Fields

		private readonly Subject<String> _parserManagerInformation;

		#endregion

		#region Constructors

		public ParserManager()
		{
			_parserManagerInformation = new Subject<String>();
		}

		#endregion

		#region Internal Properties

		public IObservable<String> ParserManagerInformation
		{
			get { return _parserManagerInformation; }
		}

		#endregion

		#region Public Methods

		public void Start()
		{
			var currentDirectory = Directory.GetCurrentDirectory();
			var xmlFilenames = new List<String>(Directory.GetFiles(Path.Combine(currentDirectory, DocumentsFolder), XmlSearchPattern));
			var processedDocumentFile = Path.Combine(currentDirectory, ProcessedDocumentsFile);

			var processedDocumentsFileExists = File.Exists(processedDocumentFile);
			if (processedDocumentsFileExists)
			{
				foreach (var fullPath in File.ReadAllLines(ProcessedDocumentsFile)
											 .Where(i =>
												 !String.IsNullOrWhiteSpace(i) &&
												 Uri.IsWellFormedUriString(i, UriKind.RelativeOrAbsolute))
											 .Select(Path.GetFullPath)
											 .Where(xmlFilenames.Contains))
				{
					xmlFilenames.Remove(fullPath);
				}
			}

			using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString))
			{
				if (connection.State != ConnectionState.Open)
				{
					connection.Open();
				}

				var command = connection.CreateCommand();
				command.CommandText = ClearTablesQuery;
				command.ExecuteNonQuery();
			}

			using (var processedDocumentsStream = !processedDocumentsFileExists
													? File.Create(processedDocumentFile)
													: File.OpenWrite(processedDocumentFile))
			{
				foreach (var filename in xmlFilenames)
				{
					_parserManagerInformation.OnNext(String.Format("Processing '{0}'", filename));

					var totalEntries = 0;
					var startTime = DateTime.Now;

					using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString))
					{
						if (connection.State != ConnectionState.Open)
						{
							connection.Open();
						}

						var entryCommand = connection.CreateCommand();
						var aliasCommand = connection.CreateCommand();

						var headwordParameter = new SqlParameter("Headword", SqlDbType.VarChar, 70);
						var articleParameter = new SqlParameter("Article", SqlDbType.Text);
						var originalHeadwordParameter = new SqlParameter("OriginalHeadword", SqlDbType.NVarChar, 70);
						var aliasParameter = new SqlParameter("Alias", SqlDbType.VarChar, 70);

						entryCommand.Parameters.AddRange(new[] { headwordParameter, articleParameter, originalHeadwordParameter });
						entryCommand.CommandText = InsertEntryQuery;

						aliasCommand.Parameters.Add(aliasParameter);

						var parser = new Parser();

						using (parser.ParseInformation.Subscribe(_parserManagerInformation.OnNext))
						{
							foreach (var entry in parser.Parse(filename))
							{
								headwordParameter.Value = entry.Key.Headword;
								articleParameter.Value = entry.Key.Article;
								originalHeadwordParameter.Value = String.IsNullOrWhiteSpace(entry.Key.OriginalHeadword) ? DBNull.Value : (Object)entry.Key.OriginalHeadword;

								var entryId = Convert.ToInt32(entryCommand.ExecuteScalar());
								foreach (var entryAlias in entry.Value.Where(alias => alias.Alias != headwordParameter.Value.ToString()))
								{
									aliasCommand.CommandText = String.Format(InsertAliasQuery, entryId, (Int32)entryAlias.RelationType);
									aliasParameter.Value = entryAlias.Alias;

									aliasCommand.ExecuteNonQuery();
								}

								totalEntries++;

								if (totalEntries % 100 == 0)
								{
									_parserManagerInformation.OnNext(String.Format("Entries processed: {0} in {1}",
																				   totalEntries, DateTime.Now - startTime));
								}
							}
						}

						using (var writer = new StreamWriter(processedDocumentsStream))
						{
							writer.WriteLine(filename);
						}
					}

					_parserManagerInformation.OnNext(String.Format("Finished processing '{0}'. Total entries processed: {1} in {2}",
						filename, totalEntries, DateTime.Now - startTime));
				}
			}

			_parserManagerInformation.OnCompleted();
		}

		#endregion
	}
}

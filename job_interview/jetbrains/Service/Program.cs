using System;
using System.IO;
using System.Reflection;
using System.Text;
using CommandLine;
using CommandLine.Text;
using Grapevine.Server;
using TextIndexing.Library;

namespace TextIndexing.Service
{
	public sealed class Program
	{
		public static void Main(String[] args)
		{
			Console.OutputEncoding = Encoding.Unicode;

			// At first idea was to pass folder like "./Indexers" and "./Parsers"
			// Load all assemblies from that folders and find implementation of interfaces
			// After that we could load another implementations in runtime
			// And choose required implementation per request
			// But this produced several problems that are not required to solve
			// But would take more to implement
			IQueryParser parser = null;
			IFileIndexer indexer = null;
			var port = 1234;

			var options = new Options();
			if (Parser.Default.ParseArguments(args, options))
			{
				// -l "../../../AdvancedQueryParser/bin/{Debug/Release}/AdvancedQueryParser.dll" -p "TextIndexing.AdvancedQueryParser.IntersectionParser"
				parser = LoadImplementation<IQueryParser>(options.ParserLibrary, options.ParserType);
				indexer = LoadImplementation<IFileIndexer>(options.IndexerLibrary, options.IndexerType);
				port = options.Port;
			}

			if (parser == null)
				parser = new SimpleParser();

			if (indexer == null)
				indexer = new SimpleIndexer();

			Console.WriteLine("IFileIndexer implementation loaded: {0}", indexer.GetType());
			Console.WriteLine("IQueryParser implementation loaded: {0}", parser.GetType());

			Index.Instance.Init(parser, indexer);

			using (var server = new RESTServer(port: port.ToString()))
			{
				server.Start();

				if (server.IsListening)
					Console.WriteLine("Service started on port: {0}", server.Port);
				else
					Console.WriteLine("Failed to start service");

				Console.ReadKey(true);
			}
		}

		private static T LoadImplementation<T>(String library, String typeName)
			where T : class
		{
			try
			{
				var assembly = Assembly.LoadFile(Path.GetFullPath(library));
				var type = assembly.GetType(typeName, false);
				if (type != null)
					return (T)Activator.CreateInstance(type);
			}
			catch
			{
				return null;
			}

			return null;
		}

		private sealed class Options
		{
			// ReSharper disable UnusedAutoPropertyAccessor.Local
			[Option('t', "port", DefaultValue = (UInt16)1234, HelpText = "A port to listen for incoming requests")]
			public UInt16 Port { get; set; }

			[Option('l', "parser-library", HelpText = "A full path to DLL containing IQueryParser implementation")]
			public String ParserLibrary { get; set; }

			[Option('p', "parser-type", HelpText = "An IQueryParser implementation type name")]
			public String ParserType { get; set; }

			[Option('r', "indexer-library", HelpText = "A full path to DLL containing IFileIndexer implementation")]
			public String IndexerLibrary { get; set; }

			[Option('i', "indexer-type", HelpText = "An IFileIndexer implementation type name")]
			public String IndexerType { get; set; }
			// ReSharper restore UnusedAutoPropertyAccessor.Local

			// ReSharper disable once UnusedMember.Local
			[HelpOption]
			public string GetUsage()
			{
				return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
			}
		}
	}
}

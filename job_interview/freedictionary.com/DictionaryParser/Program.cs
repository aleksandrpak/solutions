using System;
using DictionaryParser.Tools;

namespace DictionaryParser
{
	class Program
	{
		private static void Main()
		{
			try
			{
				var manager = new ParserManager();

				using (manager.ParserManagerInformation.Subscribe(Console.WriteLine))
				{
					manager.Start();
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
			}

			Console.WriteLine("Finished. Press any key to exit...");
			Console.ReadKey(true);
		}
	}
}

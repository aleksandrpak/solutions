using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TextIndexing.Library
{
	/// <summary>
	/// Represents simple indexer that looks for words containg just letters (not digits) having length more than 2 characters.
	/// </summary>
	public sealed class SimpleIndexer : IFileIndexer
	{
		private enum ReadState
		{
			SeekingForWord,
			ReadingWord
		}

		public IEnumerable<String> GetWords(FileStream fileStream)
		{
			using (var reader = new StreamReader(fileStream, true))
			{
				var builder = new StringBuilder();
				var state = ReadState.SeekingForWord;
				var value = reader.Read();

				while (value != -1)
				{
					var character = Convert.ToChar(value);
					switch (state)
					{
						case ReadState.SeekingForWord:
							if (Char.IsLetter(character))
							{
								builder.Append(character);
								state = ReadState.ReadingWord;
							}
							break;

						case ReadState.ReadingWord:
							if (Char.IsLetter(character))
							{
								builder.Append(character);
							}
							else
							{
								if (builder.Length > 2)
									yield return builder.ToString();

								builder.Clear();
								state = ReadState.SeekingForWord;
							}
							break;
					}

					value = reader.Read();
				}
			}
		}
	}
}

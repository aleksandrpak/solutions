using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextIndexing.Library;

namespace TextIndexing.AdvancedQueryParser
{
	/// <summary>
	/// Represents simple parser that can intersect result of several words in query.
	/// </summary>
	/// <example>
	/// "Dieu", "Князь"
	/// </example>
	public sealed class IntersectionParser : IQueryParser
	{
		private enum ParserState
		{
			LookingForDelimiter,
			ReadingWord,
		}

		private const String WordDelimiter = "\"";

		public IEnumerable<String> GetFiles(String query, IIndex index)
		{
			// If query does not have delimiter than all query is one word to search
			if (!query.Contains(WordDelimiter))
				return index.FindEntry(query);

			var result = new List<IndexEntry>();
			var state = ParserState.LookingForDelimiter;
			var builder = new StringBuilder();

			foreach (var character in query)
			{
				switch (state)
				{
					case ParserState.LookingForDelimiter:
						if (character == WordDelimiter[0])
							state = ParserState.ReadingWord;

						break;

					case ParserState.ReadingWord:
						if (character == WordDelimiter[0])
						{
							var entry = index.FindEntry(builder.ToString());
							if (!entry.HasFiles)
								return Enumerable.Empty<String>();

							result.Add(entry);
							state = ParserState.LookingForDelimiter;
							builder.Clear();
						}
						else
						{
							builder.Append(character);
						}

						break;
				}
			}

			return result.Cast<IEnumerable<String>>().Aggregate((r1, r2) => r1.Intersect(r2));
		}
	}
}

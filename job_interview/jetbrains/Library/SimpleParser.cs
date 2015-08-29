using System;
using System.Collections.Generic;

namespace TextIndexing.Library
{
	/// <summary>
	/// Represents simple parser that just seeks exact match in index.
	/// </summary>
	public sealed class SimpleParser : IQueryParser
	{
		public IEnumerable<String> GetFiles(String query, IIndex index)
		{
			return index.FindEntry(query);
		}
	}
}

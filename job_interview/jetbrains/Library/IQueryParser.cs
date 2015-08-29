using System;
using System.Collections.Generic;

namespace TextIndexing.Library
{
	public interface IQueryParser
	{
		IEnumerable<String> GetFiles(String query, IIndex index);
	}
}
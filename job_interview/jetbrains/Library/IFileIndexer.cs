using System;
using System.Collections.Generic;
using System.IO;

namespace TextIndexing.Library
{
	public interface IFileIndexer
	{
		IEnumerable<String> GetWords(FileStream fileStream);
	}
}
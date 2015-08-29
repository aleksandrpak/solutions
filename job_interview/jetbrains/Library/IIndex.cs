using System;

namespace TextIndexing.Library
{
	public interface IIndex
	{
		IndexEntry FindEntry(String word);
	}
}
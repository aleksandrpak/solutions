using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TextIndexing.Library
{
	public struct IndexEntry : IEnumerable<String>
	{
		private readonly ConcurrentSet<String> _files;

		internal IndexEntry(ConcurrentSet<String> files)
		{
			_files = files;
		}

		public Boolean HasFiles
		{
			get { return _files != null; }
		}

		public IEnumerator<String> GetEnumerator()
		{
			if (_files == null)
				return Enumerable.Empty<String>().GetEnumerator();

			return _files.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
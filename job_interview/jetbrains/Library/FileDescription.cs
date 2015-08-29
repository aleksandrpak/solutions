using System;
using System.Collections.Generic;

namespace TextIndexing.Library
{
	internal sealed class FileDescription
	{
		public FileDescription()
		{
			References = new HashSet<ConcurrentSet<String>>();
			LastWriteTime = DateTime.MinValue;
		}

		public HashSet<ConcurrentSet<String>> References { get; private set; }

		public DateTime LastWriteTime { get; set; }
	}
}
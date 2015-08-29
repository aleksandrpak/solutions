using System;
using System.IO;

namespace TextIndexing.Library
{
	internal struct WatcherInfo
	{
		public WatcherInfo(FileSystemWatcher watcher, Boolean isDirectory)
			: this()
		{
			Watcher = watcher;

			if (isDirectory)
				WatchedFiles = new ConcurrentSet<String>();
		}

		public FileSystemWatcher Watcher { get; private set; }

		public ConcurrentSet<String> WatchedFiles { get; private set; }
	}
}

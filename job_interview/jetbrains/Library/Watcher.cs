using System;
using System.Collections.Concurrent;
using System.IO;

// ReSharper disable InconsistentlySynchronizedField
// Done for Watchers field which is thread safe.

namespace TextIndexing.Library
{
	// I am sure that I have missed possible events and exceptions that can occur
	// But as was said previously - solution does not have to be production ready
	public static class Watcher
	{
		#region Fields

		private static readonly ConcurrentDictionary<String, WatcherInfo> Watchers;

		private static readonly Func<String, WatcherInfo> FileWatchersFactory;

		private static readonly Func<String, WatcherInfo> DirectoryWatchersFactory;

		private static readonly FileSystemEventHandler FileCreatedOrChangedHandler;

		private static readonly FileSystemEventHandler FileDeletedHandler;

		private static readonly RenamedEventHandler FileRenamedHandler;

		private static readonly ErrorEventHandler FileWatchingErrorHandler;

		private static readonly RenamedEventHandler FileInDirectoryRenamedHandler;

		private static readonly ErrorEventHandler DirectoryWatchingErrorHandler;

		#endregion

		static Watcher()
		{
			Watchers = new ConcurrentDictionary<String, WatcherInfo>();
			FileWatchersFactory = ProduceFileWatcher;
			DirectoryWatchersFactory = ProduceDirectoryWatcher;

			FileCreatedOrChangedHandler = HandleFileCreatedOrChanged;
			FileDeletedHandler = HandleFileDeleted;
			FileRenamedHandler = HandleFileRenamed;
			FileWatchingErrorHandler = HandleFileWatchingError;

			DirectoryWatchingErrorHandler = HandleDirectoryWatchingError;
			FileInDirectoryRenamedHandler = HandleFileInDirectoryRenamed;
		}

		public static Error WatchFile(String filename)
		{
			if (String.IsNullOrWhiteSpace(filename))
				return Error.InvalidName;

			var directory = Path.GetDirectoryName(filename);
			if (String.IsNullOrWhiteSpace(directory))
				return Error.InvalidName;

			if (!File.Exists(filename))
				return Error.DoesNotExist;

			// ReSharper disable once AssignNullToNotNullAttribute
			// Resharper does not recognize IsNullOrWhiteSpace as check for null
			if (Watchers.ContainsKey(filename) || IsDirectoryWatched(directory))
				return Error.AlreadyIndexed;

			var watcherInfo = Watchers.GetOrAdd(filename, FileWatchersFactory);
			var watcher = watcherInfo.Watcher;
			lock (watcher)
			{
				try
				{
					return WatchNewFile(filename, watcher);
				}
				catch (Exception exception)
				{
					Console.WriteLine("Failed to watch file '{0}': {1}", filename, exception); // TODO: Write to logger

					RemoveFileWatcher(filename);

					return Error.FailedToWatch;
				}
			}
		}

		public static Error WatchDirectory(String directory)
		{
			if (String.IsNullOrWhiteSpace(directory))
				return Error.InvalidName;

			if (!Directory.Exists(directory))
				return Error.DoesNotExist;

			if (IsDirectoryWatched(directory))
				return Error.AlreadyIndexed;

			var watcherInfo = Watchers.GetOrAdd(directory, DirectoryWatchersFactory);
			var watcher = watcherInfo.Watcher;
			lock (watcher)
			{
				try
				{
					foreach (var file in Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories))
					{
						Index.Instance.AddFile(file);
						watcherInfo.WatchedFiles.TryAdd(file);
					}

					watcher.Changed += FileCreatedOrChangedHandler;
					watcher.Created += FileCreatedOrChangedHandler;
					watcher.Deleted += FileDeletedHandler;
					watcher.Error += DirectoryWatchingErrorHandler;
					watcher.Renamed += FileInDirectoryRenamedHandler;

					return Error.Success;
				}
				catch (Exception exception)
				{
					RemoveDirectoryWatcher(directory);
					Console.WriteLine("Failed to watch directory '{0}': {1}", directory, exception); // TODO: Write to logger
					return Error.FailedToIndex;
				}
			}
		}

		#region Directory Handlers

		private static void HandleFileInDirectoryRenamed(Object sender, RenamedEventArgs renamedEventArgs)
		{
			WatcherInfo watcherInfo;
			if (!Watchers.TryGetValue(((FileSystemWatcher)sender).Path, out watcherInfo))
				return;

			lock (watcherInfo.Watcher)
			{
				if (!watcherInfo.WatchedFiles.TryRemove(renamedEventArgs.OldFullPath))
					return;

				Index.Instance.RemoveFile(renamedEventArgs.OldFullPath);

				Index.Instance.AddFile(renamedEventArgs.FullPath);
				watcherInfo.WatchedFiles.TryAdd(renamedEventArgs.FullPath);
			}
		}

		private static void HandleDirectoryWatchingError(Object sender, ErrorEventArgs errorEventArgs)
		{
			WatcherInfo watcherInfo;
			if (!Watchers.TryGetValue(((FileSystemWatcher)sender).Path, out watcherInfo))
				return;

			lock (watcherInfo.Watcher)
			{
				var path = watcherInfo.Watcher.Path;
				foreach (var file in watcherInfo.WatchedFiles)
					Index.Instance.RemoveFile(file);

				RemoveDirectoryWatcher(path);
			}
		}

		private static void RemoveDirectoryWatcher(String directory)
		{
			WatcherInfo watcherInfo;
			if (Watchers.TryRemove(directory, out watcherInfo))
				watcherInfo.Watcher.Dispose();
		}

		private static Boolean IsDirectoryWatched(String directory)
		{
			if (Watchers.ContainsKey(directory))
				return true;

			// It is possible that we will find key and return
			// But in next moment that key will be removed and should be added
			// To handle this we should lock more actions and decrease performance
			var parent = Path.GetDirectoryName(directory);
			while (parent != null)
			{
				// ReSharper disable once AssignNullToNotNullAttribute
				if (Watchers.ContainsKey(parent))
					return true;

				parent = Path.GetDirectoryName(parent);
			}

			return false;
		}

		private static void UpdateWatchedFiles(String path, FileSystemWatcher watcher, Boolean isAdd)
		{
			if (watcher.Filter != "*.*")
				return;

			WatcherInfo watcherInfo;
			if (!Watchers.TryGetValue(watcher.Path, out watcherInfo))
				return;

			lock (watcherInfo.Watcher)
			{
				if (isAdd)
					watcherInfo.WatchedFiles.TryAdd(path);
				else
					watcherInfo.WatchedFiles.TryRemove(path);
			}
		}

		#endregion

		#region File Handlers

		private static void HandleFileCreatedOrChanged(Object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			if (!File.Exists(fileSystemEventArgs.FullPath))
				return;

			var watcher = sender as FileSystemWatcher;
			if (watcher == null)
				return;

			Index.Instance.AddFile(fileSystemEventArgs.FullPath);

			UpdateWatchedFiles(fileSystemEventArgs.FullPath, watcher, true);
		}

		private static void HandleFileDeleted(Object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			var watcher = sender as FileSystemWatcher;
			if (watcher == null)
				return;

			var path = fileSystemEventArgs.FullPath;
			if (RemoveFileWatcher(path))
			{
				UpdateWatchedFiles(path, watcher, false);
			}
			else if (String.IsNullOrEmpty(Path.GetExtension(path))) // Bad but mostly working check
			{
				WatcherInfo watcherInfo;
				if (!Watchers.TryGetValue(watcher.Path, out watcherInfo) || watcherInfo.WatchedFiles == null)
					return;

				lock (watcherInfo.Watcher)
				{
					foreach (var file in watcherInfo.WatchedFiles)
					{
						if (file.StartsWith(path))
						{
							Index.Instance.RemoveFile(file);
							UpdateWatchedFiles(file, watcherInfo.Watcher, false);
						}
					}
				}
			}
		}

		private static void HandleFileRenamed(Object sender, RenamedEventArgs renamedEventArgs)
		{
			RemoveFileWatcher(renamedEventArgs.OldFullPath);
		}

		private static void HandleFileWatchingError(Object sender, ErrorEventArgs errorEventArgs)
		{
			var watcher = sender as FileSystemWatcher;
			if (watcher == null)
				return;

			RemoveFileWatcher(Path.Combine(watcher.Path, watcher.Filter));
		}

		private static Error WatchNewFile(String filename, FileSystemWatcher watcher)
		{
			// We will try to index any file
			var result = Index.Instance.AddFile(filename);
			if (result == Error.FailedToIndex)
			{
				DisposeFileWatcher(filename);
				return result;
			}

			watcher.Renamed += FileRenamedHandler;
			watcher.Deleted += FileDeletedHandler;
			watcher.Changed += FileCreatedOrChangedHandler;
			watcher.Error += FileWatchingErrorHandler;
			return result;
		}

		private static Boolean RemoveFileWatcher(String filename)
		{
			Index.Instance.RemoveFile(filename);
			return DisposeFileWatcher(filename);
		}

		private static Boolean DisposeFileWatcher(String filename)
		{
			WatcherInfo watcherInfo;
			if (!Watchers.TryRemove(filename, out watcherInfo))
				return false;

			var watcher = watcherInfo.Watcher;
			lock (watcher)
			{
				// No need to unsubscribe from events because they do not prevent garbage collection
				watcher.Dispose();
				return true;
			}
		}

		#endregion

		#region Watchers Factories

		private static WatcherInfo ProduceFileWatcher(String filename)
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			// This should be called only after all checks
			return new WatcherInfo(new FileSystemWatcher(Path.GetDirectoryName(filename), Path.GetFileName(filename))
				{
					IncludeSubdirectories = false,
					EnableRaisingEvents = true,
					NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite
						| NotifyFilters.Size | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.Attributes
				}, false);
		}

		private static WatcherInfo ProduceDirectoryWatcher(String directory)
		{
			return new WatcherInfo(new FileSystemWatcher(directory)
				{
					IncludeSubdirectories = true,
					EnableRaisingEvents = true,
					NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite
						| NotifyFilters.Size | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.Attributes
				}, true);
		}

		#endregion
	}
}

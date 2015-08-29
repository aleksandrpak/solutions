using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TextIndexing.Library
{
	public sealed class Index : IIndex
	{
		#region Fields

		/// <summary>
		/// The single instance of the index.
		/// </summary>
		public static readonly Index Instance;

		/// <summary>
		/// The relation between word and files that contain that word.
		/// </summary>
		private readonly ConcurrentDictionary<String, ConcurrentSet<String>> _wordIndex;

		/// <summary>
		/// The relation between file and collections that have reference to this file.
		/// </summary>
		private readonly ConcurrentDictionary<String, FileDescription> _fileIndex;

		/// <summary>
		/// The saved delegate to exclude allocations.
		/// </summary>
		private readonly Func<String, FileDescription> _fileDescriptionFactory;

		/// <summary>
		/// The saved delegate to exclude allocations.
		/// </summary>
		private readonly Func<String, ConcurrentSet<String>> _wordReferenceFactory;

		private IQueryParser _parser;

		private IFileIndexer _indexer;

		#endregion

		static Index()
		{
			Instance = new Index();
		}

		private Index()
		{
			_wordIndex = new ConcurrentDictionary<String, ConcurrentSet<String>>();
			_fileIndex = new ConcurrentDictionary<String, FileDescription>();

			_fileDescriptionFactory = ProduceFileDescription;
			_wordReferenceFactory = ProduceWordReference;
		}

		// Not best solution but done for simplicity
		// Read first idea in Service project that would allow to avoid this hack
		public void Init(IQueryParser parser, IFileIndexer indexer)
		{
			if (parser == null)
				throw new ArgumentNullException("parser");

			if (indexer == null)
				throw new ArgumentNullException("indexer");

			if (_parser != null || _indexer != null)
				throw new InvalidOperationException("Cannot initialize index more than one time.");

			_parser = parser;
			_indexer = indexer;
		}

		public IEnumerable<String> GetFiles(String query)
		{
			return _parser.GetFiles(query, this);
		}

		IndexEntry IIndex.FindEntry(String word)
		{
			ConcurrentSet<String> files;
			_wordIndex.TryGetValue(word.ToLowerInvariant(), out files);
			return new IndexEntry(files);
		}

		internal Error AddFile(String filename)
		{
			var description = _fileIndex.GetOrAdd(filename, _fileDescriptionFactory);
			lock (description)
			{
				var references = description.References;

				try
				{
					var stopwatch = Stopwatch.StartNew();

					// We should open file first to make sure that it exists and to prevent any write to this file
					using (var stream = File.OpenRead(filename))
					{
						var lastWriteTime = File.GetLastWriteTimeUtc(filename);
						if (description.LastWriteTime == lastWriteTime)
							return Error.AlreadyIndexed;

						// Removing old index information so deleting of word in file can work
						// Maybe better to make diff between old and new
						foreach (var reference in references)
							reference.TryRemove(filename);

						foreach (var word in _indexer.GetWords(stream))
						{
							var reference = _wordIndex.GetOrAdd(word.ToLowerInvariant(), _wordReferenceFactory);
							reference.TryAdd(filename);
							references.Add(reference);
						}

						description.LastWriteTime = File.GetLastWriteTimeUtc(filename);
					}

					stopwatch.Stop();

					Console.WriteLine("Indexed file: {0}. Words in index: {1}. Elapsed time: {2}ms", 
						filename, _wordIndex.Count, stopwatch.ElapsedMilliseconds); // TODO: Write to logger

					return Error.Success;
				}
				catch (Exception exception)
				{
					Console.WriteLine("Failed to index file: {0}", exception); // TODO: Write to logger
					return Error.FailedToIndex; // I miss ADT in such cases
				}
			}
		}

		internal void RemoveFile(String filename)
		{
			FileDescription description;
			if (!_fileIndex.TryGetValue(filename, out description))
				return;

			// Usually lock is done on a distinct lock object
			// But the idea is to lock on private object
			// This object is not visible outside this class
			// So there is no need to make separate lock object
			lock (description)
			{
				// Can occur if file is empty or currently trying to index but this thread acquired lock faster
				// First case is ok. Second case will be solved by checking file existance after taking lock
				if (description.References.Count == 0)
					return;

				foreach (var reference in description.References)
				{
					// For simplicity we will not remove from index empty collections
					// Depending on usage of index it can be bad or good decision
					reference.TryRemove(filename);
				}

				FileDescription fileDescription;
				_fileIndex.TryRemove(filename, out fileDescription);
			}
			
			Console.WriteLine("Removed file from index: {0}", filename); // TODO: Write to logger
		}

		#region Value Factories

		private static FileDescription ProduceFileDescription(String filename)
		{
			return new FileDescription();
		}

		private static ConcurrentSet<String> ProduceWordReference(String word)
		{
			return new ConcurrentSet<String>();
		}

		#endregion
	}
}

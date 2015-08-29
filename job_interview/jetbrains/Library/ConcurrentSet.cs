using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace TextIndexing.Library
{
	internal sealed class ConcurrentSet<TItem> : IEnumerable<TItem>
	{
		/// <summary>
		/// The small hack because <see cref="ConcurrentBag{T}"/> base on thread local values and does not allow removing of specific element.
		/// Actually it is better to write own implementation from scratch.
		/// </summary>
		private readonly ConcurrentDictionary<TItem, Byte> _items;

		/// <summary>
		/// The version of current set.
		/// </summary>
		private Int32 _version;

		public ConcurrentSet()
		{
			_items = new ConcurrentDictionary<TItem, Byte>();	
		}

		public Int32 Version
		{
			get { return _version; }
		}

		public IEnumerator<TItem> GetEnumerator()
		{
			foreach (var item in _items)
				yield return item.Key;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Boolean TryAdd(TItem item)
		{
			var result = _items.TryAdd(item, 0);
			if (result)
				Interlocked.Increment(ref _version);

			return result;
		}

		public Boolean TryRemove(TItem item)
		{
			Byte value;
			var result = _items.TryRemove(item, out value);
			if (result)
				Interlocked.Increment(ref _version);

			return result;
		}
	}
}

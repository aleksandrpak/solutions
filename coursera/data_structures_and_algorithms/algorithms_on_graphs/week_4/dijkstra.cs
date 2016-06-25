using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

class HeapNode<T> : IComparable<HeapNode<T>>, IEquatable<HeapNode<T>> where T: IComparable<T>, IEquatable<T> {
	public HeapNode(T value) {
		this.value = value;
	}

	public T value { get; set; }

	public int CompareTo(HeapNode<T> other) {
		return value.CompareTo(other.value);
	}

	public bool Equals(HeapNode<T> other) {
		return value.Equals(other.value);
	}

	public override bool Equals(object other) {
		return Equals((HeapNode<T>)other);
	}

	public override int GetHashCode() {
		return value.GetHashCode();
	}
}

class BinaryHeap<T> where T: IComparable<T>, IEquatable<T> {
	List<HeapNode<T>> items;
	Dictionary<HeapNode<T>, int> nodeToIndex;

	public BinaryHeap() {
		items = new List<HeapNode<T>>();
		nodeToIndex = new Dictionary<HeapNode<T>, int>();
	}

	public int Count {
		get { return items.Count; }
	}

	public HeapNode<T> add(T value) {
		var node = new HeapNode<T>(value);

		nodeToIndex[node] = items.Count;
		items.Add(node);

		siftUp(items.Count - 1);
		return node;
	}

	public T extractMin() {
		if (items.Count == 0) {
			throw new InvalidOperationException();
		}

		nodeToIndex.Remove(items[0]);
		var value = items[0].value;
		items[0] = items[items.Count - 1];
		items.RemoveAt(items.Count - 1);

		if (items.Count > 0) {
			nodeToIndex[items[0]] = 0;
		}

		siftDown(0);
		return value;
	}

	public void updatePriority(HeapNode<T> node) {
		int index;
		if (nodeToIndex.TryGetValue(node, out index)) {
			siftUp(index);
			siftDown(index);
		}
	}

	void siftUp(int index) {;
		var parent = getParent(index);
		if (items[index].CompareTo(items[parent]) < 0) {
			swapNodes(index, parent);
			siftUp(parent);
		}
	}

	void siftDown(int index) {
		var left = getLeftChild(index);
		var right = getRightChild(index);
		var swap = index;

		if (left < items.Count && items[left].CompareTo(items[swap]) < 0) {
			swap = left;
		}

		if (right < items.Count && items[right].CompareTo(items[swap]) < 0) {
			swap = right;
		}

		if (swap != index) {
			swapNodes(index, swap);
			siftDown(swap);
		}
	}

	void swapNodes(int x, int y) {
		var xNode = items[x];
		var yNode = items[y];

		items[x] = yNode;
		items[y] = xNode;

		nodeToIndex[xNode] = y;
		nodeToIndex[yNode] = x;
	}
	
	int getParent(int index) {
		return (index - 1) / 2;
	}

	int getLeftChild(int index) {
		return index * 2 + 1;
	}

	int getRightChild(int index) {
		return index * 2 + 2;
	}

	public override string ToString() {
		var builder = new StringBuilder();
		foreach (var item in items) {
			builder.AppendLine(item.value.ToString());
		}
		return builder.ToString();
	}
}

static class Program {
	struct Path : IComparable<Path>, IEquatable<Path> {
		public Path(int vertex, long distance) : this() {
			this.vertex = vertex;
			this.distance = distance;
		}

		public int vertex { get; private set; }
		public long distance { get; private set; }

		public int CompareTo(Path other) {
			return distance.CompareTo(other.distance);
		}

		public bool Equals(Path other) {
			return vertex == other.vertex;
		}

		public override bool Equals(object other) {
			return Equals((Path)other);
		}

		public override int GetHashCode() {
			return vertex.GetHashCode();
		}
	}

	static int[] readLine() {
		return Console.ReadLine().Split(new[] {' '}).Select(int.Parse).ToArray();
	}

	static List<Path>[] readGraph() {
		var nm = readLine();
		var graph = new List<Path>[nm[0]];

		for (var i = 0; i < nm[1]; i++) {
			var xyw = readLine();
			var x = xyw[0] - 1;

			if (graph[x] == null) {
				graph[x] = new List<Path>();
			}

			graph[x].Add(new Path(xyw[1] - 1, xyw[2]));
		}

		return graph;
	}
	
	static long[] getInitialDist(int n) {
		var dist = new long[n];
		for (var i = 0; i < n; i++) {
			dist[i] = long.MaxValue;
		}

		return dist;
	}

	static BinaryHeap<Path> buildHeap(long[] dist, Dictionary<int, HeapNode<Path>> vertixToNode) {
		var heap = new BinaryHeap<Path>();
		
		for (var i = 0; i < dist.Length; i++) {
			vertixToNode[i] = heap.add(new Path(i, dist[i]));
		}

		return heap;
	}

	static void Main() {
		var graph = readGraph();
		var st = readLine().Select(i => i - 1).ToArray();
		var vertixToNode = new Dictionary<int, HeapNode<Path>>();

		var dist = getInitialDist(graph.Length);
		dist[st[0]] = 0;

		var heap = buildHeap(dist, vertixToNode);
		var result = -1L;

		while (heap.Count > 0) {
			var node = heap.extractMin();
			var u = node.vertex;

			if (u == st[1]) {
				result = dist[u];
				break;
			}

			if (graph[u] == null) {
				continue;
			}

			foreach (var edge in graph[u]) {
				var v = edge.vertex;
				if (dist[v] > dist[u] + edge.distance) {
					dist[v] = dist[u] + edge.distance;

					var vNode = vertixToNode[v];
					vNode.value = new Path(v, dist[v]);
					heap.updatePriority(vNode);
				}
			}
		}

		if (result == long.MaxValue || result < 0) {
			Console.WriteLine(-1);
		} else {
			Console.WriteLine(result);
		}
	}
}

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
	struct Point {
		public Point(double x, double y) : this() {
			this.x = x;
			this.y = y;
		}

		public double x { get; private set; }
		public double y { get; private set; }
	}

	struct Path : IComparable<Path>, IEquatable<Path> {
		public Path(int point, double cost) : this() {
			this.point = point;
			this.cost = cost;
		}

		public int point { get; private set; }
		public double cost { get; private set; }

		public int CompareTo(Path other) {
			return cost.CompareTo(other.cost);
		}

		public bool Equals(Path other) {
			return point == other.point;
		}

		public override bool Equals(object other) {
			return Equals((Path)other);
		}

		public override int GetHashCode() {
			return point.GetHashCode();
		}
	}

	static int[] readLine() {
		return Console.ReadLine().Split(new[] {' '}).Select(int.Parse).ToArray();
	}

	static double[,] readGraph() {
		var n = readLine()[0];
		var points = new List<Point>();

		for (var i = 0; i < n; i++) {
			var xy = readLine();
			var x = xy[0];
			var y = xy[1];

			points.Add(new Point(x, y));
		}

		var graph = new double[n, n];
		for (var i = 0; i < n; i++) {
			for (var j = 0; j < n; j++) {
				if (i == j) {
					continue;
				}

				graph[i, j] = Math.Sqrt(Math.Pow(points[i].x - points[j].x, 2) + Math.Pow(points[i].y - points[j].y, 2));
			}
		}

		return graph;
	}
	
	static double[] getInitialCosts(int n) {
		var cost = new double[n];
		for (var i = 0; i < n; i++) {
			cost[i] = double.MaxValue;
		}

		return cost;
	}

	static BinaryHeap<Path> buildHeap(double[] cost, Dictionary<int, HeapNode<Path>> vertixToNode) {
		var heap = new BinaryHeap<Path>();
		
		for (var i = 0; i < cost.Length; i++) {
			vertixToNode[i] = heap.add(new Path(i, cost[i]));
		}

		return heap;
	}

	static void Main() {
		var graph = readGraph();
		var n = graph.GetLength(0);
		var vertixToNode = new Dictionary<int, HeapNode<Path>>();

		var costs = getInitialCosts(n);
		costs[0] = 0;

		var heap = buildHeap(costs, vertixToNode);

		while (heap.Count > 0) {
			var node = heap.extractMin();
			var u = node.point;
			vertixToNode.Remove(u);

			for (var v = 0; v < n; v++) {
				if (!vertixToNode.ContainsKey(v)) {
					continue;
				}

				var cost = graph[u, v];

				if (costs[v] > cost) {
					costs[v] = cost;
					var vNode = vertixToNode[v];
					vNode.value = new Path(v, cost);
					heap.updatePriority(vNode);
				}
			}
		}
		
		Console.WriteLine("{0:f9}", costs.Sum());
	}
}

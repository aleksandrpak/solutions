using System;
using System.Linq;
using System.Collections.Generic;

class DisjointSet<T> where T: IEquatable<T> {
	readonly Dictionary<T, T> parents;
	readonly Dictionary<T, int> ranks;
	int count;

	public DisjointSet(IEnumerable<T> values) {
		parents = new Dictionary<T, T>();
		ranks = new Dictionary<T, int>();

		foreach (var value in values) {
			parents[value] = value;
			ranks[value] = 1;
		}

		count = parents.Count;
	}

	public int Count {
		get { return count; }
	}

	public T GetParent(T value) {
		if (!value.Equals(parents[value])) {
			parents[value] = GetParent(parents[value]);
		}

		return parents[value];
	}

	public T Merge(T x, T y) {
		x = GetParent(x);
		y = GetParent(y);

		if (x.Equals(y)) {
			return x;
		}

		if (ranks[x] > ranks[y]) {
			parents[y] = x;
		} else {
			parents[x] = y;
			
			if (ranks[x] == ranks[y]) {
				ranks[y] += 1;
			}
		}

		count--;
		return parents[x];
	}
}

static class Program {
	struct Point : IEquatable<Point> {
		public Point(int x, int y) : this() {
			X = x;
			Y = y;
		}

		public int X { get; private set; }
		public int Y { get; private set; }

		public bool Equals(Point other) {
			return X == other.X && Y == other.Y;
		}

		public override bool Equals(object other) {
			return Equals((Point)other);
		}

		public override int GetHashCode() {
			return Tuple.Create(X, Y).GetHashCode();
		}
	}

	struct Edge : IEquatable<Edge>, IComparable<Edge> {
		public Edge(Point a, Point b, double distance) : this() {
			A = a;
			B = b;
			Distance = distance;
		}

		public Point A { get; private set; }
		public Point B { get; private set; }
		public double Distance { get; private set; }

		public bool Equals(Edge other) {
			return A.Equals(other.A) && B.Equals(other.B) && Distance == other.Distance;
		}

		public override bool Equals(object other) {
			return Equals((Edge)other);
		}

		public override int GetHashCode() {
			return Tuple.Create(A.X, A.Y, B.X, B.Y, Distance).GetHashCode();
		}

		public int CompareTo(Edge other) {
			return Distance.CompareTo(other.Distance);
		}
	}

	static Point[] ReadPoints() {
		var n = int.Parse(Console.ReadLine());
		var points = new Point[n];

		for (var i = 0; i < n; i++) {
			var xy = Console.ReadLine().Split(new [] {' '}).Select(int.Parse).ToArray();
			points[i] = new Point(xy[0], xy[1]);
		}

		return points;
	}

	static Edge[] CalculateEdges(Point[] points) {
		var n = points.Length;
		var edges = new Edge[n * n];

		for (var i = 0; i < n; i++) {
			for (var j = 0; j < n; j++) {
				if (i == j) {
					continue;
				}

				var a = points[i];
				var b = points[j];
				var d = Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
				edges[i * n + j] = new Edge(a, b, d);
			}
		}
		
		Array.Sort(edges);
		return edges;
	}

	static void Main() {
		var points = ReadPoints();
		var k = int.Parse(Console.ReadLine());
		var sets = new DisjointSet<Point>(points);
		var edges = CalculateEdges(points);

		for (var i = points.Length; i < edges.Length; i++) {
			var edge = edges[i];

			if (sets.Count == k) {
				if (sets.GetParent(edge.A).Equals(sets.GetParent(edge.B))) {
					continue;
				}

				Console.WriteLine("{0:f9}", edge.Distance);
				return;
			}
			
			sets.Merge(edge.A, edge.B);
		}

		Console.WriteLine("0.000000000");
	}
}

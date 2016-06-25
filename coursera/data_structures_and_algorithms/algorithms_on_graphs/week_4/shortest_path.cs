using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

static class Program {
	struct Edge {
		public Edge(int vertex, long weight) : this() {
			this.vertex = vertex;
			this.weight = weight;
		}

		public int vertex { get; private set; }
		public long weight { get; private set; }
	}

	static int[] readLine() {
		return Console.ReadLine().Split(new[] {' '}).Select(int.Parse).ToArray();
	}

	static List<Edge>[] readGraph() {
		var nm = readLine();
		var graph = new List<Edge>[nm[0]];

		for (var i = 0; i < nm[1]; i++) {
			var xyw = readLine();
			var x = xyw[0] - 1;

			if (graph[x] == null) {
				graph[x] = new List<Edge>();
			}

			graph[x].Add(new Edge(xyw[1] - 1, xyw[2]));
		}

		return graph;
	}

	static IEnumerable<int> bellmanFord(List<Edge>[] graph, long[] dist) {
		for (var u = 0; u < graph.Length; u++) {
			var edges = graph[u];
			if (edges == null) {
				continue;
			}

			foreach (var edge in edges) {
				if (dist[u] == long.MaxValue) {
					continue;
				}

				var v = edge.vertex;
				if (dist[v] > dist[u] + edge.weight) {
					dist[v] = dist[u] + edge.weight;

					yield return u;
					yield return v;
				}
			}
		}
	}
	
	static long[] initDist(int n) {
		var dist = new long[n];
		for (var i = 0; i < n; i++) {
			dist[i] = long.MaxValue;
		}

		return dist;
	}

	static void Main() {
		var graph = readGraph();
		var s = int.Parse(Console.ReadLine()) - 1;
		var dist = initDist(graph.Length);
		dist[s] = 0;

		for (var i = 0; i < graph.Length; i++) {
			bellmanFord(graph, dist).Count();
		}

		var visited = new HashSet<int>();
		var toVisit = new Queue<int>(new HashSet<int>(bellmanFord(graph, dist)));

		while (toVisit.Count > 0) {
			var x = toVisit.Dequeue();
			if (visited.Contains(x)) {
				continue;
			}

			visited.Add(x);

			if (graph[x] == null) {
				continue;
			}

			foreach (var edge in graph[x]) {
				if (!visited.Contains(edge.vertex)) {
					toVisit.Enqueue(edge.vertex);
				}
			}
		}

		for (var i = 0; i < graph.Length; i++) {
			if (visited.Contains(i)) {
				Console.WriteLine("-");
			} else if (i == s) {
				Console.WriteLine(0);
			} else if (dist[i] == long.MaxValue) {
				Console.WriteLine("*");
			} else {
				Console.WriteLine(dist[i]);
			}
		}
	}
}

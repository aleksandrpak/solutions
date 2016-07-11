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
				var v = edge.vertex;
				if (dist[v] > dist[u] + edge.weight) {
					dist[v] = dist[u] + edge.weight;

					yield return u;
					yield return v;
				}
			}
		}
	}

	static void Main() {
		var graph = readGraph();
		var dist = new long[graph.Length];

		for (var i = 0; i < graph.Length; i++) {
			bellmanFord(graph, dist).Count();
		}

		Console.WriteLine(bellmanFord(graph, dist).Any() ? 1 : 0);
	}
}

#Uses python3

import sys
import queue

def bipartite(adj):
    used = [0] * len(adj)
    color = [-1] * len(adj)

    for v in range(len(adj)):
        if used[v] == 1:
            continue

        vs = queue.Queue()
        vs.put(v)
        used[v] = 1
        color[v] = 0

        while not vs.empty():
            x = vs.get()

            for e in adj[x]:
                if color[e] > -1 and color[e] == color[x]:
                    return 0

                color[e] = (color[x] + 1) % 2
                
                if used[e] == 0:
                    used[e] = 1
                    vs.put(e)

    return 1

if __name__ == '__main__':
    input = sys.stdin.read()
    data = list(map(int, input.split()))
    n, m = data[0:2]
    data = data[2:]
    edges = list(zip(data[0:(2 * m):2], data[1:(2 * m):2]))
    adj = [[] for _ in range(n)]
    for (a, b) in edges:
        adj[a - 1].append(b - 1)
        adj[b - 1].append(a - 1)
    print(bipartite(adj))

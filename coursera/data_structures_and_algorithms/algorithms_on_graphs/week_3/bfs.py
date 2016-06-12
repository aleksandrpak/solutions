#Uses python3

import sys
import queue

def distance(adj, s, t):
    vs = queue.Queue()
    vs.put((1, s))
    visited = set([])

    while not vs.empty():
        d, v = vs.get()        
        visited.add(v)

        if t in adj[v]:
            return d
        
        for e in adj[v]:
            if e not in visited:
                vs.put((d + 1, e))

    return -1

if __name__ == '__main__':
    input = sys.stdin.read()
    data = list(map(int, input.split()))
    n, m = data[0:2]
    data = data[2:]
    edges = list(zip(data[0:(2 * m):2], data[1:(2 * m):2]))
    adj = [set([]) for _ in range(n)]
    for (a, b) in edges:
        adj[a - 1].add(b - 1)
        adj[b - 1].add(a - 1)
    s, t = data[2 * m] - 1, data[2 * m + 1] - 1
    print(distance(adj, s, t))

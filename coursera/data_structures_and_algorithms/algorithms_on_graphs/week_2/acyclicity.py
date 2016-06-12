#Uses python3

import sys

def explore(v, adj, path, visited):
    if v in path:
        return True

    visited.add(v)
    path.add(v)

    for e in adj[v]:
        if explore(e, adj, path, visited):
            return True

    path.remove(v)

    return False


def acyclic(adj):
    visited = set([])
    for v in range(len(adj)):
        if v not in visited and explore(v, adj, set([]), visited):
            return 1

    return 0


if __name__ == '__main__':
    input = sys.stdin.read()
    data = list(map(int, input.split()))
    n, m = data[0:2]
    data = data[2:]
    edges = list(zip(data[0:(2 * m):2], data[1:(2 * m):2]))
    adj = [[] for _ in range(n)]
    for (a, b) in edges:
        adj[a - 1].append(b - 1)
    print(acyclic(adj))

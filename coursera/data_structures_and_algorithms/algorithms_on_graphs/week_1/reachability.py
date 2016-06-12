#Uses python3

import sys

def reach(adj, x, y):
    if x not in adj:
        return 0

    visit = [x]
    visited = set([])

    while len(visit) > 0:
        v = visit.pop()
        if v in adj and y in adj[v]:
            return 1

        visited.add(v)
        for e in adj[v]:
            if e not in visited:
                visit.append(e)

    return 0

def add_edge(adj, x, y):
    if x not in adj:
        adj[x] = set([])

    adj[x].add(y)

if __name__ == '__main__':
    input = sys.stdin.read()
    data = list(map(int, input.split()))

    n, m = data[0:2]
    data = data[2:]
    edges = list(zip(data[0:(2 * m):2], data[1:(2 * m):2]))
    x, y = data[2 * m:]
    adj = {}

    for (a, b) in edges:
        add_edge(adj, a, b)
        add_edge(adj, b, a)

    print(reach(adj, x, y))

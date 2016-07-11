#Uses python3

import sys

sys.setrecursionlimit(200000)

def get_reversed(adj):
    radj = [[] for _ in range(len(adj))]
    
    for v in range(len(adj)):
        for e in adj[v]:
            radj[e].append(v)

    return radj


def get_order(visited, adj, order, v):
    if v in visited:
        return

    visited.add(v)

    for e in adj[v]:
        get_order(visited, adj, order, e)

    order.append(v)


def number_of_strongly_connected_components(adj):
    result = 0
    radj = get_reversed(adj)
    visited = set([])
    order = []

    for v in range(len(adj)):
        get_order(visited, radj, order, v)
        
    visited = set([])
    for v in reversed(order):
        c = len(visited)
        get_order(visited, adj, [], v)
        
        if len(visited) > c:
            result += 1

    return result

if __name__ == '__main__':
    input = sys.stdin.read()
    data = list(map(int, input.split()))
    n, m = data[0:2]
    data = data[2:]
    edges = list(zip(data[0:(2 * m):2], data[1:(2 * m):2]))
    adj = [[] for _ in range(n)]
    for (a, b) in edges:
        adj[a - 1].append(b - 1)
        
    print(number_of_strongly_connected_components(adj))

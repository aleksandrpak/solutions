#Uses python3

import sys

def explore(visited, adj, x):
    if x in visited:
        return visited

    visited.add(x)

    if x not in adj:
        return visited

    for y in adj[x]:
        visited = explore(visited, adj, y)

    return visited

def number_of_components(adj, n):
    result = 0

    visited = set([])
    for x in range(n):
        c = len(visited)
        visited = explore(visited, adj, x + 1)
        if len(visited) > c:
            result += 1
    
    return result

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
    adj = {}

    for (a, b) in edges:
        add_edge(adj, a, b)
        add_edge(adj, b, a)

    print(number_of_components(adj, n))

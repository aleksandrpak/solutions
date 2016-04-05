# Uses python3
def fib_huge(n, m):
    if (m == 1):
        return 0

    ns = [0, 1, 1]
    f = (1, 1)
    p = set([])

    while (f not in p):
        next = (f[0] + f[1]) % m
        ns.append(next)
        p.add(f)
        f = (f[1], next)

    r = n % len(p)

    return ns[r]

n, m = map(int, input().split())
print(fib_huge(n, m))

# Uses python3
def gcd(a, b):
    if b == 0:
        return a

    return gcd(b, a % b)

def lcm(a, b):
    d = gcd(a, b)
    p = a * b

    return p // d

a, b = map(int, input().split())
a, b = max(a, b), min(a, b)

print(lcm(a, b))


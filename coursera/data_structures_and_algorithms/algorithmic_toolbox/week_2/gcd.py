# Uses python3
def gcd(a, b):
    if b == 0:
        return a

    return gcd(b, a % b)
    
a, b = map(int, input().split())
a, b = max(a, b), min(a, b)

print(gcd(a, b))


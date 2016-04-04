# Uses python3
def calc_fib(n):
    if (n <= 1):
        return n

    n1, n2 = 1, 1
    for i in range(2, n):
        n1, n2 = (n1 + n2), n1

    return n1

n = int(input())
print(calc_fib(n))

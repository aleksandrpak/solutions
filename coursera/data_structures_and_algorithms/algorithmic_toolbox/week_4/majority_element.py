# Uses python3
import sys

def get_majority_element(a):
    n = len(a)
    m, c = a[0], 1

    for i in range(1, n):
        ai = a[i]
        if (ai == m):
            c += 1
        else:
            c -= 1

        if (c == 0):
            m = ai
            c = 1

    c = 0
    for i in range(0, n):
        if (a[i] == m):
            c += 1

    return int(c > n // 2)

if __name__ == '__main__':
    input = sys.stdin.read()
    n, *a = list(map(int, input.split()))
    print(get_majority_element(a))

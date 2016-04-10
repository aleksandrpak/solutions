# Uses python3
import sys

def optimal_summands(n):
    l = 1
    k = n
    result = []

    while (True):
        if (k <= 2 * l):
            result.append(k)
            break
        else:
            result.append(l)
            l, k = l + 1, k - l

    return result

if __name__ == '__main__':
    input = sys.stdin.read()
    n = int(input)
    summands = optimal_summands(n)
    print(len(summands))
    for x in summands:
        print(x, end=' ')

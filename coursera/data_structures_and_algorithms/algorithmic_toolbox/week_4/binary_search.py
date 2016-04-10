# Uses python3
import sys

def binary_search(a, x):
    l, h = 0, len(a)

    while (True):
        if (l > h or l == len(a) or h < 0):
            return -1

        mi = (h + l) // 2
        m = a[mi]

        if (m == x):
            return mi

        if (m < x):
            l = mi + 1
        else:
            h = mi - 1

if __name__ == '__main__':
    input = sys.stdin.read()
    data = list(map(int, input.split()))
    n = data[0]
    m = data[n + 1]
    a = data[1 : n + 1]
    for x in data[n + 2:]:
        # replace with the call to binary_search when implemented
        print(binary_search(a, x), end = ' ')

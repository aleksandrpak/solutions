# Uses python3
import sys
import random

def partition3(a, l, r):
    x = a[l]
    low, high = l, l

    for i in range(l + 1, r + 1):
        ai = a[i]

        if (ai < x):
            a[i] = a[high + 1]
            a[low] = ai
            a[high + 1] = x
            low, high = low + 1, high + 1
        elif (ai == x):
            if (high + 1 <= r and high + 1 < i):
                a[high + 1], a[i] = x, a[high + 1]

            high += 1

    return low - 1, high + 1


def randomized_quick_sort(a, l, r):
    if l >= r:
        return

    k = random.randint(l, r)
    a[l], a[k] = a[k], a[l]

    m1, m2 = partition3(a, l, r)

    randomized_quick_sort(a, l, m1);
    randomized_quick_sort(a, m2, r);


if __name__ == '__main__':
    input = sys.stdin.read()
    n, *a = list(map(int, input.split()))
    randomized_quick_sort(a, 0, n - 1)
    for x in a:
        print(x, end=' ')

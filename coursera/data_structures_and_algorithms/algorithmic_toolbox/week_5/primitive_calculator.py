# Uses python3
import sys

def optimal_sequence(n):
    s = [None] * n
    s[0] = (1, None)

    for i in range(2, n + 1):
        l1 = s[i - 2][0]
        l2, l3 = 0, 0

        if (i % 2 == 0):
            l2 = s[i // 2 - 1][0]
        if (i % 3 == 0):
            l3 = s[i // 3 - 1][0]

        l, seq = 0, None
        if (l2 != 0 and l2 <= l1 and (l3 == 0 or l2 <= l3)):
            l, seq = l2, 2
        elif (l3 != 0 and l3 <= l1 and (l2 == 0 or l3 <= l2)):
            l, seq = l3, 3
        else:
            l, seq = l1, 1

        s[i - 1] = (l + 1, seq)

    yield n
    while n > 1:
        prev = s[n - 1][1]

        if (prev == 1):
            n -= 1
        elif (prev == 2):
            n = n // 2
        else:
            n = n // 3

        yield n

input = sys.stdin.read()
n = int(input)
sequence = list(optimal_sequence(n))
print(len(sequence) - 1)
for x in reversed(sequence):
    print(x, end=' ')

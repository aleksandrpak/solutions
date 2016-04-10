# Uses python3
import sys

def optimal_sequence(n):
    s = {1:(1, [1])}

    for i in range(2, n + 1):
        l1 = s[i - 1][0]
        l2, l3 = 0, 0

        if (i % 2 == 0):
            l2 = s[i / 2][0]
        if (i % 3 == 0):
            l3 = s[i / 3][0]

        l, seq = 0, []
        if (l2 != 0 and l2 <= l1 and (l3 == 0 or l2 <= l3)):
            l, seq = s[i / 2][0], list(s[i / 2][1])
        elif (l3 != 0 and l3 <= l1 and (l2 == 0 or l3 <= l2)):
            l, seq = s[i / 3][0], list(s[i / 3][1])
        else:
            l, seq = s[i - 1][0], list(s[i - 1][1])

        seq.append(i)
        s[i] = (l + 1, seq)

    return s[n][1]

input = sys.stdin.read()
n = int(input)
sequence = list(optimal_sequence(n))
print(len(sequence) - 1)
for x in sequence:
    print(x, end=' ')

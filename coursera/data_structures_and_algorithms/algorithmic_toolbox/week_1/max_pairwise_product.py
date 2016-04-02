# Uses python3
n = int(input())
a = [int(x) for x in input().split()]
assert(len(a) == n)

max1 = 0
max2 = 0

for x in a:
    if x > max1:
        max1 = x

        if max1 > max2:
            max1, max2 = max2, max1

print(max1 * max2)

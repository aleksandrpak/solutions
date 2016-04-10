# Uses python3
import sys

def count(nums):
    n = len(nums)

    if (n == 1):
        return 0, nums

    x, a = count(nums[0 : n // 2])
    y, b = count(nums[n // 2 : n])
    z, c = merge(a, b)

    return x + y + z, c

def merge(a, b):
    i, j, z = 0, 0, 0
    la, lb = len(a), len(b)
    c = []

    for _ in range(0, la + lb):
        if (j >= lb or i < la and a[i] <= b[j]):
            c.append(a[i])
            i += 1
        else:
            c.append(b[j])
            j += 1
            z += la - i

    return z, c

if __name__ == '__main__':
    input = sys.stdin.read()
    _, *a = list(map(int, input.split()))
    print(count(a)[0])

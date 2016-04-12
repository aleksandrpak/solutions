# Uses python3
import sys

def fast_count_segments(starts, ends, points):
    cnt = [0] * len(points)

    all = []
    all.extend([(x, 'l', 0) for x in starts])
    all.extend([(x, 'r', 0) for x in ends])
    all.extend([(points[i], 'p', i) for i in range(0, len(points))])

    s = 0
    for (_, t, i) in sorted(all, key=lambda x: (x[0], x[1])):
        if (t == 'l'):
            s += 1
        elif (t == 'r'):
            s -= 1
        else:
            cnt[i] = s

    return cnt

if __name__ == '__main__':
    input = sys.stdin.read()
    data = list(map(int, input.split()))
    n = data[0]
    m = data[1]
    starts = data[2:2 * n + 2:2]
    ends   = data[3:2 * n + 2:2]
    points = data[2 * n + 2:]

    cnt = fast_count_segments(starts, ends, points)
    for x in cnt:
        print(x, end=' ')

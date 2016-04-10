# Uses python3
import sys

def fast_count_segments(starts, ends, points):
    cnt = [0] * len(points)

    for i in range(0, len(ends)):


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

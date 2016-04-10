# Uses python3
import sys
from collections import namedtuple

Segment = namedtuple('Segment', 'start end')

def optimal_points(segments):
    points = []

    segments.sort(key=lambda x: x.end, reverse=True)

    while (len(segments) > 0):
        first = segments.pop()
        point = first.end
        points.append(point)

        for i in reversed(range(0, len(segments))):
            segment = segments[i]
            if (segment.start <= point and segment.end >= point):
                del segments[i]

    return points

if __name__ == '__main__':
    input = sys.stdin.read()
    n, *data = map(int, input.split())
    segments = list(map(lambda x: Segment(x[0], x[1]), zip(data[::2], data[1::2])))
    points = optimal_points(segments)
    print(len(points))
    for p in points:
        print(p, end=' ')

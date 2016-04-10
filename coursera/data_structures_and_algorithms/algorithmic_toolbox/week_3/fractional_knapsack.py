# Uses python3
import sys

def get_optimal_value(capacity, weights, values):
    value = 0.

    for (v, w) in sorted([(v / w, w) for (w, v) in zip(weights, values)], reverse=True, key=lambda x: x[0]):
        value += min(capacity, w) * v
        capacity -= w

        if (capacity <= 0):
            break

    return value


if __name__ == "__main__":
    data = list(map(int, sys.stdin.read().split()))
    n, capacity = data[0:2]
    values = data[2:(2 * n + 2):2]
    weights = data[3:(2 * n + 2):2]
    opt_value = get_optimal_value(capacity, weights, values)
    print("{:.10f}".format(opt_value))

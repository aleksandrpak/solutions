# Uses python3
import sys

def optimal_weight(W, w):
    n = len(w)
    values = [[0 for x in range(n + 1)] for x in range(W + 1)] 

    for i in range(1, n + 1):
        for j in range(1, W + 1):
            values[j][i] = values[j][i - 1]

            if (w[i - 1] <= j):
                val = values[j - w[i - 1]][i - 1] + w[i - 1]
                if (values[j][i] < val):
                    values[j][i] = val

    return values[W][n]

if __name__ == '__main__':
    input = sys.stdin.read()
    W, n, *w = list(map(int, input.split()))
    print(optimal_weight(W, w))

# Uses python3
def min_max(m, M, op, i, j):
    mn, mx = None, None

    for k in range(i, j):
        a = evalt(M[i][k], M[k + 1][j], op[k])
        b = evalt(M[i][k], m[k + 1][j], op[k])
        c = evalt(m[i][k], M[k + 1][j], op[k])
        d = evalt(m[i][k], m[k + 1][j], op[k])

        if (mn == None):
            mn = min(a, b, c, d)
        else:
            mn = min(mn, a, b, c, d)

        if (mx == None):
            mx = max(a, b, c, d)
        else:
            mx = max(mx, a, b, c, d)

    return mn, mx

def evalt(a, b, op):
    if op == '+':
        return a + b
    elif op == '-':
        return a - b
    elif op == '*':
        return a * b
    else:
        assert False

def get_maximum_value(dataset):
    n = len(dataset) - len(dataset) // 2
    op = []
    M = [[0 for i in range(n)] for x in range(n)] 
    m = [[0 for i in range(n)] for x in range(n)] 

    for i in range(0, n):
        if (i < n - 1):
            op.append(dataset[i * 2 + 1])

        m[i][i] = int(dataset[i * 2])
        M[i][i] = int(dataset[i * 2])

    for s in range(0, n):
        for i in range(0, n - s - 1):
            j = i + s + 1
            m[i][j], M[i][j] = min_max(m, M, op, i, j)

    return M[0][n - 1]

if __name__ == "__main__":
    print(get_maximum_value(input()))

#Uses python3

def lcs3(a, b, c):
    d = [[[0 for x in range(len(c) + 1)] for x in range(len(b) + 1)] for x in range(len(a) + 1)] 

    for i in reversed(range(len(a))):
        for j in reversed(range(len(b))):
            for k in reversed(range(len(c))):
                d[i][j][k] = d[i + 1][j + 1][k + 1]

                if a[i] == b[j] and b[j] == c[k]:
                    d[i][j][k] += 1

                d[i][j][k] = max(d[i][j][k], d[i + 1][j][k], d[i][j + 1][k], d[i][j][k + 1])

    return d[0][0][0]

def read_input():
    input()
    return list(map(int, input().split()))

print(lcs3(read_input(), read_input(), read_input()))

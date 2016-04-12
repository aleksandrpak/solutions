# Uses python3
def edit_distance(s, t):
    d = [[0 for x in range(len(s) + 1)] for x in range(len(t) + 1)] 
    
    for j in range(0, len(s) + 1):
        d[0][j] = j

    for i in range(0, len(t) + 1):
        d[i][0] = i

    for j in range(1, len(s) + 1):
        for i in range(1, len(t) + 1):
            ii = d[i][j - 1] + 1
            dd = d[i - 1][j] + 1
            ma = d[i - 1][j - 1]
            mi = d[i - 1][j - 1] + 1

            if (s[j - 1] == t[i - 1]):
                d[i][j] = min(ii, dd, ma)
            else:
                d[i][j] = min(ii, dd, mi)

    return d[len(t)][len(s)]

if __name__ == "__main__":
    print(edit_distance(input(), input()))

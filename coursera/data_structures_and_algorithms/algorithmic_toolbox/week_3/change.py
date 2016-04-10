# Uses python3
def get_change(m):
    n, r = divmod(m, 10)

    if (r >= 5):
        n += 1
        r -= 5
    
    return n + r

m = int(input())
print(get_change(m))

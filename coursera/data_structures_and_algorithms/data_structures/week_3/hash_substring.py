# python3

from functools import reduce

def read_input():
    return (input().rstrip(), input().rstrip())


def print_occurrences(output):
    print(' '.join(map(str, output)))


def hash_func(s, p, x):
    return reduce(lambda a, c: (a * x + ord(c)) % p, reversed(s), 0)


def precompute_hashes(t, pl, p, x):
    h = [0] * (len(t) - pl + 1)
    h[-1] = hash_func(t[-pl:], p, x)
    y = reduce(lambda a, _: (a * x) % p, range(pl), 1)

    for i in reversed(range(len(h) - 1)):
        h[i] = (x * h[i + 1] + ord(t[i]) - y * ord(t[i + pl])) % p

    return h


def get_occurrences(pattern, text):
    x = 263
    p = 1000000007
    tl = len(text)
    pl = len(pattern)
    ph = hash_func(pattern, p, x)
    h = precompute_hashes(text, pl, p, x)

    return [i for i in range(tl - pl + 1) if ph == h[i] and pattern == text[i:i+pl]]


if __name__ == '__main__':
    print_occurrences(get_occurrences(*read_input()))


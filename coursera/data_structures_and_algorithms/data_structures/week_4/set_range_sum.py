# python3

from sys import stdin


class Vertex:
    def __init__(self, key):
        (self.key, self.sum, self.left, self.right, self.parent) = (key, key, None, None, None)


def update(v):
    if v is None:
        return
    v.sum = v.key + (v.left.sum if v.left is not None else 0) + (v.right.sum if v.right is not None else 0)
    if v.left is not None:
        v.left.parent = v
    if v.right is not None:
        v.right.parent = v


def small_rotation(v):
    parent = v.parent
    if parent is None:
        return
    grandparent = v.parent.parent
    if parent.left == v:
        m = v.right
        v.right = parent
        parent.left = m
    else:
        m = v.left
        v.left = parent
        parent.right = m
    update(parent)
    update(v)
    v.parent = grandparent
    if grandparent is not None:
        if grandparent.left == parent:
            grandparent.left = v
        else:
            grandparent.right = v


def big_rotation(v):
    if v.parent.left == v and v.parent.parent.left == v.parent:
        # Zig-zig
        small_rotation(v.parent)
        small_rotation(v)
    elif v.parent.right == v and v.parent.parent.right == v.parent:
        # Zig-zig
        small_rotation(v.parent)
        small_rotation(v)
    else:
        # Zig-zag
        small_rotation(v)
        small_rotation(v)


def splay(v):
    if v is None:
        return None
    while v.parent is not None:
        if v.parent.parent is None:
            small_rotation(v)
            break
        big_rotation(v)
    return v


def find(root, key):
    v = root
    last = root
    found = None
    while v is not None:
        if v.key >= key and (found is None or v.key < found.key):
            found = v
        last = v
        if v.key == key:
            break
        if v.key < key:
            v = v.right
        else:
            v = v.left
    root = splay(last)
    return found, root


def split(root, key):
    (result, root) = find(root, key)
    if result is None:
        return root, None
    right = splay(result)
    left = right.left
    right.left = None
    if left is not None:
        left.parent = None
    update(left)
    update(right)
    return left, right


def merge(left, right):
    if left is None:
        return right
    if right is None:
        return left
    while right.left is not None:
        right = right.left
    right = splay(right)
    right.left = left
    update(right)
    return right


# Code that uses splay tree to solve the problem

def insert(root, key):
    (left, right) = split(root, key)

    new_vertex = None

    if right is None or right.key != key:
        new_vertex = Vertex(key)

    return merge(merge(left, new_vertex), right)


def erase(root, key):
    (left, right) = split(root, key)

    if right is None:
        return left
    else:
        if right.right is not None:
            right.right.parent = None
        return merge(left, right.right)


def search(root, key):
    if root is None:
        return root, False

    (left, right) = find(root, key)
    return right, left is not None and left.key == key


def do_sum(root, fr, to):
    (left, middle) = split(root, fr)
    (middle, right) = split(middle, to + 1)
    ans = 0

    if middle is not None:
        ans = middle.sum

    return merge(merge(left, middle), right), ans


MODULO = 1000000001
source = stdin
n = int(source.readline())
last_sum_result = 0
tree = None

for i in range(n):
    line = source.readline().split()
    if line[0] == '+':
        x = int(line[1])
        tree = insert(tree, (x + last_sum_result) % MODULO)
    elif line[0] == '-':
        x = int(line[1])
        tree = erase(tree, (x + last_sum_result) % MODULO)
    elif line[0] == '?':
        x = int(line[1])
        tree, res = search(tree, (x + last_sum_result) % MODULO)
        print('Found' if res else 'Not found')
    elif line[0] == 's':
        l = int(line[1])
        r = int(line[2])
        tree, res = do_sum(tree, (l + last_sum_result) % MODULO, (r + last_sum_result) % MODULO)
        print(res)
        last_sum_result = res % MODULO

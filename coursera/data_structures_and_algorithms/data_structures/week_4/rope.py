# python3

import sys
import threading

sys.setrecursionlimit(10 ** 6)  # max depth of recursion
threading.stack_size(2 ** 25)  # new thread will get stack of such size


class Vertex:
    def __init__(self, key, size):
        (self.key, self.size, self.left, self.right, self.parent) = (key, size, None, None, None)


def update(v):
    if v is None:
        return

    v.size = 1
    update_parent(v, v.left)
    update_parent(v, v.right)


def update_parent(parent, child):
    if child is not None:
        parent.size += child.size
        child.parent = parent


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


def find(root, idx):
    v = root

    while v is not None:
        left_size = v.left.size if v.left is not None else 0
        if idx == 1 + left_size:
            break
        elif idx <= left_size:
            v = v.left
        else:
            idx -= left_size
            idx -= 1
            v = v.right

    return splay(v)


def split(root, idx):
    if idx < 1:
        return None, root
    elif idx > root.size:
        return root, None

    right = find(root, idx)
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


def move(root, l, r, to):
    left, middle = split(root, l)
    middle, right = split(middle, r - l + 2)

    if to == 0:
        return merge(merge(middle, left), right)

    left_size = left.size if left is not None else 0
    if to < left_size:
        left1, left2 = split(left, to + 1)
        return merge(merge(merge(left1, middle), left2), right)
    elif to == left_size:
        return merge(merge(left, middle), right)
    else:
        right1, right2 = split(right, to - left_size + 1)
        return merge(merge(merge(left, right1), middle), right2)


def build_tree(root, t, left, right):
    if right < left:
        return None

    middle = (right + left) // 2
    v = Vertex(t[middle], right - left + 1)
    v.parent = root
    v.left = build_tree(v, t, left, middle - 1)
    v.right = build_tree(v, t, middle + 1, right)

    return v


def print_tree(root):
    if root.left is not None:
        print_tree(root.left)

    sys.stdout.write(root.key)

    if root.right is not None:
        print_tree(root.right)


source = sys.stdin
text = source.readline().rstrip()
tree = build_tree(None, text, 0, len(text) - 1)
q = int(source.readline())

for _ in range(q):
    i, j, k = map(int, source.readline().split())
    tree = move(tree, i + 1, j + 1, k)

print_tree(tree)

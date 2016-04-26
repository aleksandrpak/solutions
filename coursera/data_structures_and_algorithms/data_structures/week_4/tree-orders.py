# python3

import sys, threading
sys.setrecursionlimit(10**6) # max depth of recursion
threading.stack_size(2**25)  # new thread will get stack of such size

class TreeOrders:
  def read(self):
    self.n = int(sys.stdin.readline())
    self.key = [0 for i in range(self.n)]
    self.left = [0 for i in range(self.n)]
    self.right = [0 for i in range(self.n)]

    for i in range(self.n):
      [a, b, c] = map(int, sys.stdin.readline().split())
      self.key[i] = a
      self.left[i] = b
      self.right[i] = c

  def inOrder(self):
    self.result = []
    self.inOrderRec(0)
    return self.result

  def inOrderRec(self, root):
    if self.left[root] != -1:
      self.inOrderRec(self.left[root])

    self.result.append(self.key[root])

    if self.right[root] != -1:
      self.inOrderRec(self.right[root])

  def preOrder(self):
    self.result = []
    self.preOrderRec(0)
    return self.result

  def preOrderRec(self, root):
    self.result.append(self.key[root])

    if self.left[root] != -1:
      self.preOrderRec(self.left[root])

    if self.right[root] != -1:
      self.preOrderRec(self.right[root])

  def postOrder(self):
    self.result = []
    self.postOrderRec(0)
    return self.result

  def postOrderRec(self, root):
    if self.left[root] != -1:
      self.postOrderRec(self.left[root])

    if self.right[root] != -1:
      self.postOrderRec(self.right[root])

    self.result.append(self.key[root])

def main():
	tree = TreeOrders()
	tree.read()
	print(" ".join(str(x) for x in tree.inOrder()))
	print(" ".join(str(x) for x in tree.preOrder()))
	print(" ".join(str(x) for x in tree.postOrder()))

threading.Thread(target=main).start()

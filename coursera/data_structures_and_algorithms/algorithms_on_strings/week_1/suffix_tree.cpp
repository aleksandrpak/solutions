#include <iostream>
#include <stdlib.h>
#include <map>
#include <string>
#include <vector>

using std::cin;
using std::string;

int Letters = 5;
int NA = 0;
int tree_size = 0;
int tree_length = 50000;
int* tree = (int*)calloc(tree_length, sizeof(int));
int nodes = tree_length / Letters;
int* sizes = (int*)calloc(nodes, sizeof(int));
char *letters = (char*)malloc(nodes * sizeof(char));
int* nexts = (int*)malloc(nodes * sizeof(int));

void new_node() {
  if ((tree_size + 1) * Letters > tree_length) {
    tree = (int*)realloc(tree, tree_length * 2 * sizeof(int));
    sizes = (int*)realloc(sizes, nodes * 2 * sizeof(int));
    letters = (char*)realloc(letters, nodes * 2 * sizeof(char));
    nexts = (int*)realloc(nexts, nodes * 2 * sizeof(int));

    memset(tree + tree_length, 0, tree_length * sizeof(int));
    memset(sizes + nodes, 0, nodes * sizeof(int));

    tree_length *= 2;
    nodes *= 2;
  }

  tree_size++;
}

int size(int node) {
  return sizes[node - 1];
}

int letterToIndex(const char& letter) {
  switch (letter) {
    case 'A': return 0;
    case 'C': return 1;
    case 'G': return 2;
    case 'T': return 3;
    case '$': return 4;
    default: return -1;
  }
}

char indexToLetter(int index) {
  switch (index) {
    case 0: return 'A';
    case 1: return 'C';
    case 2: return 'G';
    case 3: return 'T';
    case 4: return '$';
    default: return 0;
  }
}

int get_node(int node, int letter) {
  return tree[(node - 1) * Letters + letter];
}

void set_node(int node, int letter, int index) {
  node--;

  sizes[node] += 1;

  if (sizes[node] == 1) {
    letters[node] = indexToLetter(letter);
    nexts[node] = index;
  }

  tree[node * Letters + letter] = index;
}

void build_suffix_tree(const string& text) {
  new_node();
  int len = text.length();

  for (int j = 0; j < len; j++) {
    int node = 1;

    for (int i = j; i < len; i++) {
      int letter = letterToIndex(text[i]);

      if (get_node(node, letter) == NA) {
        new_node();
        set_node(node, letter, tree_size);
        node = tree_size;
      } else {
        node = get_node(node, letter);
      }
    }
  }
}

int get_first_node(int node) {
  return nexts[node - 1];
}

int get_first_letter(int node) {
  return letters[node - 1];
}

void find_leafs(int index, const char& c) {
  int node = index;

  if (size(node) == 1) {
    bool isPrinted = false;
    if (c != 0) {
      putchar(c);
      isPrinted = true;
    }

    while (size(node) == 1) {
      isPrinted = true;
      putchar(get_first_letter(node));
      node = get_first_node(node);
    }

    if (isPrinted) {
      putchar('\n');
    }
  } else if (c != 0) {
    putchar(c);
    putchar('\n');
  }

  for (int i = 0; i < Letters; i++) {
    if (get_node(node, i) != NA) {
      find_leafs(get_node(node, i), indexToLetter(i));
    }
  }
}

int main() {
  string text;
  cin >> text;

  build_suffix_tree(text);
  find_leafs(1, 0);

  return 0;
}

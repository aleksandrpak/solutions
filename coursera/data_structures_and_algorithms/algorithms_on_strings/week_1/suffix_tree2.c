#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <time.h>

#define Letters 5
#define NA -1

struct node {
  int* nodes;
  int size;
  int first_node;
  char first_letter;
};

int letterToIndex(char letter) {
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

void new_node(struct node** tree, int* tree_size, int* tree_length) {
  int size = *tree_size;
  int length = *tree_length;

  if ((size + 1) * Letters > length) {
    *tree = (struct node*)realloc(*tree, length * 2 * sizeof(struct node));
    *tree_length = length * 2;
  }

  struct node new;
  new.size = 0;

  (*tree)[size] = new;
  *tree_size = size + 1;
}

int get_node(struct node* tree, int node, char letter) {
  struct node n = tree[node];

  if (n.size == 0) {
    return NA;
  } else if (n.size == 1) {
    if (n.first_letter == letter) {
      return n.first_node;
    } else {
      return NA;
    }
  } else {
    return n.nodes[letterToIndex(letter)];
  }
}

void set_node(struct node* tree, int node, int letter, int next) {
  struct node n = tree[node];
  if (n.size == 0) {
    n.size = 1;
    n.first_node = next;
    n.first_letter = letter;
  } else if (n.size == 1) {
    n.size = 2;
    n.nodes = (int*)malloc(Letters * sizeof(int));
    int index = letterToIndex(letter);
    int firstIndex = letterToIndex(n.first_letter);
    for (int i = 0; i < Letters; i++) {
      if (i == index) {
        n.nodes[i] = next;
      } else if (i == firstIndex) {
        n.nodes[i] = n.first_node;
      } else {
        n.nodes[i] = -1;
      }
    }
  } else {
    int index = letterToIndex(letter);
    n.nodes[index] = next;
    n.size += 1;
  }

  tree[node] = n;
}

struct node* build_suffix_tree(char* text, int text_length) {
  int tree_size = 0;
  int tree_length = 600000;
  struct node* tree = (struct node*)malloc(tree_length * sizeof(struct node));

  new_node(&tree, &tree_size, &tree_length);

  for (int j = 0; j < text_length; j++) {
    int node = 0;

    for (int i = j; i < text_length; i++) {
      int letter = text[i];

      int next = get_node(tree, node, letter);
      if (next == NA) {
        new_node(&tree, &tree_size, &tree_length);
        set_node(tree, node, letter, tree_size - 1);
        node = tree_size - 1;
      } else {
        node = next;
      }
    }
  }

  return tree;
}

void find_leafs(int index, char c, struct node* tree) {
  struct node node = tree[index];

  if (node.size == 1) {
    int isPrinted = 0;
    if (c != 0) {
      putchar(c);
      isPrinted = 1;
    }

    while (node.size == 1) {
      isPrinted = 1;
      putchar(node.first_letter);
      node = tree[node.first_node];
    }

    if (isPrinted) {
      putchar('\n');
    }
  } else if (c != 0) {
    putchar(c);
    putchar('\n');
  }

  if (node.size == 0) {
    return;
  } else {
    for (int i = 0; i < Letters; i++) {
      if (node.nodes[i] != NA) {
        find_leafs(node.nodes[i], indexToLetter(i), tree);
      }
    }
  }
}

char* get_text(int* size) {
  int length = 5500;
  char* line = malloc(length);
  *size = 0;

  int c;
  while (1) {
    c = getchar();

    if (*size == length) {
      line = realloc(line, length *= 2);
    }

    line[*size] = c;
    *size += 1;

    if (c == '$') {
      break;
    }
  }

  return line;
}

int main() {
  int text_length;
  char* text = get_text(&text_length);

  clock_t t;
  t = clock();
  struct node* tree = build_suffix_tree(text, text_length);
  t = clock() - t;
  double time_taken = ((double)t)/CLOCKS_PER_SEC; // in seconds

  printf("build_suffix_tree() took %f seconds to execute \n", time_taken);
  t = clock();
  find_leafs(0, 0, tree);
  t = clock() - t;
  time_taken = ((double)t)/CLOCKS_PER_SEC; // in seconds

  printf("find_leafs() took %f seconds to execute \n", time_taken);

  return 0;
}

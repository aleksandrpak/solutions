#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#define Letters 5
#define NA -1

char letters[Letters] = "ACGT$";
char indices[90] = {
  0,0,0,0,0,0,0,0,0,0,
  0,0,0,0,0,0,0,0,0,0,
  0,0,0,0,0,0,0,0,0,0,
  0,0,0,0,0,0,4,0,0,0,
  0,0,0,0,0,0,0,0,0,0,
  0,0,0,0,0,0,0,0,0,0,
  0,0,0,0,0,0,0,1,0,0,
  0,2,0,0,0,0,0,0,0,0,
  0,0,0,0,3,0,0,0,0,0};

struct node {
  int* nodes;
  int size;
  int first_node;
  int first_letter;
};

void new_node(struct node** tree, int* tree_size, int* tree_length) {
  int size = *tree_size;
  int length = *tree_length;

  if (size + 1 > length) {
    *tree = (struct node*)realloc(*tree, length * 2 * sizeof(struct node));
    *tree_length = length * 2;
  }

  struct node new;
  new.size = 0;

  (*tree)[size] = new;
  *tree_size = size + 1;
}

int get_node(struct node* tree, int node, int letter) {
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
    return n.nodes[letter];
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
    for (int i = 0; i < Letters; i++) {
      if (i == letter) {
        n.nodes[i] = next;
      } else if (i == n.first_letter) {
        n.nodes[i] = n.first_node;
      } else {
        n.nodes[i] = -1;
      }
    }
  } else {
    n.size += 1;
    n.nodes[letter] = next;
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

void find_leafs(int index, int c, struct node* tree) {
  struct node node = tree[index];

  if (node.size == 1) {
    int isPrinted = 0;
    if (c != -1) {
      putchar(letters[c]);
      isPrinted = 1;
    }

    while (node.size == 1) {
      isPrinted = 1;
      putchar(letters[node.first_letter]);
      node = tree[node.first_node];
    }

    if (isPrinted) {
      putchar('\n');
    }
  } else if (c != -1) {
    putchar(letters[c]);
    putchar('\n');
  }

  if (node.size == 0) {
    return;
  } else {
    for (int i = 0; i < Letters; i++) {
      if (node.nodes[i] != NA) {
        find_leafs(node.nodes[i], i, tree);
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

    line[*size] = indices[c];
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

  struct node* tree = build_suffix_tree(text, text_length);
  find_leafs(0, -1, tree);

  return 0;
}

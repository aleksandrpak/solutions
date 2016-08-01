import java.util.*;
import java.io.*;
import java.util.zip.CheckedInputStream;

public class SuffixTree {
    private static int[] tree = new int[50000];
    private static int treeSize = 0;
    public static final int Letters = 5;
    public static final int NA = 0;

    private static void newNode() {
        if ((treeSize + 1) * Letters > tree.length) {
            tree = Arrays.copyOf(tree, tree.length * 2);
        }

        treeSize++;
    }

    private static int size(int node) {
        int count = 0;
        node = (node - 1) * Letters;

        for (int i = node; i < node + Letters; i++) {
            if (tree[i] != 0) {
                count++;
            }
        }

        return count;
    }

    private static int letterToIndex(char letter) {
        switch (letter) {
            case 'A': return 0;
            case 'C': return 1;
            case 'G': return 2;
            case 'T': return 3;
            case '$': return 4;
            default: return NA;
        }
    }

    private static char indexToLetter(int index) {
        switch (index) {
            case 0: return 'A';
            case 1: return 'C';
            case 2: return 'G';
            case 3: return 'T';
            case 4: return '$';
            default: return 0;
        }
    }

    private static void buildSuffixTree() throws IOException {
        StringBuilder builder = new StringBuilder();
        
        int ch;
        while ((ch = System.in.read()) != -1) {
            builder.append((char)ch);
            if ((char)ch == '$') {
                break;
            }
        }
        
        String text = builder.toString();

        newNode();
        int len = text.length();
        for (int j = 0; j < len; j++) {
            int node = 1;

            for (int i = j; i < len; i++) {
                int letter = letterToIndex(text.charAt(i));

                if (getNode(node, letter) == NA) {
                    newNode();
                    setNode(node, letter, treeSize);
                    node = treeSize;
                } else {
                    node = getNode(node, letter);
                }
            }
        }
    }

    private static void findLeafs(int index, Character c) {
        int node = index;

        if (size(node) == 1) {
            boolean isPrinted = false;
            if (c != null) {
                System.out.print(c);
                isPrinted = true;
            }

            while (size(node) == 1) {
                for (int i = 0; i < Letters; i++) {
                    int next = getNode(node, i);
                    if (next != NA) {
                        System.out.print(indexToLetter(i));
                        isPrinted = true;
                        node = next;
                        break;
                    }
                }
            }

            if (isPrinted) {
                System.out.print("\n");
            }
        } else if (c != null) {
            System.out.println(c);
        }

        for (int i = 0; i < Letters; i++) {
            if (getNode(node, i) != NA) {
                findLeafs(getNode(node, i), indexToLetter(i));
            }
        }
    }

    private static int getNode(int node, int letter) {
        return tree[(node - 1) * Letters + letter];
    }

    private static void setNode(int node, int letter, int index) {
        tree[(node - 1) * Letters + letter] = index;
    }

    static public void main(String[] args) throws IOException {
        buildSuffixTree();
        findLeafs(1, null);
    }
}

import java.io.*;
import java.util.*;

class Node {
	public static final int Letters =  4;
	public static final int NA      = -1;
	public int next[];
	public boolean patternEnd;

	Node() {
		next = new int[Letters];
		Arrays.fill(next, NA);
		patternEnd = false;
	}
}

public class TrieMatchingExtended implements Runnable {
	int letterToIndex(char letter) {
		switch (letter) {
			case 'A': return 0;
			case 'C': return 1;
			case 'G': return 2;
			case 'T': return 3;
			default: assert(false); return Node.NA;
		}
	}

	List<Node> buildTrie(List<String> patterns) {
		List<Node> trie = new ArrayList<Node>();
		trie.add(new Node());

		for (String pattern : patterns) {
			Node node = trie.get(0);

			for (int i = 0; i < pattern.length(); i++) {
				int index = letterToIndex(pattern.charAt(i));

				if (node.next[index] == Node.NA) {
					trie.add(new Node());
					node.next[index] = trie.size() - 1;
					node = trie.get(trie.size() - 1);
				} else {
					node = trie.get(node.next[index]);
				}
			}

			node.patternEnd = true;
		}

		return trie;
	}

	List<Integer> solve(String text, List<String> patterns) {
		List<Integer> result = new ArrayList<Integer>();
		List<Node> trie = buildTrie(patterns);

		for (int i = 0; i < text.length(); i++) {
			Node node = trie.get(0);
			int index = i;

			while (index < text.length()) {
				int nextIndex = letterToIndex(text.charAt(index));

				if (node.next[nextIndex] != Node.NA) {
					node = trie.get(node.next[nextIndex]);
					if (node.patternEnd) {
						result.add(i);
						break;
					}

					index++;
				} else {
					break;
				}
			}
		}

		return result;
	}

	public void run() {
		try {
			BufferedReader in = new BufferedReader(new InputStreamReader(System.in));
			String text = in.readLine();
		 	int n = Integer.parseInt(in.readLine());
		 	List<String> patterns = new ArrayList<String>();
			for (int i = 0; i < n; i++) {
				patterns.add(in.readLine());
			}

			List<Integer> ans = solve(text, patterns);

			for (int j = 0; j < ans.size(); j++) {
				System.out.print("" + ans.get(j));
				System.out.print(j + 1 < ans.size() ? " " : "\n");
			}
		}
		catch(Throwable e) {
			e.printStackTrace();
			System.exit(1);
		}
	}

	public static void main(String [] args) {
		new Thread(new TrieMatchingExtended()).start();
	}
}

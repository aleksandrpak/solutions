import java.io.*;
import java.util.*;

public class TrieMatching implements Runnable {
	List<Map<Character, Integer>> buildTrie(List<String> patterns) {
		List<Map<Character, Integer>> trie = new ArrayList<Map<Character, Integer>>();

		trie.add(new HashMap<Character, Integer>());

		for (String pattern : patterns) {
			Map<Character, Integer> node = trie.get(0);

			for (int i = 0; i < pattern.length(); i++) {
				Character c = pattern.charAt(i);

				if (node.containsKey(c)) {
					node = trie.get(node.get(c));
				} else {
					trie.add(new HashMap<Character, Integer>());
					node.put(c, trie.size() - 1);
					node = trie.get(trie.size() - 1);
				}
			}
		}

		return trie;
	}

	List<Integer> solve(String text,  List<String> patterns) {
		List<Integer> result = new ArrayList<Integer>();
		List<Map<Character, Integer>> trie = buildTrie(patterns);

		for (int i = 0; i < text.length(); i++) {
			Map<Character, Integer> node = trie.get(0);
			int index = i;

			while (index < text.length()) {
				Character c = text.charAt(index);

				if (node.containsKey(c)) {
					node = trie.get(node.get(c));
					if (node.size() == 0) {
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
		new Thread(new TrieMatching()).start();
	}
}

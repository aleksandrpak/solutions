import java.io.BufferedReader;
import java.io.FileReader;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.*;

public class MP1 {
    Random generator;
    String userName;
    String inputFileName;
    String delimiters = " \t,;.?!-:@[](){}_*/";
    String[] stopWordsArray = {"i", "me", "my", "myself", "we", "our", "ours", "ourselves", "you", "your", "yours",
            "yourself", "yourselves", "he", "him", "his", "himself", "she", "her", "hers", "herself", "it", "its",
            "itself", "they", "them", "their", "theirs", "themselves", "what", "which", "who", "whom", "this", "that",
            "these", "those", "am", "is", "are", "was", "were", "be", "been", "being", "have", "has", "had", "having",
            "do", "does", "did", "doing", "a", "an", "the", "and", "but", "if", "or", "because", "as", "until", "while",
            "of", "at", "by", "for", "with", "about", "against", "between", "into", "through", "during", "before",
            "after", "above", "below", "to", "from", "up", "down", "in", "out", "on", "off", "over", "under", "again",
            "further", "then", "once", "here", "there", "when", "where", "why", "how", "all", "any", "both", "each",
            "few", "more", "most", "other", "some", "such", "no", "nor", "not", "only", "own", "same", "so", "than",
            "too", "very", "s", "t", "can", "will", "just", "don", "should", "now"};

    void initialRandomGenerator(String seed) throws NoSuchAlgorithmException {
        MessageDigest messageDigest = MessageDigest.getInstance("SHA");
        messageDigest.update(seed.toLowerCase().trim().getBytes());
        byte[] seedMD5 = messageDigest.digest();

        long longSeed = 0;
        for (int i = 0; i < seedMD5.length; i++) {
            longSeed += ((long) seedMD5[i] & 0xffL) << (8 * i);
        }

        this.generator = new Random(longSeed);
    }

    Integer[] getIndexes() throws NoSuchAlgorithmException {
        Integer n = 10000;
        Integer number_of_lines = 50000;
        Integer[] ret = new Integer[n];
        this.initialRandomGenerator(this.userName);
        for (int i = 0; i < n; i++) {
            ret[i] = generator.nextInt(number_of_lines);
        }
        return ret;
    }

    public MP1(String userName, String inputFileName) {
        this.userName = userName;
        this.inputFileName = inputFileName;
    }

    public String[] process() throws Exception {
        String[] ret = new String[20];

        HashSet<String> stopWordsSet = new HashSet<>(stopWordsArray.length);
        Collections.addAll(stopWordsSet, stopWordsArray);

        Integer[] indexes = getIndexes();
        Arrays.sort(indexes);

        HashMap<String, Integer> wordCount = new HashMap<>();

        int lineIndex = -1;
        int nextIndexPosition = 0;
        int nextIndex = indexes[nextIndexPosition];
        int repeatCount = 1;

        int shift = moveIndexPosition(indexes, nextIndexPosition, nextIndex);
        repeatCount += shift;
        nextIndexPosition += shift;

        try (BufferedReader br = new BufferedReader(new FileReader(inputFileName))) {
            for (String line; (line = br.readLine()) != null; ) {
                lineIndex++;

                if (lineIndex != nextIndex) {
                    continue;
                }

                StringTokenizer tokenizer = new StringTokenizer(line, delimiters);
                while (tokenizer.hasMoreTokens()) {
                    String word = tokenizer.nextToken().trim().toLowerCase();
                    if (stopWordsSet.contains(word)) {
                        continue;
                    }

                    Integer count = 0;
                    if (wordCount.containsKey(word)) {
                        count = wordCount.get(word);
                    }

                    wordCount.put(word, count + repeatCount);
                }

                if (++nextIndexPosition < indexes.length) {
                    nextIndex = indexes[nextIndexPosition];
                    repeatCount = 1;
                } else {
                    break;
                }

                shift = moveIndexPosition(indexes, nextIndexPosition, nextIndex);
                repeatCount += shift;
                nextIndexPosition += shift;
            }
        }

        // I don't like this copying for just sorting
        // Had an idea to keep track of current top 20 words
        // But that way I need to handle different words with same count
        // At the end of the collection with current top 20 words
        // Keep track of all tail elements with same count in different collection
        // Keep first 20 in heap data structure to have knowledge of the minimum
        // And maintain tail only if there are elements with equal count to minimum
        // But all this requires a lot of complication and own class with comparing and equals abilities
        ArrayList<Map.Entry<String, Integer>> entries = new ArrayList<>(wordCount.entrySet());
        Collections.sort(entries, new Comparator<Map.Entry<String, Integer>>() {
            @Override
            public int compare(Map.Entry<String, Integer> x, Map.Entry<String, Integer> y) {
                if (Objects.equals(x.getValue(), y.getValue())) {
                    return y.getKey().compareTo(x.getKey());
                }

                return x.getValue().compareTo(y.getValue());
            }
        });

        int entriesSize = entries.size();
        for (int i = 0; i < ret.length; ++i) {
            ret[i] = entries.get(entriesSize - i - 1).getKey();
        }

        return ret;
    }

    private int moveIndexPosition(Integer[] indexes, int nextIndexPosition, Integer nextIndex) {
        int count = 0;

        while (nextIndexPosition < indexes.length - 1) {
            if (Objects.equals(nextIndex, indexes[nextIndexPosition + 1])) {
                count++;
                nextIndexPosition++;
                continue;
            }

            break;
        }

        return count;
    }

    public static void main(String[] args) throws Exception {
        if (args.length < 1) {
            System.out.println("MP1 <User ID>");
        } else {
            String userName = args[0];
            String inputFileName = "./input.txt";
            MP1 mp = new MP1(userName, inputFileName);
            String[] topItems = mp.process();
            for (String item : topItems) {
                System.out.println(item);
            }
        }
    }
}

#include <iostream>
#include <tuple>
#include <vector>
#include <algorithm>

using std::vector;

vector<int> optimal_sequence(int n) {
  std::vector<std::tuple<int, int>> s(n);
  s[0] = std::make_tuple(1, 0);

  int i = 2;
  while (i < n + 1) {
    int l1 = std::get<0>(s[i - 2]);
    int l2 = 0;
    int l3 = 0;

    if (i % 2 == 0) {
      l2 = std::get<0>(s[(i / 2) - 1]);
    }
    
    if (i % 3 == 0) {
      l3 = std::get<0>(s[(i / 3) - 1]);
    }

    int l = 0;
    int seq = 0;

    if (l2 != 0 && l2 <= l1 && (l3 == 0 || l2 <= l3)) {
      l = l2;
      seq = 2;
    } else if (l3 != 0 && l3 <= l1 && (l2 == 0 || l3 <= l2)) {
      l = l3;
      seq = 3;
    } else {
      l = l1;
      seq = 1;
    }

    s[i - 1] = std::make_tuple(l + 1, seq);
    i++;
  }

  std::vector<int> sequence;
  sequence.push_back(n);

  i = n;
  while (i > 1) {
    int prev = std::get<1>(s[i - 1]);

    if (prev == 1) {
      i = i - 1;
    } else if (prev == 2) {
      i = i / 2;
    } else if (prev == 3) {
      i = i / 3;
    }

    sequence.push_back(i);
  }

  reverse(sequence.begin(), sequence.end());
  return sequence;
}

int main() {
  int n;
  std::cin >> n;
  vector<int> sequence = optimal_sequence(n);
  std::cout << sequence.size() - 1 << std::endl;
  for (size_t i = 0; i < sequence.size(); ++i) {
    std::cout << sequence[i] << " ";
  }
}

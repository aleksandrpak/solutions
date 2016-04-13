# python3
import sys

class Bracket:
    def __init__(self, bracket_type, position):
        self.bracket_type = bracket_type
        self.position = position

    def Match(self, c):
        if self.bracket_type == '[' and c == ']':
            return True
        if self.bracket_type == '{' and c == '}':
            return True
        if self.bracket_type == '(' and c == ')':
            return True
        return False

if __name__ == "__main__":
    text = sys.stdin.read()

    s = []
    result = "Success"

    for i, next in enumerate(text):
        if next == '(' or next == '[' or next == '{':
            s.append(Bracket(next, i))
        elif next == ')' or next == ']' or next == '}':
            if (not s or not s.pop().Match(next)):
                result = i + 1
                break

    if (result == "Success" and len(s) > 0):
        result = s.pop().position + 1

    print(result)

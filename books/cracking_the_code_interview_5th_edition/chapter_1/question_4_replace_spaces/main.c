#include <stdio.h>
#include <string.h>

void replace(char* str) {
	int i;
	int len = strlen(str);
	int spaceCount = 0;

	for (i = 0; i < len; ++i) {
		if (str[i] == ' ') {
			++spaceCount;
		}
	}

	if (spaceCount == 0) {
	    return;
    }

	int newLen = len + spaceCount * 2;
	str[newLen] = 0;

	for (i = 0; i < len; ++i) {
		if (str[len - i - 1] == ' ') {
			str[--newLen - i] = '0';
			str[--newLen - i] = '2';
			str[newLen - i - 1] = '%';
		} else {
			str[newLen - i - 1] = str[len - i - 1];
		}
	}
}

int main() {
	char str[18];
	strcpy(str, "Mr John Smith");

	printf("before replacing: %s\n", str);

	replace(str);

	printf("after replacing: %s", str);	
}

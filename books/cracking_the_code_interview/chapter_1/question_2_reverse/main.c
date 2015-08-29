#include <stdio.h>
#include <string.h>

void reverse(char* str) {
	char tmp;
	int i, len = strlen(str);

	for (i = 0; i < len / 2; ++i) {
		tmp = str[i];
		str[i] = str[len - i - 1];
		str[len - i - 1] = tmp;
	}
}

int main() {
	char str[50];
	strcpy(str, "The C Programming Language");

	reverse(str);

	printf("%s", str);
}
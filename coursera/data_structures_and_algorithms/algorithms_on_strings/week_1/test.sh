#!/bin/bash

# javac -encoding UTF-8 SuffixTree.java
# time cat in.txt | java SuffixTree > 1.txt

g++ -pipe -O2 -std=c++11 suffix_tree.cpp -lm
time cat in.txt | ./a.out > 1.txt

gcc -pipe -O2 -std=c11 suffix_tree.c -lm
time cat in.txt | ./a.out > 2.txt

sort 1.txt > out1.txt
sort 2.txt > out2.txt

diff out1.txt out2.txt 

# rm *.class
rm a.out
rm 1.txt
rm 2.txt
rm out1.txt
rm out2.txt

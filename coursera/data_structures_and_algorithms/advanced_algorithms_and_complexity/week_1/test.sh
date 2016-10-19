#!/bin/sh

set -e

PROBLEM=$1

TESTS=./$PROBLEM/tests/*

for fullpath in $TESTS
do
	if [[ "$fullpath" != *.a ]]
	then
		cat $fullpath | python3 ./$PROBLEM/$PROBLEM.py > ./a

		if !(cmp --silent ./a $fullpath.a)
		then
			echo $fullpath

			echo "input:"
			cat $fullpath

			echo "received:"
			cat ./a

			echo "correct:"
			cat $fullpath.a

			exit 1
		fi
	fi
done

echo "finished"

rm -f ./a

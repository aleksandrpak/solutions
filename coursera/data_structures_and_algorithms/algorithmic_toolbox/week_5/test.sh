for n in {1..1000}
do
	o1=$(echo "$n" | python3 primitive_calculator.py)
	o2=$(echo "$n" | ./a.out)

	if [ "$o1" != "$o2" ]; then
		echo "$o1"
		echo "$o2"
		break
	fi

done

package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
)

func main() {
	file, err := os.Open("in.txt")
	if err != nil {
		log.Fatal(err)
	}

	defer file.Close()

	var nums []int
	scanner := bufio.NewScanner(file)

	for scanner.Scan() {
		num, err := strconv.ParseInt(scanner.Text(), 10, 0)
		if err != nil {
			log.Fatal(err)
		}

		nums = append(nums, int(num))
	}

	z, _ := Count(nums)
	fmt.Println(z)
}

func Count(nums []int) (int, []int) {
	n := len(nums)

	if n == 1 {
		return 0, nums
	}

	x, a := Count(nums[0 : n/2])
	y, b := Count(nums[n/2 : n])

	z, c := Merge(a, b)

	return x + y + z, c
}

func Merge(a, b []int) (int, []int) {
	i, j, z := 0, 0, 0
	var c []int

	for k := 0; k < len(a)+len(b); k++ {
		if j >= len(b) || i < len(a) && a[i] < b[j] {
			c = append(c, a[i])
			i++
		} else {
			c = append(c, b[j])
			j++
			z += len(a) - i
		}
	}

	return z, c
}

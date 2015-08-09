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

	z := QuickSort(nums)
	fmt.Println(z)
}

func QuickSort(nums []int) int {
	if len(nums) <= 1 {
		return 0
	}

	result := len(nums) - 1

	middle := 0
	if len(nums)%2 == 0 {
		middle = (len(nums) / 2) - 1
	} else {
		middle = len(nums) / 2
	}

	a, b, c, pivot := nums[0], nums[result], nums[middle], -1

	if (a <= b && a >= c) || (a >= b && a <= c) {
		pivot = a
	} else if (b <= a && b >= c) || (b >= a && b <= c) {
		nums[result] = a
		nums[0] = b
		pivot = b
	} else {
		nums[middle] = a
		nums[0] = c
		pivot = c
	}

	i, j := 1, 1

	for ; j < len(nums); j++ {
		if nums[j] < pivot {
			tmp := nums[j]
			nums[j] = nums[i]
			nums[i] = tmp
			i++
		}
	}

	nums[0] = nums[i-1]
	nums[i-1] = pivot

	if i-2 > 0 {
		result += QuickSort(nums[0 : i-1])
	}

	if i < len(nums) {
		result += QuickSort(nums[i:len(nums)])
	}

	return result
}

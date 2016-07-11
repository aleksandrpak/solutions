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

	nums := make(map[int64]bool)
	scanner := bufio.NewScanner(file)

	for scanner.Scan() {
		num, err := strconv.ParseInt(scanner.Text(), 10, 0)
		if err != nil {
			log.Fatal(err)
		}

		nums[num] = true
	}

	c := make(chan bool)

	for i := -10000; i <= 10000; i++ {
		go func(t int64) {
			for k := range nums {
				diff := t - k
				if _, found := nums[diff]; found {
					c <- true
					break
				}
			}

			c <- false
		}(int64(i))
	}

	count := 0
	for i := -10000; i <= 10000; i++ {
		if <-c {
			count++
		}
	}

	fmt.Println(count)
}

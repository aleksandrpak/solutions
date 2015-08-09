package main

import (
	"archive/zip"
	"bufio"
	"fmt"
	"log"
	"sort"
	"strconv"
	"strings"
)

func main() {
	r, err := zip.OpenReader("in.txt.zip")
	if err != nil {
		log.Fatal(err)
	}
	defer r.Close()

	file, err := r.File[0].Open()
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	graph := make(map[int][]int)
	reverseGraph := make(map[int][]int)
	explored := make(map[int]bool)
	max := 0

	scanner := bufio.NewScanner(file)
	for scanner.Scan() {
		line := scanner.Text()

		wordsScanner := bufio.NewScanner(strings.NewReader(line))
		wordsScanner.Split(bufio.ScanWords)

		wordsScanner.Scan()
		v, err := strconv.ParseInt(wordsScanner.Text(), 10, 0)
		if err != nil {
			log.Fatal(err)
		}

		wordsScanner.Scan()
		u, err := strconv.ParseInt(wordsScanner.Text(), 10, 0)
		if err != nil {
			log.Fatal(err)
		}

		edges := graph[int(v)]
		edges = append(edges, int(u))
		graph[int(v)] = edges

		reverseEdges := reverseGraph[int(u)]
		reverseEdges = append(reverseEdges, int(v))
		reverseGraph[int(u)] = reverseEdges

		explored[int(v)] = false
		explored[int(u)] = false

		if int(v) > max {
			max = int(v)
		}

		if int(u) > max {
			max = int(u)
		}
	}

	fmt.Println(findScc(max, graph, reverseGraph, explored))
}

func markTime(reverseGraph map[int][]int, start int, f map[int]int, explored map[int]bool) {
	if explored[start] {
		return
	}

	explored[start] = true

	for _, edge := range reverseGraph[start] {
		if explored[edge] {
			continue
		}

		markTime(reverseGraph, edge, f, explored)
	}

	f[len(f)+1] = start
}

func find(graph map[int][]int, start int, explored map[int]bool, length int) int {
	if explored[start] {
		return length - 1
	}

	explored[start] = true

	if _, ok := graph[start]; ok {
		for _, edge := range graph[start] {
			if explored[edge] {
				continue
			}

			innerLength := find(graph, edge, explored, length+1)
			if innerLength > length {
				length = innerLength
			}
		}
	}

	return length
}

func findScc(max int, graph, reverseGraph map[int][]int, explored map[int]bool) []int {
	f := make(map[int]int)

	for i := max; i >= 1; i-- {
		markTime(reverseGraph, i, f, explored)
	}

	for i := 1; i <= max; i++ {
		explored[i] = false
	}

	var scc []int
	for i := max; i >= 1; i-- {
		var length = find(graph, f[i], explored, 1)
		if length >= 1 {
			scc = append(scc, length)
		}
	}

	sort.Ints(scc)
	if len(scc) > 5 {
		return scc[len(scc)-5:]
	}

	return scc
}

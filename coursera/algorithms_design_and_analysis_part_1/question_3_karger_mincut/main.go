package main

import (
	"bufio"
	"fmt"
	"log"
	"math/rand"
	"os"
	"strconv"
	"strings"
	"time"
)

type Edge struct {
	src, dest int
}

type Graph struct {
	v, e  int
	edges []Edge
}

type Subset struct {
	parent, rank int
}

func kargerMinCut(graph Graph) int {
	v := graph.v
	e := graph.e
	edges := graph.edges

	subsets := make([]Subset, v)

	for i := 0; i < v; i++ {
		subsets[i].parent = i + 1
		subsets[i].rank = 0
	}

	vertices := v

	for vertices > 2 {
		i := rand.Intn(len(edges))

		subset1 := find(subsets, edges[i].src)
		subset2 := find(subsets, edges[i].dest)

		if subset1 == subset2 {
			continue
		} else {
			vertices--
			Union(subsets, subset1, subset2)
		}
	}

	cutedges := 0
	for i := 0; i < e; i++ {
		subset1 := find(subsets, edges[i].src)
		subset2 := find(subsets, edges[i].dest)
		if subset1 != subset2 {
			cutedges++
		}
	}

	return cutedges
}

func find(subsets []Subset, i int) int {
	if subsets[i-1].parent != i {
		subsets[i-1].parent = find(subsets, subsets[i-1].parent)
	}

	return subsets[i-1].parent
}

func Union(subsets []Subset, x, y int) {
	xroot := find(subsets, x)
	yroot := find(subsets, y)

	if subsets[xroot-1].rank < subsets[yroot-1].rank {
		subsets[xroot-1].parent = yroot
	} else if subsets[xroot-1].rank > subsets[yroot-1].rank {
		subsets[yroot-1].parent = xroot
	} else {
		subsets[yroot-1].parent = xroot
		subsets[xroot-1].rank++
	}
}

func main() {
	file, err := os.Open("in.txt")
	if err != nil {
		log.Fatal(err)
	}

	defer file.Close()

	var edges []Edge
	curr := int64(1)

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

		if v != curr {
			if v > curr {
				edges = append(edges, Edge{int(curr), int(v)})
			}

			v = curr
		} else {
			curr++
		}

		for wordsScanner.Scan() {
			c, err := strconv.ParseInt(wordsScanner.Text(), 10, 0)
			if err != nil {
				log.Fatal(err)
			}

			if c > v {
				edges = append(edges, Edge{int(v), int(c)})
			}
		}
	}

	graph := Graph{v: int(curr) - 1, e: len(edges), edges: edges}
	min := graph.e

	for i := 0; i < 10000; i++ {
		rand.Seed(int64(time.Now().Nanosecond()))
		r := kargerMinCut(graph)
		if r < min {
			min = r
		}
	}

	fmt.Println(min)
}

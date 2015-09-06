package main

import "fmt"

func addPath(path *map[uint64]float64, price uint64, rank float64) {
	if oldRank, ok := (*path)[price]; ok && oldRank > rank {
		rank = oldRank
	}

	(*path)[price] = rank
}

func main() {
	var c, b uint64
	fmt.Scanf("%v %v", &c, &b)

	path1 := make(map[uint64]float64)
	path2 := make(map[uint64]float64)

	var maxRank float64
	var totalMin uint64

	for i := uint64(0); i < c; i++ {
		var n int
		fmt.Scanf("%v", &n)

		var path, newPath map[uint64]float64
		if i%2 == 0 {
			path = path1
			newPath = path2
		} else {
			path = path2
			newPath = path1
		}

		localMin := ^uint64(0)
		for j := 0; j < n; j++ {
			var price uint64
			var rank float64

			fmt.Scanf("%v %v", &price, &rank)

			if price < localMin {
				localMin = price
			}

			if price+totalMin > b {
				continue
			}

			if i == 0 {
				addPath(&newPath, price, rank)
				if rank > maxRank {
					maxRank = rank
				}
			} else {
				for p, r := range path {
					currPrice := p + price
					if currPrice > b {
						continue
					}

					currRank := r + rank
					if i < c-1 {
						addPath(&newPath, currPrice, currRank)
					}

					if currRank > maxRank {
						maxRank = currRank
					}
				}
			}
		}

		if i < c-1 {
			path = make(map[uint64]float64)
		}

		if localMin < ^uint64(0) {
			totalMin += localMin
		}
	}

	if totalMin > b {
		fmt.Println("-1")
	} else {
		fmt.Printf("%.2f\n", maxRank)
	}
}

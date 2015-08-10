use std::io::prelude::*;
use std::io::BufReader;
use std::fs::File;
use std::cmp::Ordering;
use std::collections::BinaryHeap;

#[derive(Eq, PartialEq,)]
pub struct ReverseInt {
	v: i64
}

impl Ord for ReverseInt {
	fn cmp(&self, other: &Self) -> Ordering {
		if self.v == other.v {
			Ordering::Equal
		} else if self.v > other.v {
			Ordering::Less
		} else {
			Ordering::Greater
		}
	}
}

impl PartialOrd for ReverseInt {
	fn partial_cmp(&self, other: &Self) -> Option<Ordering> {
		if self.v == other.v {
			Some(Ordering::Equal)
		} else if self.v > other.v {
			Some(Ordering::Less)
		} else {
			Some(Ordering::Greater)
		}
	}

    fn lt(&self, other: &Self) -> bool { 
    	self.v > other.v
    }

    fn le(&self, other: &Self) -> bool { 
    	self.v >= other.v
    }

    fn gt(&self, other: &Self) -> bool {
    	self.v < other.v
    }

    fn ge(&self, other: &Self) -> bool { 
    	self.v <= other.v
    }
}

fn main() {
    let file = File::open("in.txt").unwrap();
    let reader = BufReader::new(file);
    let mut max_heap = BinaryHeap::new();
    let mut min_heap = BinaryHeap::new();

    let mut lines = reader.lines();
    let first = lines.next().unwrap().unwrap().trim().parse::<i64>().unwrap();
    let second = lines.next().unwrap().unwrap().trim().parse::<i64>().unwrap();

    let mut sum = first + push_initial(first, second, &mut max_heap, &mut min_heap);    

    for line in lines {
    	let num = line.unwrap().trim().parse::<i64>().unwrap();
    	
    	if *max_heap.peek().unwrap() > num {
    		max_heap.push(num);
    	} else {
    		min_heap.push(ReverseInt { v: num });
    	}

    	if max_heap.len() > min_heap.len() + 1 {
    		min_heap.push(ReverseInt { v: max_heap.pop().unwrap() });
    	} else if min_heap.len() > max_heap.len() + 1 {
    		max_heap.push(min_heap.pop().unwrap().v);
    	}

    	let mut m;
    	if max_heap.len() < min_heap.len() {
    		m = min_heap.peek().unwrap().v;
    	} else {    		
    		m = *max_heap.peek().unwrap();
    	}

    	sum += m;
    }

    println!("{:?}", sum % 10000);
}

fn push_initial(first: i64, second: i64, max_heap: &mut BinaryHeap<i64>, min_heap: &mut BinaryHeap<ReverseInt>) -> i64 {
	if first < second {
		max_heap.push(first);
    	min_heap.push(ReverseInt { v: second });
    	first
    } else {
    	max_heap.push(second);
    	min_heap.push(ReverseInt { v: first });
    	second
    }
}
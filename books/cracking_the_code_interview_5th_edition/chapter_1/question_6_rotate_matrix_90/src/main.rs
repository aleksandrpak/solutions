extern crate rand;

use rand::distributions::{IndependentSample, Range};

fn main() {
	let mut matrix = generate_matrix(4);

    println!("before rotation:");
    print_matrix(&matrix);

    rotate_90(&mut matrix);

    println!("\nafter rotation:");
    print_matrix(&matrix);
}

fn rotate_90(m: &mut Vec<Vec<i32>>) {
	let n = m.len();

	for i in 0..(n/2) {
		for j in (0+i)..(n-i-1) {
			let mut elem = m[i][j];
			elem = replace_elem(m, j, n - i - 1, elem);
			elem = replace_elem(m, n - i - 1, n - j - 1, elem);
			elem = replace_elem(m, n - j - 1, i, elem);
			m[i][j] = elem;
		}
	}
}

fn replace_elem(m: &mut Vec<Vec<i32>>, i: usize, j: usize, elem: i32) -> i32 {
	let tmp = m[i][j];
	m[i][j] = elem;
	tmp
}

fn print_matrix(m: &Vec<Vec<i32>>) {
	for row in m {
		println!("{:?}", row);
	}
}

fn generate_matrix(n: usize) -> Vec<Vec<i32>> {
	let mut m = Vec::with_capacity(n);
	let between = Range::new(1, 10);
    let mut rng = rand::thread_rng();

	for _ in 0..n {
		let mut row = Vec::with_capacity(n);
		for _ in 0..n {
			row.push(between.ind_sample(&mut rng));
		}

		m.push(row);
	}

	m
}
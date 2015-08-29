extern crate rand;

use std::fmt::Debug;
use rand::{Rand, Rng};

fn main() {
	let original = generate_matrix::<u16>(7);
	let mut rotated = original.clone();

    rotate_90(&mut rotated);

    println!("is rotated: {:?}", is_rotated(&original, &rotated));
    println!("before rotation:");
    print_matrix(&original);

    println!("\nafter rotation:");
    print_matrix(&rotated);
}

fn rotate_90<T: Copy>(m: &mut Vec<Vec<T>>) {
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

fn replace_elem<T: Copy>(m: &mut Vec<Vec<T>>, i: usize, j: usize, elem: T) -> T {
	let tmp = m[i][j];
	m[i][j] = elem;
	tmp
}

fn is_rotated<T: Eq>(original: &Vec<Vec<T>>, rotated: &Vec<Vec<T>>) -> bool {
	let n = original.len();
	for i in 0..original.len() {
		for j in 0..original.len() {
			if original[i][j] != rotated[j][n - i - 1] {
				return false
			}
		}
	}

	return true
}

fn print_matrix<T: Debug>(m: &Vec<Vec<T>>) {
	for row in m {
		println!("{:?}", row);
	}
}

fn generate_matrix<T: Rand>(n: usize) -> Vec<Vec<T>> {
	let mut m = Vec::with_capacity(n);
	let mut rng = rand::thread_rng();

	for _ in 0..n {
		let mut row = Vec::with_capacity(n);
		for _ in 0..n {
			row.push(rng.gen());
		}

		m.push(row);
	}

	m
}
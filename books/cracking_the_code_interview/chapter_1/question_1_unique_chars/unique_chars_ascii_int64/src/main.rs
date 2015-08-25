use std::fs::File;
use std::io::prelude::*;

fn main() {
	let mut file = File::open("in.txt").unwrap();
	let mut text = String::new();

	file.read_to_string(&mut text).unwrap();

	for line in text.lines() {
		println!("{:?}: {:?}", line, is_unique(line));
	}
}

fn is_unique(text: &str) -> (bool, Option<char>) {
	let mut first: u64 = 0;
	let mut second: u64 = 0;
	let mut third: u64 = 0;
	let mut fourth: u64 = 0;

	for ch in text.chars() {
		let n_pos = ch as u32 / 64;
		let n = match n_pos {
			0 => &mut first,
			1 => &mut second,
			2 => &mut third,
			3 => &mut fourth,
			_ => panic!("invalid character: {:?} ({:?})", ch, ch as u32),
		};

		if !update_number(n, ch as u32 - n_pos * 64) {
			return (false, Some(ch))
		}
	}

	(true, None)
}

fn update_number(n: &mut u64, pos: u32) -> bool {
	let mask: u64 = 1 << pos;
	if mask & *n != 0 {
		return false
	}

	*n = *n | mask;
	true
}
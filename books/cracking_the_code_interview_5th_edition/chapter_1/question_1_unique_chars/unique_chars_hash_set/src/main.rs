use std::fs::File;
use std::io::prelude::*;
use std::collections::HashSet;

fn main() {
	let mut file = File::open("in.txt").unwrap();
	let mut text = String::new();

	file.read_to_string(&mut text).unwrap();

	for line in text.lines() {
		println!("{:?}: {:?}", line, is_unique(line));
	}
}

fn is_unique(text: &str) -> (bool, Option<char>) {
	let mut chars = HashSet::new();

	for ch in text.chars() {
		if chars.contains(&ch) {
			return (false, Some(ch))
		}

		chars.insert(ch);
	}

	(true, None)
}

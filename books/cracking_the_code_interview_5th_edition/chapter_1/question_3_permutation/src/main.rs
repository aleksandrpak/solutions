use std::collections::HashMap;

fn main() {
	let str1 = String::from("hello, world!");
	let str2 = String::from("werlo, dholl!");

    println!("{} : {} = {:?}", str1, str2, is_permutation(&str1, &str2));
}

fn is_permutation(str1: &String, str2: &String) -> bool {
	if str1.len() != str2.len() {
		return false
	}

	let mut char_count = HashMap::new();

	for ch in str1.chars() {
		update_count(&mut char_count, ch, 1);
	}

	for ch in str2.chars() {
		update_count(&mut char_count, ch, -1);
	}

	char_count.is_empty()
}

fn update_count(char_count: &mut HashMap<char, i32>, ch: char, inc: i32) {
	let mut count = match char_count.get(&ch) {
		Some(count) => *count,
		None => 0
	};

	count += inc;
	if count == 0 {
		char_count.remove(&ch);
	} else {
		char_count.insert(ch, count);
	}
}

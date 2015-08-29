fn main() {
    let a = "aabcccccaaa";

    println!("original: {:?}, compressed: {:?}", a, compress(a));
}

fn compress(a: &str) -> String {
	let mut c = String::new();
	let mut curr = None;
	let mut count = 0;

	for ch in a.chars() {
		match curr {
			Some(curr_ch) => {
				if curr_ch != ch {
					c.push(curr_ch);
					c.push_str(count.to_string().as_ref());
					curr = Some(ch);
					count = 1;
				} else {
					count += 1;
				}
			},
			None => {
				curr = Some(ch);
				count = 1;
			}
		}
	}

	if curr.is_some() {
		c.push(curr.unwrap());
		c.push_str(count.to_string().as_ref());
	}

	if a.len() > c.len() {
		c
	} else {
		String::from(a)
	}
}

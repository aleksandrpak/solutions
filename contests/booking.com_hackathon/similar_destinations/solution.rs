use std::io;
use std::io::Read;
use std::cmp::Ordering;
use std::collections::{HashMap, HashSet};

fn main() {
    let mut input = String::new();
    io::stdin().read_to_string(&mut input).unwrap();
    
    let mut n_tmp = None;
    let mut destinations = vec![];
    let mut tags_map = HashMap::new();
    
    for line in input.lines() {
        if n_tmp.is_none() {
            n_tmp = Some(line.parse().unwrap());
            continue;
        }
        
        let mut city = None;
        for word in line.split(':') {
            if city.is_none() {
                city = Some(word);
                continue;
            }
            
            let mut tags = vec![];
            for tag in word.split(',') {
                tags.push(tag);
                
                if !tags_map.contains_key(&tag) {
                    tags_map.insert(tag, HashSet::new());
                }
                
                let mut cities = tags_map.get_mut(&tag).unwrap();
                cities.insert(city.unwrap());
            }
            
            tags.sort();
            destinations.push(tags);
        }           
    }
    
    let n = n_tmp.unwrap();
    let mut groups = HashMap::new();    
    let mut cities = vec![];
    let mut tags = vec![];
    
    let size = destinations.len();
    for i in 0..(size - 1) {
        let t1 = &destinations[i];
        if t1.len() < n {
            continue;
        }
        
        for j in (i + 1)..size {
            let t2 = &destinations[j];
            if t2.len() < n {
                continue;
            }
            
            let mut i1 = 0;
            let mut i2 = 0;
            cities.clear();
            tags.clear();            
            
            loop {
                if i1 == t1.len() || i2 == t2.len() {
                    break;
                }
                
                let tag = t2[i2];
                match t1[i1].cmp(&tag) {
                    Ordering::Less => i1 += 1,
                    Ordering::Greater => i2 += 1,
                    Ordering::Equal => {
                        tags.push(t1[i1]);
                        i1 += 1;
                        i2 += 1;
                        
                        let match_cities = tags_map.get(&tag).unwrap();
                        if tags.len() == 1 {
                            for city in match_cities {
                                cities.push(city);
                            }
                        } else {
                            let mut removed = 0;
                            for k in 0..cities.len() {
                                let index = k + removed;
                                if !match_cities.contains(cities[index]) {
                                    cities.remove(index);
                                    removed -= 1;
                                }
                            }
                        }
                    }
                }
            }
            
            if tags.len() < n {
                continue;
            }
            
            let mut tags_key = String::new();
            for tag in &tags {
                tags_key.push_str(tag);
                tags_key.push(',');
            }
            
            tags_key.pop();
            
            if groups.contains_key(&tags_key) {
                continue;
            }
            
            cities.sort();
            
            let mut cities_val = String::new();
            for city in &cities {
                cities_val.push_str(city);
                cities_val.push(',');                
            }
            
            cities_val.pop();
            
            groups.insert(tags_key, (tags.len(), cities_val));
        }
    }    
    
    let mut output = Vec::with_capacity(groups.len());
    for (tags_key, (len, cities_val)) in groups {
        output.push((tags_key, (len, cities_val)));
    }    
    
    output.sort_by(|a, b| {
        let &(ref t1, (l1, ref c1)) = a;
        let &(ref t2, (l2, ref c2)) = b;
        
        match l2.cmp(&l1) {
            Ordering::Less => Ordering::Less,
            Ordering::Greater => Ordering::Greater,
            Ordering::Equal => {
                match c1.cmp(&c2) {
                    Ordering::Less => Ordering::Less,
                    Ordering::Greater => Ordering::Greater,
                    Ordering::Equal => {
                        t1.cmp(&t2)
                    }
                }
            }
        }
    });
    
    for (t1, (_, c1)) in output {
        println!("{}:{}", c1, t1);
    }
}
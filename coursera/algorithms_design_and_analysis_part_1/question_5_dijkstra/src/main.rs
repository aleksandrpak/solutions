use std::io::prelude::*;
use std::fs::File;
use std::collections::HashMap;

fn main() {
    let mut file = File::open("in.txt").unwrap();
    let mut text = String::new();

    file.read_to_string(&mut text).unwrap();

    let mut graph = HashMap::<i32, Vec<(i32, i32)>>::new();

    for line in text.split("\r\n") {
        if line.len() == 0 {
            continue;
        }

        let mut vertex = -1;
        for tuple in line.split("\t") {
            if vertex == -1 {
                vertex = tuple.parse().unwrap();
                graph.insert(vertex, Vec::new());
            } else {
                if tuple.len() <= 1 {
                    continue;
                }

                let mut values = tuple.split(",");
                let to_vertex = values.next().unwrap().parse().unwrap();
                let distance = values.next().unwrap().parse().unwrap();

                graph.get_mut(&vertex).unwrap().push((to_vertex, distance));
            }
        }
    }

    let mut scores = HashMap::new();

    scores.insert(1, 0);

    calc(&graph, &mut scores);

    println!("{:?}", scores.get(&7));
    println!("{:?}", scores.get(&37));
    println!("{:?}", scores.get(&59));
    println!("{:?}", scores.get(&82));
    println!("{:?}", scores.get(&99));
    println!("{:?}", scores.get(&115));
    println!("{:?}", scores.get(&133));
    println!("{:?}", scores.get(&165));
    println!("{:?}", scores.get(&188));
    println!("{:?}", scores.get(&197));
}

fn calc(graph: &HashMap<i32, Vec<(i32, i32)>>, scores: &mut HashMap<i32, i32>) {

    loop {
        if scores.len() == graph.len() {
            break;
        }

        let mut min_distance = 10000000;
        let mut min_vertex = -1;

        for x in scores.keys() {
            let x_distance = scores.get(&x).unwrap();

            for edge in graph.get(&x).unwrap() {
                let &(v, d) = edge;

                if scores.contains_key(&v) {
                    continue;
                }

                let new_distance = x_distance + d;
                if new_distance < min_distance {
                    min_distance = new_distance;
                    min_vertex = v;
                }
            }
        }

        scores.insert(min_vertex, min_distance);
    }
}

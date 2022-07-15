use std::fs;
use std::collections::HashSet;
use std::collections::HashMap;

fn get_count(ppl: &Vec<String>) -> usize {
    let mut hashset = HashSet::new();

    for p in ppl.iter() {
        for c in p.chars() {
            hashset.insert(c);
        }
    }

    hashset.len()
}

fn get_count2(ppl: &Vec<String>) -> usize {
    let mut hashmap = HashMap::new();

    for p in ppl.iter() {
        for c in p.chars() {
            let mb_v = hashmap.get(&c.clone());
            if let Some(value) = mb_v {
                hashmap.insert(c, value+1);
            } else {
                hashmap.insert(c, 1);
            }
        }
    }

    hashmap.iter().filter(|(_, &v)| v == ppl.len()).count()
}

fn main() {
    let lines: Vec<String> =
        fs::read_to_string("./data.txt").unwrap()
        .split("\n").map(|v| v.to_owned()).collect();
    
    let groups: Vec<Vec<String>> =
        lines.split(|v| v == "").map(|v| v.to_vec()).filter(|v| v.len() > 0).collect();

    let counts: usize = groups.iter().map(get_count).sum();
    println!("{:#?}", counts);
    
    let counts2: usize = groups.iter().map(get_count2).sum();
    println!("{:#?}", counts2);
}

use std::collections::{HashMap, HashSet};

fn get_formatted(lines: &Vec<String>) -> (HashMap<String, HashSet<String>>, HashSet<String>, HashMap::<String, i32>) {
    lines.iter().fold((HashMap::<String, HashSet<String>>::new(), HashSet::<String>::new(), HashMap::<String, i32>::new()), |(hm, all_ings, ing_counts), l| {
        let ingredients = l.split('(').nth(0).unwrap().split(' ').map(|s| s.to_string()).filter(|s| s.len() > 0).collect::<HashSet<String>>();
        let allergens = l.split("contains").nth(1).unwrap().split(',').map(|s| s.chars().filter(|c| c.is_alphabetic()).collect()).collect::<Vec<String>>();
        let mut new_hm = hm.clone();
        for a in allergens.iter() { 
            if let Some(current_ingrs) = hm.get(a) {
                new_hm.insert(a.clone(), current_ingrs.intersection(&ingredients).map(|s| s.to_owned()).collect());
            } else {
                new_hm.insert(a.clone(), ingredients.clone());
            }
        }
        let mut new_ing_counts = ing_counts.clone();
        for i in ingredients.iter() {
            if let Some(count) = new_ing_counts.get(i) {
                new_ing_counts.insert(i.clone(), count + 1);
            } else {
                new_ing_counts.insert(i.clone(), 1);
            }
        }
        (new_hm, all_ings.union(&ingredients).map(|s| s.to_owned()).collect(), new_ing_counts)
    })
}

fn eliminate_with_singles(hm: &HashMap<String, HashSet<String>>) -> HashMap<String, HashSet<String>> {
    let mut new_hm = hm.clone();
    let mut used = HashSet::<String>::new();

    loop {
        let mut stop = true;
        for (k, ings) in new_hm.clone().iter() {
            if !used.contains(k) && ings.len() == 1 {
                let ing = ings.iter().next().unwrap();
                for (k2, ings2) in new_hm.iter_mut() {
                    if k != k2 {
                        used.insert(k.clone());
                        stop = false;
                        ings2.remove(ing);
                    }
                }
            }
        }
        if stop {
            break;
        }
    }
    new_hm
}

fn main() {
    let lines: Vec<String> = std::fs::read_to_string("./data.txt").unwrap().lines().map(|v| v.to_string()).collect();
    let (hm, ings, counts) = get_formatted(&lines);
    
    let new_hm = eliminate_with_singles(&hm);
    let mut non_allergents = ings.clone();
    for (_, ings) in new_hm.iter() {
        for ing in ings.iter() {
            non_allergents.remove(ing);
        }
    }
    let result1: i32 = non_allergents.iter().map(|na| counts.get(na).unwrap()).sum();
    println!("result1: {:#?}", result1);

    println!("new_hm: {:#?}", new_hm);
    let mut result2 = new_hm.iter().map(|(_, s)| s.iter().next().unwrap()).map(|v| v.to_owned()).collect::<Vec<_>>();
    result2.sort();
    println!("resul2: {:#?}", result2);
}

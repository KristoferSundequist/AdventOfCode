use std::collections::{HashMap, HashSet};
use std::fs;

fn get_sub_bags(str: &String) -> HashMap<String, usize> {
    let after_contain = str.split(" ").collect::<Vec<&str>>()[4..].join(" ");

    let mut children = HashMap::new();

    if after_contain == "no other bags.".to_string() {
        return children;
    }

    after_contain
        .split(",")
        .map(|sub| sub.trim())
        .for_each(|sub| {
            let subthing = sub.split(" ").collect::<Vec<&str>>();
            let number = subthing[0].parse::<usize>().unwrap();
            let bag = subthing[1..3].join(" ").to_string();
            children.insert(bag, number);
        });

    children
}

fn part_one(
    name_of_bag: &String,
    name_to_number: &HashMap<String, usize>,
    number_to_name: &Vec<String>,
    matrix: &Vec<Vec<usize>>,
) -> usize {
    let mut visited = HashSet::new();
    let mut to_visit = HashSet::new();
    to_visit.insert(name_to_number[name_of_bag]);

    while to_visit.len() > 0 {
        let v = to_visit.iter().next().unwrap().to_owned();
        for i in 0..number_to_name.len() {
            if matrix[i][v] > 0 && !visited.contains(&i) {
                to_visit.insert(i);
            }
        }
        to_visit.remove(&v);
        visited.insert(v);
    }
    visited.len() - 1
}

fn part_two(
    name_of_bag: &String,
    name_to_number: &HashMap<String, usize>,
    number_to_name: &Vec<String>,
    matrix: &Vec<Vec<usize>>,
) -> usize {
    let bag = name_to_number.get(name_of_bag).unwrap().to_owned();
    (0..number_to_name.len()).filter(|&i| matrix[bag][i] > 0).map(|i| matrix[bag][i] + matrix[bag][i]*part_two(&number_to_name[i], name_to_number, number_to_name, matrix)).sum()
}

fn main() {
    let lines = fs::read_to_string("./data.txt")
        .unwrap()
        .split("\n")
        .map(|v| v.to_string())
        .filter(|l| l.len() > 0)
        .collect::<Vec<String>>();

    let mut name_to_number: HashMap<String, usize> = HashMap::new();
    let mut number_to_name: Vec<String> = Vec::new();
    for (i, l) in lines.iter().enumerate() {
        let words = l.split(" ").collect::<Vec<&str>>();
        let key = words[0..2].join(" ").to_string();
        name_to_number.insert(key.clone(), i);
        number_to_name.push(key);
    }

    let matrix: Vec<Vec<usize>> = (0..number_to_name.len())
        .map(|i| {
            let sub_bags = get_sub_bags(&lines[i]);
            (0..number_to_name.len())
                .map(|j| {
                    if let Some(n) = sub_bags.get(&number_to_name[j]) {
                        n.to_owned()
                    } else {
                        0
                    }
                })
                .collect()
        })
        .collect();

    let count = part_one(
        &"shiny gold".to_string(),
        &name_to_number,
        &number_to_name,
        &matrix,
    );
    println!("{:#?}", count);

    let count2 = part_two(
        &"shiny gold".to_string(),
        &name_to_number,
        &number_to_name,
        &matrix,
    );
    println!("{:#?}", count2);
}

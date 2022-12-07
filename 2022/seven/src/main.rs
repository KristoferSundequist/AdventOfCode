mod parse;
use parse::{parse_line, Line};

use std::collections::HashMap;

fn main() {
    let input = std::fs::read_to_string("./input.txt").unwrap();
    let lines = input.lines().collect::<Vec<_>>();
    let parsed_lines = lines
        .iter()
        .map(|line| parse_line(line))
        .collect::<Vec<_>>();

    let mut dir_sizes: HashMap<String, i64> = HashMap::new();
    let mut current_location: Vec<String> = vec![];
    for line in parsed_lines.iter() {
        if let Line::Cd(path) = line {
            if path == ".." {
                current_location.pop();
            } else if path == "/" {
                current_location = vec![];
            } else {
                current_location.push(path.clone());
            }
        } else if let Line::Ls = line {
            // do nothing
        } else if let Line::Dir(_) = line {
            // do nothing
        } else if let Line::File(file_size, _file_name) = line {
            let mut jointdir = String::from("");

            if let Some(old_size) = dir_sizes.get(&"/".to_string()) {
                dir_sizes.insert("/".to_string(), old_size + file_size);
            } else {
                dir_sizes.insert("/".to_string(), file_size.clone());
            }

            for dir in current_location.iter() {
                jointdir += &(dir.clone() + "/");

                if let Some(old_size) = dir_sizes.get(&jointdir) {
                    dir_sizes.insert(jointdir.clone(), old_size + file_size);
                } else {
                    dir_sizes.insert(jointdir.clone(), file_size.clone());
                }
            }
        }
    }
    println!(
        "Part 1: {:#?}",
        dir_sizes
            .iter()
            .filter(|dir| dir.1 <= &100000)
            .map(|dir| dir.1)
            .sum::<i64>()
    );

    let total: i64 = 70000000;
    let unused_needed = 30000000;
    let total_used = dir_sizes.get(&"/".to_string()).unwrap();
    let need_to_remove = unused_needed - (total - total_used);

    println!(
        "Part 2: {:#?}",
        dir_sizes
            .iter()
            .filter(|dir| dir.1 >= &need_to_remove)
            .map(|dir| dir.1)
            .min()
            .unwrap()
    );
}

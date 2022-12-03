use std::{collections::HashMap, collections::HashSet, io::BufRead};

fn main() {
    let lines: Vec<String> = std::fs::read("./input.txt")
        .unwrap()
        .lines()
        .map(|line| String::from(line.unwrap()))
        .collect();

    part_one(&lines);
    part_two(&lines);
}

fn part_one(lines: &Vec<String>) {
    let mut sum: i64 = 0;
    for line in lines.iter() {
        let half_position = line.len() / 2;

        let mut char_tally: HashMap<char, i32> = HashMap::new();
        let first_part = line[..half_position].to_string();
        for c in first_part.chars() {
            if char_tally.contains_key(&c) {
                let old_value = char_tally.get(&c).unwrap();
                char_tally.insert(c, old_value + 1);
            } else {
                char_tally.insert(c, 1);
            }
        }

        let second_part = line[half_position..].to_string();

        for c in second_part.chars() {
            if char_tally.contains_key(&c) {
                sum += get_char_score(&c);
                break;
            }
        }
    }
    println!("Part 1: {}", sum);
}

fn part_two(lines: &Vec<String>) {
    let mut sum: i64 = 0;
    for (i, chunk) in lines.chunks(3).enumerate() {
        let chars: Vec<HashSet<char>> = chunk
            .iter()
            .map(|line| line.chars().collect::<HashSet<_>>())
            .collect();
        let intersection: HashSet<char> = chars[0]
            .clone()
            .into_iter()
            .filter(|c| chars[1].contains(&c) && chars[2].contains(&c))
            .collect();
        if intersection.len() != 1 {
            panic!("Expected chars to only have a single char but it had {} at chunk {}. Intersection: {:#?}", intersection.len(), i, intersection);
        }
        sum += get_char_score(intersection.iter().nth(0).unwrap());
    }
    println!("Part 2: {}", sum);
}

fn get_char_score(char: &char) -> i64 {
    let ascii = *char as i64;
    if char.is_lowercase() {
        return ascii - 96;
    } else {
        return ascii - 64 + 26;
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_char_score() {
        assert_eq!(get_char_score(&'a'), 1, "a");
        assert_eq!(get_char_score(&'b'), 2, "b");
        assert_eq!(get_char_score(&'z'), 26, "z");
        assert_eq!(get_char_score(&'A'), 27, "A");
        assert_eq!(get_char_score(&'B'), 28, "B");
    }
}

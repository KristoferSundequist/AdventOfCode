mod linked_list;
use linked_list::CustomLinkedList;

fn main() {
    let filepath = "./input.txt";
    part1(filepath);
    part2(filepath);
}

fn part1(filepath: &str) {
    let input = parse(filepath);
    let mut ll = CustomLinkedList::new(&input);
    for i in 0..input.len() {
        ll.move_index(i);
    }
    let coordinates = ll.groove_coordinates();
    println!("Part 1: {}", coordinates);
}

fn part2(filepath: &str) {
    let input = parse(filepath)
        .iter()
        .map(|n| n * 811589153)
        .collect::<Vec<i64>>();
    let mut ll = CustomLinkedList::new(&input);
    for _ in 0..10 {
        for i in 0..input.len() {
            ll.move_index(i);
        }
    }
    let coordinates = ll.groove_coordinates();
    println!("Part 1: {}", coordinates);
}

fn parse(filepath: &str) -> Vec<i64> {
    let input = std::fs::read_to_string(filepath).unwrap();
    input
        .lines()
        .map(|line| line.parse::<i64>().unwrap())
        .collect()
}

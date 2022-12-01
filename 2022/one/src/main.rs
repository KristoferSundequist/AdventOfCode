fn main() {
    let input = std::fs::read_to_string("./input.txt").unwrap();

    let elfs = input
        .split("\n\n")
        .map(|elf| {
            elf.split("\n")
                .map(|str| str.parse::<i32>().unwrap())
                .collect::<Vec<i32>>()
        })
        .collect::<Vec<Vec<i32>>>();

    let mut counts = elfs
        .iter()
        .map(|elf| elf.iter().sum())
        .collect::<Vec<i32>>();

    counts.sort();

    println!("Part 1: {:#?}", counts[counts.len() - 1]);
    println!(
        "Part 2: {:#?}",
        counts[counts.len() - 1] + counts[counts.len() - 2] + counts[counts.len() - 3]
    );
}

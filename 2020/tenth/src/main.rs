fn part_one(numbers: &Vec<i64>) -> i64 {
    let mut sorted_numbers = numbers.clone();
    sorted_numbers.sort();

    let mut diffs = vec![0, 0, 0, 1];
    diffs[sorted_numbers[0] as usize] += 1;

    for i in 1..sorted_numbers.len() {
        diffs[(sorted_numbers[i as usize] - sorted_numbers[(i - 1) as usize]) as usize] += 1;
    }
    println!("{:#?}", diffs);
    diffs[1] * diffs[3]
}

fn can_goto(i: usize, dist: usize, sorted_numbers: &Vec<i64>) -> bool {
    if i + dist >= sorted_numbers.len() {
        return false
    }

    if sorted_numbers[i + dist] - sorted_numbers[i] <= 3 {
        true
    } else {
        false
    }
}


fn part_two(numbers: &Vec<i64>) -> i64 {
    let mut sorted_numbers = numbers.clone();
    sorted_numbers.sort();

    let mut ways: Vec<i64> = (0..sorted_numbers.len()).map(|_| 0).collect();

    ways[0] += 1;

    if sorted_numbers[1] <= 3 {
        ways[1] += 1;
    }

    if sorted_numbers[2] <= 3 {
        ways[2] += 1;
    }
    

    for i in 0..sorted_numbers.len()-1 {
        ways[i+1] += ways[i];
        if can_goto(i, 2, &sorted_numbers) {
            ways[i+2] += ways[i];
        }
        
        if can_goto(i, 3, &sorted_numbers) {
            ways[i+3] += ways[i];
        }
    }
    println!("ways: {:#?}", ways);
    ways.last().unwrap().clone()
}

fn main() {
    let numbers: Vec<i64> = std::fs::read_to_string("./data.txt")
        .unwrap()
        .lines()
        .map(|v| v.parse::<i64>().unwrap())
        .collect();

    let result1 = part_one(&numbers);
    println!("part one: {:#?}", result1);

    let result2 = part_two(&numbers);
    println!("part two: {:#?}", result2);
}

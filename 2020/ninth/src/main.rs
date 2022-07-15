use std::fs;

fn part_one(nums: &Vec<i64>) -> i64 {
    for i in 25..nums.len() {
        let preamble = &nums[i-25..i];
        let sums = (0..25).flat_map(|j|
            (0..25).filter(|z| z != &j).map(|z| preamble[j] + preamble[z]).collect::<Vec<i64>>()
        ).collect::<Vec<i64>>();
        if !sums.contains(&nums[i]) {
            return nums[i]
        }
    }
    -1
}

fn part_two(nums: &Vec<i64>, x: i64) -> i64 {
    for i in 0..nums.len()-1 {
        let mut sum = nums[i];
        for j in i+1..nums.len() {
            sum += nums[j];
            if sum == x {
                let min = nums[i..=j].iter().min().unwrap();
                let max = nums[i..=j].iter().max().unwrap();
                return min+max;
            } else if sum > x {
                break;
            }
        }
    }
    -1
}
fn main() {
    let nums = fs::read_to_string("./data.txt").unwrap().lines().map(|l| l.parse::<i64>().unwrap()).collect::<Vec<i64>>();

    let first = part_one(&nums);
    println!("{:#?}", first);

    let second = part_two(&nums, first);
    println!("{:#?}", second);
}

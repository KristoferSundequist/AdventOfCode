use std::collections::{VecDeque, HashSet};

fn part_one(first: &VecDeque<i64>, second: &VecDeque<i64>) -> i64 {
    let mut firstc = first.clone();
    let mut secondc = second.clone();

    while firstc.len() > 0 && secondc.len() > 0 {
        let first_top = firstc.pop_front().unwrap();
        let second_top = secondc.pop_front().unwrap();

        if first_top > second_top {
            firstc.push_back(first_top);
            firstc.push_back(second_top);
        } else {
            secondc.push_back(second_top);
            secondc.push_back(first_top);
        }
    }

    let winner = if firstc.len() == 0 { secondc } else { firstc };
    winner.iter().rev().enumerate().map(|(i, c)| ((i + 1) as i64)*c).sum()
}

fn part_two(first: &VecDeque<i64>, second: &VecDeque<i64>) -> (usize, i64) {
    //println!(" ---- New game -----");
    let mut firstc = first.clone();
    let mut secondc = second.clone();
    let mut prev = HashSet::<(VecDeque<i64>, VecDeque<i64>)>::new();

    while firstc.len() > 0 && secondc.len() > 0 {
        if prev.contains(&(firstc.clone(), secondc.clone())) {
            return (0, 0);
        } else {
            prev.insert((firstc.clone(), secondc.clone()));
        }

        let first_top = firstc.pop_front().unwrap();
        let second_top = secondc.pop_front().unwrap();
        // println!("1 draws: {}", first_top);
        // println!("2 draws: {}", second_top);
        // println!("---------");

        // subgame
        if first_top <= (firstc.len() as i64) && second_top <= (secondc.len() as i64) {
            let first_copy = &firstc.clone().into_iter().take(first_top as usize).collect::<VecDeque<_>>();
            let second_copy = &secondc.clone().into_iter().take(second_top as usize).collect::<VecDeque<_>>();
            let (wi, _) = part_two(&first_copy, &second_copy);
            if wi == 0 {
                firstc.push_back(first_top);
                firstc.push_back(second_top);
            } else {
                secondc.push_back(second_top);
                secondc.push_back(first_top);
            }
        } else {
            if first_top > second_top {
                firstc.push_back(first_top);
                firstc.push_back(second_top);
            } else {
                secondc.push_back(second_top);
                secondc.push_back(first_top);
            }
        }
    }

    // println!("first: {:?}", firstc);
    // println!("second: {:?}", secondc);

    let (i, winner) = if firstc.len() == 0 { (1, secondc) } else { (0, firstc) };
    (i, winner.iter().rev().enumerate().map(|(i, c)| ((i + 1) as i64)*c).sum())
}

fn main() {
    let first = std::fs::read_to_string("./data1.txt").unwrap().lines().map(|v| v.parse::<i64>().unwrap()).collect::<VecDeque<_>>();
    let second = std::fs::read_to_string("./data2.txt").unwrap().lines().map(|v| v.parse::<i64>().unwrap()).collect::<VecDeque<_>>();

    let result1 = part_one(&first, &second);
    println!("result1: {:?}", result1);

    let (a,b) = part_two(&first, &second);
    println!("result2: {:?} {:?}", a, b);
}

use std::fs;

fn get_seat(v: &Vec<char>) -> i32 {
    let row = {
        let mut lower = 0;
        let mut upper = 127;
        for r in &v[0..7] {
            let midpoint = lower + (upper - lower) / 2;
            if r == &'F' {
                upper = midpoint;
            } else {
                lower = midpoint + 1;
            }
        }
        lower
    };

    let col = {
        let mut lower = 0;
        let mut upper = 7;
        for r in &v[7..] {
            let midpoint = lower + (upper - lower) / 2;
            if r == &'L' {
                upper = midpoint;
            } else {
                lower = midpoint + 1;
            }
        }
        lower
    };

    return row * 8 + col
}

fn main() {
    let passes: Vec<Vec<char>> = fs::read_to_string("./data.txt").unwrap().split("\n").map(|v| v.chars().collect::<Vec<char>>()).filter(|v| v.len() > 0).collect();
    let mut seats: Vec<i32> = passes.iter().map(|p| get_seat(p)).collect();
    seats.sort();
    for i in 1..seats.len() {
        if seats[i] - seats[i-1] != 1 {
            println!("{}", seats[i]);
        }
    }
    println!("{:#?}", seats);
}


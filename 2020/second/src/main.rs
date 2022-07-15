use std::fs;

#[derive(Debug, Clone)]
struct Line {
    low: usize,
    high: usize,
    char: char,
    password: String
}

fn to_line(s: &str) -> Line {
    let program_password: Vec<&str> = s.split(":").collect();
    let bounds_char: Vec<&str> = program_password[0].split(" ").collect();
    let low_high: Vec<&str> = bounds_char[0].split("-").collect();

    Line {
        low: low_high[0].parse().unwrap(),
        high: low_high[1].parse().unwrap(),
        char: bounds_char[1].parse().unwrap(),
        password: program_password[1].trim().parse().unwrap()
    }
}

fn is_correct(line: &Line) -> bool {
    let mut char_count = 0;
    for c in line.password.chars() {
        if c == line.char {
            char_count += 1;
        }
    }

    line.low <= char_count && char_count <= line.high
}

fn is_correct2(line: &Line) -> bool {
    let mut count = 0;
    if line.password.chars().nth(line.low-1).unwrap() == line.char {
        count += 1;
    }
    if line.password.chars().nth(line.high-1).unwrap() == line.char {
        count += 1;
    }
    count == 1
}


fn main() {
    let lines: Vec<Line> =
        fs::read_to_string("./data.txt").unwrap()
        .split("\n")
        .filter(|v| v.len() > 0)
        .map(to_line)
        .filter(is_correct2)
        .collect();

    println!("{:#?}", lines.len());
}

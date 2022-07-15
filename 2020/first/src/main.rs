use std::fs;

fn main() {
    let lines: Vec<i32> = fs::read_to_string("./data.txt")
        .unwrap()
        .split("\n")
        .filter(|v| v.len() > 0)
        .map(|v| v.parse::<i32>().unwrap())
        .collect();

    for i in 0..lines.len() - 2 {
        for j in i..lines.len() - 1 {
            for z in j..lines.len() {
                if lines[i] + lines[j] + lines[z] == 2020 {
                    println!("{}", lines[i] * lines[j] * lines[z])
                }
            }
        }
    }
}

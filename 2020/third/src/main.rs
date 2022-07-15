use std::fs;

fn get_n_trees(slope: &Vec<Vec<char>>, angle: &(usize, usize)) -> i32 {
    let slope_width = slope[0].len();
    let slope_height = slope.len();

    let mut current_position = (0,0);
    let mut tree_count = 0;
    for _ in 0..slope.len() {
        if slope[current_position.0][current_position.1] == '#' {
            tree_count += 1;
        }
        current_position = (
            current_position.0 + angle.1,
            (current_position.1 + angle.0) % slope_width
        );

        if current_position.0 >= slope_height {
            return tree_count
        }
    }
    tree_count
}

fn main() {
    let slope: Vec<Vec<char>> =
        fs::read_to_string("./data.txt").unwrap()
        .split("\n").filter(|v| v.len() > 0)
        .map(|l| l.chars().collect::<Vec<char>>())
        .collect();

    let angles = vec![
        (1,1),
        (3,1),
        (5,1),
        (7,1),
        (1,2)
    ];

    for angle in angles.iter() {
        let count = get_n_trees(&slope, angle);
        println!("angle ({}, {}) crosses {} trees", angle.0, angle.1, count);
    }

    let final_answer: i64 =
        angles.iter()
        .map(|a| get_n_trees(&slope, a))
        .fold(1i64, |acc, c| acc*(c as i64));

    println!("product is {}", final_answer);
}

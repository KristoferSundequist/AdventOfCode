mod board;
use board::{Board, Point};
use std::collections::HashSet;

fn main() {
    let boards_by_minute = get_boards_by_minute();
    let wavelength = get_wavelength(&boards_by_minute);
    let start_point = boards_by_minute[0].get_start_point();
    let end_point = boards_by_minute[0].get_end_point();

    let mut current_points = vec![start_point.clone()]
        .into_iter()
        .collect::<HashSet<_>>();
    let mut phase = 0;
    for minute in 0..10000 {
        let board_index = minute % wavelength;
        current_points = tick(&current_points, &boards_by_minute[board_index]);
        if phase == 0 && current_points.contains(&end_point) {
            println!("Part 1: {}", minute + 1);
            phase = 1;
            current_points = vec![end_point.clone()].into_iter().collect::<HashSet<_>>();
        }

        if phase == 1 && current_points.contains(&start_point) {
            println!("Reached start again: {}", minute + 1);
            phase = 2;
            current_points = vec![start_point.clone()]
                .into_iter()
                .collect::<HashSet<_>>();
        }

        if phase == 2 && current_points.contains(&end_point) {
            println!("Part 2: {}", minute + 1);
            break;
        }
    }
}

fn tick(old_points: &HashSet<Point>, board: &Board) -> HashSet<Point> {
    let mut new_points: HashSet<Point> = HashSet::new();
    for p in old_points.iter() {
        let moves = board.get_available_moves(p);
        for m in moves.into_iter() {
            new_points.insert(m);
        }
    }
    return new_points;
}

fn get_wavelength(boards: &Vec<Board>) -> usize {
    for i in 1..boards.len() {
        if boards[i] == boards[0] {
            return i;
        }
    }
    unreachable!("shouldnt happen");
}

fn get_boards_by_minute() -> Vec<Board> {
    let mut board = Board::new("input.txt");
    let mut boards_by_minute = vec![];
    for _ in 0..1000 {
        board = board.tick();
        boards_by_minute.push(board.clone());
    }
    boards_by_minute
}

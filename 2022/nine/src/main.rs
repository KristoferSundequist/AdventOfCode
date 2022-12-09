use std::collections::HashSet;

fn main() {
    let moves = parse_input("./input.txt");

    let part1 = calc_visited(&moves, 2, false);
    println!("Part 1: {}", part1);
    let part2 = calc_visited(&moves, 10, false);
    println!("Part 2: {}", part2);
}

fn parse_input(file_path: &str) -> Vec<Move> {
    let input = std::fs::read_to_string(file_path).unwrap();
    let lines = input.lines().collect::<Vec<_>>();
    return lines
        .iter()
        .map(|line| {
            let split = line.split(" ").collect::<Vec<_>>();
            let number = split[1].parse::<i32>().unwrap();
            return Move {
                amount: number,
                direction: match split[0] {
                    "U" => Direction::U,
                    "R" => Direction::R,
                    "D" => Direction::D,
                    "L" => Direction::L,
                    _ => panic!("aaahh"),
                },
            };
        })
        .collect::<Vec<_>>();
}

fn calc_visited(moves: &Vec<Move>, n_knots: usize, verbose: bool) -> i32 {
    let mut knots = (0..n_knots)
        .map(|_| Point { x: 0, y: 0 })
        .collect::<Vec<_>>();
    let mut visited = vec![Point { x: 0, y: 0 }]
        .into_iter()
        .collect::<HashSet<_>>();

    for current_move in moves {
        if verbose {
            println!("{:?}", current_move);
        }
        for _ in 0..current_move.amount {
            knots = get_new_knots(&knots, &current_move.direction, verbose);
            visited.insert(knots.last().unwrap().clone());
            if verbose {
                println!("------------------------------");
                draw_knots(&knots);
            }
        }
    }
    if verbose {
        println!("{:#?}", &visited);
        draw_visited(&visited);
    }
    return visited.len() as i32;
}

fn draw_visited(points: &HashSet<Point>) {
    let min_x = points.iter().map(|p| p.x).min().unwrap();
    let min_y = points.iter().map(|p| p.y).min().unwrap();
    let max_x = points.iter().map(|p| p.x).max().unwrap();
    let max_y = points.iter().map(|p| p.y).max().unwrap();
    for y in min_y..=max_y {
        for x in min_x..=max_x {
            if points.contains(&Point { x: x, y: y }) {
                print!("#");
            } else {
                print!(".");
            }
        }
        print!("\n");
    }
}

fn draw_knots(knots: &Vec<Point>) {
    let min_x = knots.iter().map(|p| p.x).min().unwrap();
    let min_y = knots.iter().map(|p| p.y).min().unwrap();
    let max_x = knots.iter().map(|p| p.x).max().unwrap();
    let max_y = knots.iter().map(|p| p.y).max().unwrap();
    for y in min_y..=max_y {
        for x in min_x..=max_x {
            let maybe_k = knots.iter().position(|p| p.x == x && p.y == y);
            if let Some(k) = maybe_k {
                if k == 0 {
                    print!("H");
                } else {
                    print!("{}", k);
                }
            } else {
                print!(".");
            }
        }
        print!("\n");
    }
}

fn move_point_in_direction(old_point: &Point, direction: &Direction) -> Point {
    if direction == &Direction::U {
        return Point {
            y: old_point.y - 1,
            x: old_point.x,
        };
    } else if direction == &Direction::R {
        return Point {
            y: old_point.y,
            x: old_point.x + 1,
        };
    } else if direction == &Direction::D {
        return Point {
            y: old_point.y + 1,
            x: old_point.x,
        };
    } else if direction == &Direction::L {
        return Point {
            y: old_point.y,
            x: old_point.x - 1,
        };
    }
    panic!("ahhhhh unknown direction");
}

fn move_point_towards(point_to_move: &Point, target: &Point) -> Point {
    let xdiff = target.x - point_to_move.x;
    let ydiff = target.y - point_to_move.y;
    let manhattan_distance = xdiff.abs() + ydiff.abs();
    if manhattan_distance >= 3 {
        return Point {
            x: point_to_move.x + xdiff.signum(),
            y: point_to_move.y + ydiff.signum(),
        };
    } else if ydiff.abs() == 2 {
        return Point {
            x: point_to_move.x,
            y: point_to_move.y + ydiff.signum(),
        };
    } else if xdiff.abs() == 2 {
        return Point {
            x: point_to_move.x + xdiff.signum(),
            y: point_to_move.y,
        };
    } else {
        return Point {
            x: point_to_move.x,
            y: point_to_move.y,
        };
    }
}

fn get_new_knots(old_knots: &Vec<Point>, direction: &Direction, verbose: bool) -> Vec<Point> {
    let mut new_knots: Vec<Point> = vec![move_point_in_direction(&old_knots[0], direction)];

    for i in 1..old_knots.len() {
        new_knots.push(move_point_towards(&old_knots[i], &new_knots[i - 1]));

        if verbose {
            println!("{}: {:?}", i, new_knots[i]);
        }
    }

    return new_knots;
}

#[derive(Debug, Hash, PartialEq, Eq, Clone)]
enum Direction {
    U,
    R,
    D,
    L,
}

#[derive(Debug, Hash, PartialEq, Eq, Clone)]
struct Move {
    direction: Direction,
    amount: i32,
}

#[derive(Debug, Hash, PartialEq, Eq, Clone)]
struct Point {
    x: i32,
    y: i32,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_testinput_2() {
        let moves = parse_input("./testinput.txt");
        let visited = calc_visited(&moves, 2, false);
        assert!(visited == 13);
    }

    #[test]
    fn test_testinput_10() {
        let moves = parse_input("./testinput.txt");
        let visited = calc_visited(&moves, 10, false);
        assert!(visited == 1);
    }

    #[test]
    fn test_testpart2_10() {
        let moves = parse_input("./testpart2.txt");
        let visited = calc_visited(&moves, 10, false);
        assert!(visited == 36);
    }

    #[test]
    fn test_input_2() {
        let moves = parse_input("./input.txt");
        let visited = calc_visited(&moves, 2, false);
        assert!(visited == 6018);
    }

    #[test]
    fn test_input_10() {
        let moves = parse_input("./input.txt");
        let visited = calc_visited(&moves, 10, false);
        assert!(visited == 2619);
    }

    #[test]
    fn test_move_towards_target_1() {
        let head = Point { x: 2, y: 2 };
        let tail = Point { x: 2, y: 2 };
        let new_tail = move_point_towards(&tail, &head);
        assert!(new_tail.x == 2 && new_tail.y == 2);
    }

    #[test]
    fn test_move_towards_target_2() {
        let head = Point { x: -5, y: -5 };
        let tail = Point { x: -5, y: -4 };
        let new_tail = move_point_towards(&tail, &head);
        assert!(new_tail.x == -5 && new_tail.y == -4);
    }

    #[test]
    fn test_move_towards_target_3() {
        let head = Point { x: -5, y: -5 };
        let tail = Point { x: -5, y: -3 };
        let new_tail = move_point_towards(&tail, &head);
        assert!(new_tail.x == -5 && new_tail.y == -4);
    }

    #[test]
    fn test_move_towards_target_4() {
        let head = Point { x: -5, y: -5 };
        let tail = Point { x: -3, y: -5 };
        let new_tail = move_point_towards(&tail, &head);
        assert!(new_tail.x == -4 && new_tail.y == -5);
    }

    #[test]
    fn test_move_towards_target_5() {
        let head = Point { x: -5, y: -5 };
        let tail = Point { x: -3, y: -4 };
        let new_tail = move_point_towards(&tail, &head);
        assert!(new_tail.x == -4 && new_tail.y == -5);
    }

    #[test]
    fn test_move_towards_target_6() {
        let head = Point { x: -5, y: -5 };
        let tail = Point { x: -7, y: -4 };
        let new_tail = move_point_towards(&tail, &head);
        assert!(new_tail.x == -6 && new_tail.y == -5);
    }

    #[test]
    fn test_move_towards_target_7() {
        let head = Point { x: -5, y: -5 };
        let tail = Point { x: -4, y: -3 };
        let new_tail = move_point_towards(&tail, &head);
        assert!(new_tail.x == -5 && new_tail.y == -4);
    }

    #[test]
    fn test_move_towards_target_8() {
        let head = Point { x: -5, y: -5 };
        let tail = Point { x: -3, y: -3 };
        let new_tail = move_point_towards(&tail, &head);
        assert!(new_tail.x == -4 && new_tail.y == -4);
    }
}

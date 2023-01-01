use std::collections::{HashMap, HashSet};

fn main() {
    let points = parse("./input.txt");
    let mut current_points = points.clone();
    for i in 0..10 {
        current_points = step(&current_points, i);
    }
    println!("Part 1: {}", get_ground(&current_points));

    let mut current_points2 = points.clone();
    for i in 0..1000000 {
        let new_points = step(&current_points2, i);
        if new_points == current_points2 {
            println!("Part 2: {}", i + 1);
            break;
        }
        current_points2 = new_points;
    }
}

fn step(points: &HashMap<i64, Point>, direction_start: usize) -> HashMap<i64, Point> {
    let mut proposals: HashMap<Point, i64> = HashMap::new();
    let mut rejects: HashSet<i64> = HashSet::new();

    let points_map = points
        .iter()
        .map(|(_, p)| p.clone())
        .collect::<HashSet<_>>();

    for (pid, p) in points.iter() {
        if p.get_surrounding()
            .iter()
            .all(|sp| !points_map.contains(sp))
        {
            proposals.insert(p.clone(), pid.clone());
            continue;
        }
        let mut did_propose_move = false;
        for i in 0..4 {
            let actual_i = (direction_start + i) % 4;
            match actual_i {
                0 => {
                    if p.get_north().iter().all(|sp| !points_map.contains(sp)) {
                        let move_point = Point { y: p.y - 1, x: p.x };
                        if let Some(other_point_id) = proposals.get(&move_point) {
                            rejects.insert(other_point_id.clone());
                            proposals.insert(p.clone(), pid.clone());
                        } else {
                            proposals.insert(move_point, pid.clone());
                        }
                        did_propose_move = true;
                        break;
                    }
                }
                1 => {
                    if p.get_south().iter().all(|sp| !points_map.contains(sp)) {
                        let move_point = Point { y: p.y + 1, x: p.x };
                        if let Some(other_point_id) = proposals.get(&move_point) {
                            rejects.insert(other_point_id.clone());
                            proposals.insert(p.clone(), pid.clone());
                        } else {
                            proposals.insert(move_point, pid.clone());
                        }
                        did_propose_move = true;
                        break;
                    }
                }
                2 => {
                    if p.get_west().iter().all(|sp| !points_map.contains(sp)) {
                        let move_point = Point { y: p.y, x: p.x - 1 };
                        if let Some(other_point_id) = proposals.get(&move_point) {
                            rejects.insert(other_point_id.clone());
                            proposals.insert(p.clone(), pid.clone());
                        } else {
                            proposals.insert(move_point, pid.clone());
                        }
                        did_propose_move = true;
                        break;
                    }
                }
                3 => {
                    if p.get_east().iter().all(|sp| !points_map.contains(sp)) {
                        let move_point = Point { y: p.y, x: p.x + 1 };
                        if let Some(other_point_id) = proposals.get(&move_point) {
                            rejects.insert(other_point_id.clone());
                            proposals.insert(p.clone(), pid.clone());
                        } else {
                            proposals.insert(move_point, pid.clone());
                        }
                        did_propose_move = true;
                        break;
                    }
                }
                _ => panic!("Should never happen"),
            }
        }
        if !did_propose_move {
            proposals.insert(p.clone(), pid.clone());
        }
    }
    let mut new_points: HashMap<i64, Point> = HashMap::new();

    for (p, pid) in proposals.iter() {
        if rejects.contains(pid) {
            if new_points.contains_key(pid) {
                println!("new points already contains it warrrrrning -- reject {pid}");
            }
            new_points.insert(pid.clone(), points.get(pid).unwrap().clone());
        } else {
            if new_points.contains_key(pid) {
                println!("new points already contains it warrrrrning -- not reject {pid}");
            }
            new_points.insert(pid.clone(), p.clone());
        }
    }

    return new_points;
}

fn parse(filepath: &str) -> HashMap<i64, Point> {
    let input = std::fs::read_to_string(filepath).unwrap();
    let mut points: HashMap<i64, Point> = HashMap::new();
    let mut current_id: i64 = 0;
    for (y, line) in input.lines().enumerate() {
        for (x, c) in line.chars().enumerate() {
            if c == '#' {
                points.insert(
                    current_id,
                    Point {
                        y: y as i64,
                        x: x as i64,
                    },
                );
                current_id += 1;
            }
        }
    }
    return points;
}

fn draw(points: &HashMap<i64, Point>) {
    let min_x = points.iter().map(|(_, p)| p.x).min().unwrap();
    let min_y = points.iter().map(|(_, p)| p.y).min().unwrap();
    let max_x = points.iter().map(|(_, p)| p.x).max().unwrap();
    let max_y = points.iter().map(|(_, p)| p.y).max().unwrap();

    let points_map = points
        .iter()
        .map(|(pid, p)| (p.clone(), pid.clone()))
        .collect::<HashMap<Point, i64>>();

    for y in min_y..=max_y {
        for x in min_x..=max_x {
            let current_point = Point { y: y, x: x };
            if let Some(pid) = points_map.get(&current_point) {
                print!("#");
            } else {
                print!(".");
            }
        }
        println!("");
    }
}

fn get_ground(points: &HashMap<i64, Point>) -> i64 {
    let min_x = points.iter().map(|(_, p)| p.x).min().unwrap();
    let min_y = points.iter().map(|(_, p)| p.y).min().unwrap();
    let max_x = points.iter().map(|(_, p)| p.x).max().unwrap();
    let max_y = points.iter().map(|(_, p)| p.y).max().unwrap();

    return (max_y - min_y + 1).abs() * (max_x - min_x + 1).abs() - points.len() as i64;
}

#[derive(Clone, Debug, Eq, PartialEq, Hash)]
struct Point {
    y: i64,
    x: i64,
}

impl Point {
    fn get_surrounding(&self) -> Vec<Point> {
        return vec![
            Point {
                y: self.y - 1,
                x: self.x - 1,
            },
            Point {
                y: self.y - 1,
                x: self.x,
            },
            Point {
                y: self.y - 1,
                x: self.x + 1,
            },
            Point {
                y: self.y,
                x: self.x - 1,
            },
            Point {
                y: self.y,
                x: self.x + 1,
            },
            Point {
                y: self.y + 1,
                x: self.x - 1,
            },
            Point {
                y: self.y + 1,
                x: self.x,
            },
            Point {
                y: self.y + 1,
                x: self.x + 1,
            },
        ];
    }

    fn get_north(&self) -> Vec<Point> {
        return vec![
            Point {
                y: self.y - 1,
                x: self.x - 1,
            },
            Point {
                y: self.y - 1,
                x: self.x,
            },
            Point {
                y: self.y - 1,
                x: self.x + 1,
            },
        ];
    }

    fn get_south(&self) -> Vec<Point> {
        return vec![
            Point {
                y: self.y + 1,
                x: self.x - 1,
            },
            Point {
                y: self.y + 1,
                x: self.x,
            },
            Point {
                y: self.y + 1,
                x: self.x + 1,
            },
        ];
    }

    fn get_west(&self) -> Vec<Point> {
        return vec![
            Point {
                y: self.y - 1,
                x: self.x - 1,
            },
            Point {
                y: self.y,
                x: self.x - 1,
            },
            Point {
                y: self.y + 1,
                x: self.x - 1,
            },
        ];
    }

    fn get_east(&self) -> Vec<Point> {
        return vec![
            Point {
                y: self.y - 1,
                x: self.x + 1,
            },
            Point {
                y: self.y,
                x: self.x + 1,
            },
            Point {
                y: self.y + 1,
                x: self.x + 1,
            },
        ];
    }
}

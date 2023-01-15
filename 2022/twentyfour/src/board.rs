use std::collections::{HashMap, HashSet};

#[derive(Clone, Debug, Eq, PartialEq)]
pub struct Board {
    board: HashSet<Point>,
    storms: HashMap<i64, Point>,
    storm_points: HashSet<Point>,
    storm_directions: HashMap<i64, Direction>,
    min_x: i64,
    min_y: i64,
    max_x: i64,
    max_y: i64,
}

impl Board {
    pub fn new(filepath: &str) -> Board {
        let input = std::fs::read_to_string(filepath).unwrap();
        let mut board: HashSet<Point> = HashSet::new();
        let mut storms: HashMap<i64, Point> = HashMap::new();
        let mut storm_directions: HashMap<i64, Direction> = HashMap::new();
        let mut storm_points: HashSet<Point> = HashSet::new();
        let mut storm_id: i64 = 0;
        for (y, line) in input.lines().enumerate() {
            for (x, c) in line.chars().enumerate() {
                let point = Point {
                    y: y as i64,
                    x: x as i64,
                };
                if c != '#' {
                    board.insert(point.clone());
                }
                let direction: Option<Direction> = match c {
                    '^' => Some(Direction::Up),
                    'v' => Some(Direction::Down),
                    '<' => Some(Direction::Left),
                    '>' => Some(Direction::Right),
                    _ => None,
                };
                if let Some(direction) = direction {
                    storms.insert(storm_id, point.clone());
                    storm_directions.insert(storm_id, direction);
                    storm_points.insert(point);
                    storm_id += 1;
                }
            }
        }

        let min_x = 1;
        let min_y = 1;
        let max_x = board.iter().map(|p| p.x).max().unwrap();
        let max_y = board.iter().map(|p| p.y).max().unwrap() - 1;

        Board {
            board,
            storms,
            storm_points,
            storm_directions,
            min_x,
            min_y,
            max_x,
            max_y,
        }
    }

    pub fn draw(&self) {
        let min_x = 0;
        let min_y = 0;
        let max_x = self.board.iter().map(|p| p.x).max().unwrap();
        let max_y = self.board.iter().map(|p| p.y).max().unwrap();

        let storm_points: HashMap<Point, Direction> = self
            .storms
            .iter()
            .map(|(id, p)| (p.clone(), self.storm_directions.get(id).unwrap().clone()))
            .collect();

        for y in min_y..=max_y {
            for x in min_x..=max_x {
                let point = Point { y, x };
                if let Some(storm_direction) = storm_points.get(&point) {
                    print!("{}", storm_direction.to_string());
                } else if self.board.contains(&point) {
                    print!(".");
                } else {
                    print!(" ");
                }
            }
            println!();
        }
    }

    pub fn tick(self) -> Board {
        let mut new_storms: HashMap<i64, Point> = HashMap::new();
        let mut new_storm_points = HashSet::new();
        for (id, point) in self.storms.iter() {
            let direction = self.storm_directions.get(id).unwrap();
            let new_point = match direction {
                Direction::Up => Point {
                    y: if point.y - 1 < self.min_y {
                        self.max_y
                    } else {
                        point.y - 1
                    },
                    x: point.x,
                },
                Direction::Down => Point {
                    y: if point.y + 1 > self.max_y {
                        self.min_y
                    } else {
                        point.y + 1
                    },
                    x: point.x,
                },
                Direction::Left => Point {
                    y: point.y,
                    x: if point.x - 1 < self.min_x {
                        self.max_x
                    } else {
                        point.x - 1
                    },
                },
                Direction::Right => Point {
                    y: point.y,
                    x: if point.x + 1 > self.max_x {
                        self.min_x
                    } else {
                        point.x + 1
                    },
                },
            };
            new_storms.insert(*id, new_point.clone());
            new_storm_points.insert(new_point);
        }

        Board {
            board: self.board,
            storms: new_storms,
            storm_points: new_storm_points,
            storm_directions: self.storm_directions,
            min_x: self.min_x,
            min_y: self.min_y,
            max_x: self.max_x,
            max_y: self.max_y,
        }
    }

    pub fn get_size(&self) -> i64 {
        return (self.board.iter().map(|p| p.x).max().unwrap() - 1)
            * (self.board.iter().map(|p| p.y).max().unwrap() - 2);
    }

    pub fn get_available_moves(&self, p: &Point) -> HashSet<Point> {
        let mut moves: HashSet<Point> = HashSet::new();

        // up
        let up = Point { y: p.y - 1, x: p.x };
        if !self.storm_points.contains(&up) && self.board.contains(&up) {
            moves.insert(up);
        }

        // down
        let down = Point { y: p.y + 1, x: p.x };
        if !self.storm_points.contains(&down) && self.board.contains(&down) {
            moves.insert(down);
        }

        // left
        let left = Point { y: p.y, x: p.x - 1 };
        if !self.storm_points.contains(&left) && self.board.contains(&left) {
            moves.insert(left);
        }

        // right
        let right = Point { y: p.y, x: p.x + 1 };
        if !self.storm_points.contains(&right) && self.board.contains(&right) {
            moves.insert(right);
        }

        // wait
        let wait = Point { y: p.y, x: p.x };
        if !self.storm_points.contains(&wait) && self.board.contains(&wait) {
            moves.insert(wait);
        }

        return moves;
    }

    pub fn get_end_point(&self) -> Point {
        return self
            .board
            .iter()
            .max_by(|&p1, &p2| return p1.y.cmp(&p2.y))
            .unwrap()
            .clone();
    }

    pub fn get_start_point(&self) -> Point {
        return self
            .board
            .iter()
            .min_by(|&p1, &p2| return p1.y.cmp(&p2.y))
            .unwrap()
            .clone();
    }
}

#[derive(Clone, Debug, Eq, PartialEq, Hash)]
enum Direction {
    Up,
    Down,
    Left,
    Right,
}

impl Direction {
    fn to_string(&self) -> String {
        match self {
            Direction::Up => "^".to_string(),
            Direction::Down => "v".to_string(),
            Direction::Left => "<".to_string(),
            Direction::Right => ">".to_string(),
        }
    }
}

#[derive(Clone, Debug, Eq, PartialEq, Hash)]
pub struct Point {
    pub y: i64,
    pub x: i64,
}

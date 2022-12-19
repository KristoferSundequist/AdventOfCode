use std::collections::HashSet;

fn main() {
    let input = parse("./input.txt");
    part1(&input);
    let out_air = get_surrounding_air(&input);
    part2(&input, &out_air);
}

fn part1(points: &HashSet<Point>) {
    let mut area = 0;
    for p in points.iter() {
        let surrounding = p.get_surrounding();
        for s in surrounding.iter() {
            if !points.contains(&s) {
                area += 1;
            }
        }
    }
    println!("Part 1: {}", area);
}

fn part2(points: &HashSet<Point>, out_air: &HashSet<Point>) {
    let mut area = 0;
    for p in points.iter() {
        let surrounding = p.get_surrounding();
        for s in surrounding.iter() {
            if !points.contains(&s) && out_air.contains(&s) {
                area += 1;
            }
        }
    }
    println!("Part 2: {}", area);
}

fn get_surrounding_air(points: &HashSet<Point>) -> HashSet<Point> {
    let edge_point = points.iter().min_by(|&p1, &p2| p1.x.cmp(&p2.x)).unwrap();

    let x_lower_bound = points.iter().map(|p| p.x).min().unwrap() - 1;
    let x_upper_bound = points.iter().map(|p| p.x).max().unwrap() + 1;

    let y_lower_bound = points.iter().map(|p| p.y).min().unwrap() - 1;
    let y_upper_bound = points.iter().map(|p| p.y).max().unwrap() + 1;

    let z_lower_bound = points.iter().map(|p| p.z).min().unwrap() - 1;
    let z_upper_bound = points.iter().map(|p| p.z).max().unwrap() + 1;

    let mut air_candidates = vec![Point {
        x: edge_point.x - 1,
        y: edge_point.y,
        z: edge_point.z,
    }];
    let mut surrounding_air = HashSet::<Point>::new();
    loop {
        let maybe_air_candidate = air_candidates.pop();
        if let Some(air_candidate) = maybe_air_candidate {
            if points.contains(&air_candidate) {
                continue;
            }
            surrounding_air.insert(air_candidate.clone());
            let surrounding = air_candidate.get_surrounding();
            for s in surrounding.iter() {
                let is_within_bounds = x_lower_bound <= s.x
                    && s.x <= x_upper_bound
                    && y_lower_bound <= s.y
                    && s.y <= y_upper_bound
                    && z_lower_bound <= s.z
                    && s.z <= z_upper_bound;
                if !surrounding_air.contains(s) && is_within_bounds {
                    air_candidates.push(s.clone());
                }
            }
        } else {
            return surrounding_air;
        }
    }
}

fn parse(filepath: &str) -> HashSet<Point> {
    let input = std::fs::read_to_string(filepath).unwrap();
    input
        .lines()
        .map(|line| {
            let numbers = line
                .split(",")
                .map(|str| str.parse::<i32>().unwrap())
                .collect::<Vec<_>>();
            Point {
                x: numbers[0],
                y: numbers[1],
                z: numbers[2],
            }
        })
        .collect::<HashSet<_>>()
}

#[derive(Clone, Debug, Hash, Eq, PartialEq)]
struct Point {
    x: i32,
    y: i32,
    z: i32,
}

impl Point {
    fn get_surrounding(&self) -> Vec<Point> {
        vec![
            Point {
                x: self.x - 1,
                y: self.y,
                z: self.z,
            },
            Point {
                x: self.x + 1,
                y: self.y,
                z: self.z,
            },
            Point {
                x: self.x,
                y: self.y - 1,
                z: self.z,
            },
            Point {
                x: self.x,
                y: self.y + 1,
                z: self.z,
            },
            Point {
                x: self.x,
                y: self.y,
                z: self.z - 1,
            },
            Point {
                x: self.x,
                y: self.y,
                z: self.z + 1,
            },
        ]
    }
}

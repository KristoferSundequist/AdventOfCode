use regex::Regex;
use std::collections::HashSet;

fn main() {
    let (filepath, y_row, part2max) = if false {
        ("./testinput.txt", 10, 20)
    } else {
        ("./input.txt", 2000000, 4000000)
    };
    let input = parse(filepath);
    let distances = input
        .iter()
        .map(|(sensor, beacon)| manhattan_distance(sensor, beacon))
        .collect::<Vec<_>>();
    let mut stuff: HashSet<Point> = HashSet::new();
    for i in 0..input.len() {
        stuff.insert(input[i].0.clone());
        stuff.insert(input[i].1.clone());
    }
    let max_distance = distances.iter().max().unwrap();
    let minx = stuff.iter().map(|p| p.x).min().unwrap() - max_distance - 100000;
    let maxx = stuff.iter().map(|p| p.x).max().unwrap() + max_distance + 1000000;

    let mut empty_count: i64 = 0;
    for x in minx..maxx {
        let p = Point { x: x, y: y_row };
        if (0..input.len()).any(|i| {
            if manhattan_distance(&p, &input[i].0) <= distances[i] && !stuff.contains(&p) {
                true
            } else {
                false
            }
        }) {
            empty_count += 1;
        }
    }
    println!("Part 1: {}", empty_count);

    let mut candidate_points: HashSet<Point> = HashSet::new();
    input.iter().for_each(|(sensor, beacon)| {
        let distance = manhattan_distance(sensor, beacon);
        let circle_points = get_circle(sensor, distance + 1)
            .into_iter()
            .filter(|p| 0 <= p.x && p.x <= part2max && 0 <= p.y && p.y <= part2max);
        for p in circle_points {
            candidate_points.insert(p);
        }
    });
    for p in candidate_points {
        if (0..input.len()).all(|i| {
            if manhattan_distance(&p, &input[i].0) <= distances[i] || stuff.contains(&p) {
                false
            } else {
                true
            }
        }) {
            println!("Part 2: {} {:?}", p.x * 4000000 + p.y, p);
        }
    }
}

fn get_circle(center: &Point, distance: i64) -> HashSet<Point> {
    let mut points: HashSet<Point> = HashSet::new();
    for i in 0..=distance {
        points.insert(Point {
            x: center.x + distance - i,
            y: center.y - i,
        });

        points.insert(Point {
            x: center.x - i,
            y: center.y - distance + i,
        });

        points.insert(Point {
            x: center.x - distance + i,
            y: center.y + i,
        });

        points.insert(Point {
            x: center.x + i,
            y: center.y + distance - i,
        });
    }
    return points;
}

fn parse(filepath: &str) -> Vec<(Point, Point)> {
    let input = std::fs::read_to_string(filepath).unwrap();
    let re = Regex::new(
        "Sensor at x=(-?[0-9]+), y=(-?[0-9]+): closest beacon is at x=(-?[0-9]+), y=(-?[0-9]+)$",
    )
    .unwrap();

    input
        .lines()
        .map(|line| {
            let captures = re.captures(line).unwrap();
            return (
                Point {
                    x: captures.get(1).unwrap().as_str().parse::<i64>().unwrap(),
                    y: captures.get(2).unwrap().as_str().parse::<i64>().unwrap(),
                },
                Point {
                    x: captures.get(3).unwrap().as_str().parse::<i64>().unwrap(),
                    y: captures.get(4).unwrap().as_str().parse::<i64>().unwrap(),
                },
            );
        })
        .collect::<Vec<_>>()
}

fn manhattan_distance(p1: &Point, p2: &Point) -> i64 {
    return (p1.x - p2.x).abs() + (p1.y - p2.y).abs();
}

#[derive(Debug, Clone, Hash, Eq, PartialEq)]
struct Point {
    x: i64,
    y: i64,
}

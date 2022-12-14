use std::collections::HashMap;

fn main() {
    let input = parse_input("./input.txt");

    let sanded1 = pour_sand1(&input);
    println!(
        "Part 1: {}",
        sanded1.iter().filter(|(_, t)| *t == &Thing::Sand).count()
    );

    let sanded2 = pour_sand2(&input);
    println!(
        "Part 2: {}",
        sanded2.iter().filter(|(_, t)| *t == &Thing::Sand).count()
    );
}

fn pour_sand1(input: &HashMap<Point, Thing>) -> HashMap<Point, Thing> {
    let maxy = input.iter().map(|(p, _)| p.y).max().unwrap();
    let mut sanded = input.clone();
    loop {
        let mut current_sand_location = Point { x: 500, y: 0 };
        loop {
            let under = Point {
                x: current_sand_location.x,
                y: current_sand_location.y + 1,
            };
            let under_left = Point {
                x: current_sand_location.x - 1,
                y: current_sand_location.y + 1,
            };
            let under_right = Point {
                x: current_sand_location.x + 1,
                y: current_sand_location.y + 1,
            };

            if current_sand_location.y >= maxy {
                return sanded;
            } else if !sanded.contains_key(&under) {
                current_sand_location = under;
            } else if !sanded.contains_key(&under_left) {
                current_sand_location = under_left;
            } else if !sanded.contains_key(&under_right) {
                current_sand_location = under_right;
            } else {
                sanded.insert(current_sand_location.clone(), Thing::Sand);
                break;
            }
        }
    }
}

fn pour_sand2(input: &HashMap<Point, Thing>) -> HashMap<Point, Thing> {
    let maxy = input.iter().map(|(p, _)| p.y).max().unwrap();
    let mut sanded = input.clone();
    loop {
        let mut current_sand_location = Point { x: 500, y: 0 };
        loop {
            let under = Point {
                x: current_sand_location.x,
                y: current_sand_location.y + 1,
            };
            let under_left = Point {
                x: current_sand_location.x - 1,
                y: current_sand_location.y + 1,
            };
            let under_right = Point {
                x: current_sand_location.x + 1,
                y: current_sand_location.y + 1,
            };

            if current_sand_location.y == maxy + 1 {
                sanded.insert(current_sand_location.clone(), Thing::Sand);
                break;
            } else if !sanded.contains_key(&under) {
                current_sand_location = under;
            } else if !sanded.contains_key(&under_left) {
                current_sand_location = under_left;
            } else if !sanded.contains_key(&under_right) {
                current_sand_location = under_right;
            } else {
                sanded.insert(current_sand_location.clone(), Thing::Sand);
                if current_sand_location == (Point { x: 500, y: 0 }) {
                    return sanded;
                } else {
                    break;
                }
            }
        }
    }
}

fn parse_input(filepath: &str) -> HashMap<Point, Thing> {
    let input = std::fs::read_to_string(filepath).unwrap();

    let mut things: HashMap<Point, Thing> = HashMap::new();
    input.lines().for_each(|line| {
        let ts = parse_line(line);
        for (p, t) in ts.into_iter() {
            things.insert(p, t);
        }
    });
    return things;
}

fn draw_stuff(things: &HashMap<Point, Thing>) {
    let minx = things.iter().map(|(p, _)| p.x).min().unwrap();
    let maxx = things.iter().map(|(p, _)| p.x).max().unwrap();
    let miny = things.iter().map(|(p, _)| p.y).min().unwrap();
    let maxy = things.iter().map(|(p, _)| p.y).max().unwrap();

    for y in miny..=maxy {
        for x in minx..=maxx {
            let char_repr = match things.get(&Point { x: x, y: y }) {
                Some(Thing::Rock) => '#',
                Some(Thing::Sand) => 'o',
                None => '.',
            };
            print!("{}", char_repr);
        }
        println!("");
    }
}

fn parse_line(line: &str) -> HashMap<Point, Thing> {
    let mut points: HashMap<Point, Thing> = HashMap::new();
    let mut maybe_current_location: Option<Point> = None;
    line.split("->").for_each(|point_str| {
        let point_parts = point_str.split(",").collect::<Vec<_>>();
        let point = Point {
            x: point_parts[0].trim().parse::<i64>().unwrap(),
            y: point_parts[1].trim().parse::<i64>().unwrap(),
        };
        if let Some(current_location) = &maybe_current_location {
            let distance = manhattan_distance(&current_location, &point);
            let dxsign = -(current_location.x - point.x).signum();
            let dysign = -(current_location.y - point.y).signum();
            for i in 0..=distance {
                let temp_point = Point {
                    x: current_location.x + i * dxsign,
                    y: current_location.y + i * dysign,
                };
                points.insert(temp_point, Thing::Rock);
            }
            maybe_current_location = Some(Point {
                x: current_location.x + distance * dxsign,
                y: current_location.y + distance * dysign,
            });
        } else {
            points.insert(point.clone(), Thing::Rock);
            maybe_current_location = Some(point.clone());
        }
    });
    return points;
}

fn manhattan_distance(p1: &Point, p2: &Point) -> i64 {
    return (p1.x - p2.x).abs() + (p1.y - p2.y).abs();
}

#[derive(Debug, Clone, Eq, PartialEq, Hash)]
struct Point {
    x: i64,
    y: i64,
}

#[derive(Debug, Clone, Eq, PartialEq, Hash)]
enum Thing {
    Rock,
    Sand,
}

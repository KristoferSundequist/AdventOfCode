use std::collections::HashMap;

fn main() {
    let (elevation_map, goal_location, start_location) = parse_input("./input.txt");
    let all_path_lengths = calc(&elevation_map, &goal_location);
    println!("Part 1: {:#?}", all_path_lengths.get(&start_location).unwrap());

    let mut current_shortest = usize::MAX;
    for y in 0..elevation_map.len() {
        for x in 0..elevation_map[0].len() {
            if elevation_map[y][x] == 97 {
                if let Some(&length) = all_path_lengths.get(&Location { y: y, x: x }) {
                    if length < current_shortest {
                        current_shortest = length;
                    }
                }
            }
        }
    }
    println!("Part 2: {}", current_shortest);
}

fn calc(elevation_map: &Vec<Vec<usize>>, start: &Location) -> HashMap<Location, usize> {
    let mut shortest_so_far: HashMap<Location, usize> = HashMap::new();
    calc_num_steps_helper(&mut shortest_so_far, &elevation_map, &start, 0);
    return shortest_so_far;
}

fn calc_num_steps_helper(
    shortest_so_far: &mut HashMap<Location, usize>,
    elevation_map: &Vec<Vec<usize>>,
    current_location: &Location,
    cur_steps: usize,
) {
    if let Some(&current_shortest) = shortest_so_far.get(current_location) {
        if cur_steps >= current_shortest {
            return;
        }
    }
    shortest_so_far.insert(current_location.clone(), cur_steps);
    let sides = get_sides(&current_location, &elevation_map);
    for side in sides {
        calc_num_steps_helper(shortest_so_far, &elevation_map, &side, cur_steps + 1);
    }
}

fn get_sides(location: &Location, elevation_map: &Vec<Vec<usize>>) -> Vec<Location> {
    let mut sides: Vec<Location> = vec![];
    if location.x > 0 {
        let candidate = Location {
            y: location.y,
            x: location.x - 1,
        };
        if elevation_map[candidate.y][candidate.x] + 1 >= elevation_map[location.y][location.x] {
            sides.push(candidate);
        }
    }
    if location.x + 1 < elevation_map[0].len() {
        let candidate = Location {
            y: location.y,
            x: location.x + 1,
        };
        if elevation_map[candidate.y][candidate.x] + 1 >= elevation_map[location.y][location.x] {
            sides.push(candidate);
        }
    }
    if location.y > 0 {
        let candidate = Location {
            y: location.y - 1,
            x: location.x,
        };
        if elevation_map[candidate.y][candidate.x] + 1 >= elevation_map[location.y][location.x] {
            sides.push(candidate);
        }
    }
    if location.y + 1 < elevation_map.len() {
        let candidate = Location {
            y: location.y + 1,
            x: location.x,
        };
        if elevation_map[candidate.y][candidate.x] + 1 >= elevation_map[location.y][location.x] {
            sides.push(candidate);
        }
    }
    return sides;
}

fn parse_input(filepath: &str) -> (Vec<Vec<usize>>, Location, Location) {
    let input = std::fs::read_to_string(filepath).unwrap();
    let mut lines = input
        .lines()
        .map(|line| line.chars().collect::<Vec<_>>())
        .collect::<Vec<_>>();
    let mut goal_location: Option<Location> = None;
    let mut start_location: Option<Location> = None;
    for y in 0..lines.len() {
        for x in 0..lines[0].len() {
            if lines[y][x] == 'E' {
                goal_location = Some(Location { y: y, x: x });
                lines[y][x] = 'z';
            } else if lines[y][x] == 'S' {
                start_location = Some(Location { y: y, x: x });
                lines[y][x] = 'a';
            }
        }
    }

    let as_heights = lines
        .iter()
        .map(|row| row.iter().map(|&c| c as usize).collect::<Vec<_>>())
        .collect::<Vec<_>>();
    if let Some(goal) = goal_location {
        if let Some(start) = start_location {
            return (as_heights, goal, start);
        } else {
            panic!("Unexpecedd no goal");
        }
    } else {
        panic!("Unexpecedd no goal");
    }
}

#[derive(Clone, Hash, PartialEq, Eq, Debug)]
struct Location {
    y: usize,
    x: usize,
}

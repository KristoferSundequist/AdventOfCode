use std::collections::{HashSet, HashMap};

type Coord = (i32, i32, i32);

type Coord4d = (i32, i32, i32, i32);

fn get_neighbors((x,y,z): &Coord) -> HashSet<Coord> {
    (-1..=1).map(|xi|
        (-1..=1).map(|yi|
            (-1..=1).map(move |zi| (x+xi,y+yi,z+zi))
        ).flatten().collect::<Vec<_>>()
    ).flatten()
    .filter(|&c| c != (*x,*y,*z))
    .into_iter().collect::<HashSet<_>>()
}

fn get_neighbors4d((x,y,z,w): &Coord4d) -> HashSet<Coord4d> {
    (-1..=1).map(|xi|
        (-1..=1).map(|yi|
            (-1..=1).map(|zi| 
                (-1..=1).map(move |wi| (x+xi,y+yi,z+zi,w+wi))
            ).flatten().collect::<Vec<_>>()
        ).flatten().collect::<Vec<_>>()
    )
    .flatten()
    .filter(|&c| c != (*x,*y,*z,*w))
    .into_iter().collect::<HashSet<_>>()
}

fn part_one(actives: &HashSet<Coord>) -> i32 {
    let final_actives = (0..6).fold(actives.clone(), |prev_actives, _| {
        let mut counts = HashMap::<Coord, i32>::new();
        for c in prev_actives.iter() {
            let neighbors = get_neighbors(&c);
            for n in neighbors.iter() {
                if let Some(&v) = counts.get(n) {
                    counts.insert(*n, v+1);
                } else {
                    counts.insert(*n, 1);
                }
            }
        }
        let new_active = counts.iter().filter_map(|(coord, &count)| {
            if prev_actives.contains(coord) {
                if 2 <= count && count <= 3 {
                    Some(coord.clone())
                } else {
                    None
                }
            } else {
                if count == 3 {
                    Some(coord.clone())
                } else {
                    None
                }
            }
        }).collect::<HashSet<_>>();

        new_active

    });

    final_actives.len() as i32
}

fn part_two(actives: &HashSet<Coord4d>) -> i32 {
    let final_actives = (0..6).fold(actives.clone(), |prev_actives, _| {
        let mut counts = HashMap::<Coord4d, i32>::new();
        for c in prev_actives.iter() {
            let neighbors = get_neighbors4d(&c);
            for n in neighbors.iter() {
                if let Some(&v) = counts.get(n) {
                    counts.insert(*n, v+1);
                } else {
                    counts.insert(*n, 1);
                }
            }
        }
        let new_active = counts.iter().filter_map(|(coord, &count)| {
            if prev_actives.contains(coord) {
                if 2 <= count && count <= 3 {
                    Some(coord.clone())
                } else {
                    None
                }
            } else {
                if count == 3 {
                    Some(coord.clone())
                } else {
                    None
                }
            }
        }).collect::<HashSet<_>>();

        new_active

    });

    final_actives.len() as i32
}

fn main() {
    println!("Hello, world!");
    let actives = input_str
        .lines()
        .map(|l| l.trim())
        .enumerate()
        .map(|(li, l)| {
            l.chars()
                .enumerate()
                .map(|(ci, c)| match c {
                    '#' => Some((li as i32, ci as i32, 0 as i32)),
                    _ => None,
                })
                .filter(|v| v.is_some())
                .map(|o| o.unwrap())
                .collect::<HashSet<_>>()
        })
        .flatten()
        .collect::<HashSet<_>>();

    
    let result1 = part_one(&actives);
    println!("part1: {:?}", result1);

    let actives2 = actives.iter().map(|(x,y,z)| (*x,*y,*z,0)).collect::<HashSet<_>>();
    let result2 = part_two(&actives2);
    println!("part2: {:?}", result2);

}

static example_str: &str =
    ".#.
    ..#
    ###";

static input_str: &str =
    ".##.##..
    ..###.##
    .##....#
    ###..##.
    #.###.##
    .#.#..#.
    .......#
    .#..#..#";

#[cfg(test)]
mod tests {
    use super::*;
    
    #[test]
    fn test_neighbors() {
        let neighbors = get_neighbors(&(1,1,2));
        assert_eq!(neighbors.len(), 26);
    }

    #[test]
    fn test_neighbors_4d() {
        let neighbors = get_neighbors4d(&(1,1,2,3));
        assert_eq!(neighbors.len(), 80);
    }
}
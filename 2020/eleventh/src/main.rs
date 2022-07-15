#[derive(Debug, Clone)]
pub enum Coord {
    Floor,
    Empty,
    Occupied,
}

mod part_one {
    use crate::*;
    fn n_occupied_neighbours((y, x): &(usize, usize), room: &Vec<Vec<Coord>>) -> i32 {
        let y: i32 = *y as i32;
        let x: i32 = *x as i32;

        let is_occupied = |&(y, x): &(i32, i32)| {
            if y < 0 || (room.len() as i32 <= y) {
                return 0;
            };
            if x < 0 || (room[0].len() as i32 <= x) {
                return 0;
            };
            if let Coord::Occupied = &room[y as usize][x as usize] {
                return 1;
            }
            return 0;
        };

        vec![
            (y - 1, x - 1),
            (y - 1, x),
            (y - 1, x + 1),
            (y, x - 1),
            (y, x + 1),
            (y + 1, x - 1),
            (y + 1, x),
            (y + 1, x + 1),
        ]
        .iter()
        .map(is_occupied)
        .sum()
    }

    pub fn run(room: &Vec<Vec<Coord>>) -> i32 {
        let mut changed = false;
        let mut n_occupied = 0;
        let new_room = (0..room.len())
            .map(|y| {
                (0..room[0].len())
                    .map(|x| match room[y][x] {
                        Coord::Empty => {
                            if n_occupied_neighbours(&(y, x), &room) == 0 {
                                changed = true;
                                n_occupied += 1;
                                Coord::Occupied
                            } else {
                                Coord::Empty
                            }
                        }
                        Coord::Occupied => {
                            if n_occupied_neighbours(&(y, x), &room) >= 4 {
                                changed = true;
                                Coord::Empty
                            } else {
                                n_occupied += 1;
                                Coord::Occupied
                            }
                        }
                        Coord::Floor => Coord::Floor,
                    })
                    .collect::<Vec<_>>()
            })
            .collect::<Vec<_>>();

        if changed {
            run(&new_room)
        } else {
            n_occupied
        }
    }

    #[cfg(test)]
    mod tests {
        use super::*;

        #[test]
        fn n_occupired_test() {
            let room: Vec<Vec<Coord>> = vec![
                vec![
                    Coord::Occupied,
                    Coord::Occupied,
                    Coord::Occupied,
                    Coord::Occupied,
                    Coord::Occupied,
                ],
                vec![
                    Coord::Occupied,
                    Coord::Occupied,
                    Coord::Occupied,
                    Coord::Occupied,
                    Coord::Occupied,
                ],
            ];
            let n = n_occupied_neighbours(&(1, 2), &room);
            assert_eq!(5, n);
        }
    }
}

mod part_two {
    use crate::*;

    fn n_occupied_in_sight((y, x): &(usize, usize), room: &Vec<Vec<Coord>>) -> i32 {
        let y: i32 = *y as i32;
        let x: i32 = *x as i32;

        let is_occupied = |&(dy, dx): &(i32, i32)| {
            let mut cy = y;
            let mut cx = x;
            loop {
                cy += dy;
                cx += dx;

                if !(0 <= cy && cy < room.len() as i32 && 0 <= cx && cx < room[0].len() as i32) {
                    return 0;
                }

                if let Coord::Empty = &room[cy as usize][cx as usize] {
                    return 0;
                } else if let Coord::Occupied = &room[cy as usize][cx as usize] {
                    return 1;
                };
            }
        };

        vec![
            (-1, -1),
            (-1, 0),
            (-1, 1),
            (0, -1),
            (0, 1),
            (1, -1),
            (1, 0),
            (1, 1),
        ]
        .iter()
        .map(is_occupied)
        .sum()
    }

    pub fn run(room: &Vec<Vec<Coord>>) -> i32 {
        let mut changed = false;
        let mut n_occupied = 0;
        let new_room = (0..room.len())
            .map(|y| {
                (0..room[0].len())
                    .map(|x| match room[y][x] {
                        Coord::Empty => {
                            if n_occupied_in_sight(&(y, x), &room) == 0 {
                                changed = true;
                                n_occupied += 1;
                                Coord::Occupied
                            } else {
                                Coord::Empty
                            }
                        }
                        Coord::Occupied => {
                            if n_occupied_in_sight(&(y, x), &room) >= 5 {
                                changed = true;
                                Coord::Empty
                            } else {
                                n_occupied += 1;
                                Coord::Occupied
                            }
                        }
                        Coord::Floor => Coord::Floor,
                    })
                    .collect::<Vec<_>>()
            })
            .collect::<Vec<_>>();

        if changed {
            run(&new_room)
        } else {
            n_occupied
        }
    }

    #[cfg(test)]
    mod tests {
        use super::*;

        #[test]
        fn n_occupired_in_sighttest() {
            let room: Vec<Vec<Coord>> = vec![
                vec![
                    Coord::Occupied,
                    Coord::Occupied,
                    Coord::Occupied,
                    Coord::Occupied,
                    Coord::Occupied,
                ],
                vec![
                    Coord::Empty,
                    Coord::Floor,
                    Coord::Floor,
                    Coord::Occupied,
                    Coord::Occupied,
                ],
            ];
            let n = n_occupied_in_sight(&(1, 3), &room);
            assert_eq!(4, n);
        }
    }
}

fn main() {
    let room = std::fs::read_to_string("./data.txt")
        .unwrap()
        .lines()
        .map(|l| {
            l.chars()
                .map(|c| match c {
                    '.' => Coord::Floor,
                    'L' => Coord::Empty,
                    '#' => Coord::Occupied,
                    _ => unreachable!("unexpceded input"),
                })
                .collect::<Vec<_>>()
        })
        .collect::<Vec<_>>();

    let result1 = part_one::run(&room);
    println!("{:#?}", result1);

    let result2 = part_two::run(&room);
    println!("{:#?}", result2);
}

use regex::Regex;

#[derive(Debug, Clone)]
enum Dir {
    E,
    SE,
    SW,
    W,
    NW,
    NE
}

fn parse_traj(str: &String) -> Vec<Dir> {
    let mut traj: Vec<Dir> = vec![];
    let mut iter = str.chars();
    let mut c = iter.next();

    loop {
        match c {
            Some('e') => {
                traj.push(Dir::E);
                c = iter.next();
            },
            Some('w') => {
                traj.push(Dir::W);
                c = iter.next();
            },
            Some('n') => {
                c = iter.next();
                match c.unwrap() {
                    'e' => traj.push(Dir::NE),
                    'w' => traj.push(Dir::NW),
                    _ => panic!("unknown char after n")
                }
                c = iter.next();
            },
            Some('s') => {
                c = iter.next();
                match c.unwrap() {
                    'e' => traj.push(Dir::SE),
                    'w' => traj.push(Dir::SW),
                    _ => panic!("unknown char after s")
                }
                c = iter.next();
            },
            Some(_) => panic!("unknown character"),
            None => break
        }
    }
    traj
}


//false -> white
//true -> black
fn part_one(trajs: &Vec<Vec<Dir>>) -> Vec<Vec<bool>> {
    let mut floor = (0..1000).map(|_| (0..1000).map(|_| false).collect::<Vec<_>>()).collect::<Vec<_>>();

    for t in trajs.iter() {
        let mut current_y = 500;
        let mut current_x = 500;
        for d in t.iter() {
            match d {
                Dir::E => current_x += 1,
                Dir::W => current_x -= 1,
                Dir::NE => {
                    current_y -= 1;
                    current_x += 1;
                },
                Dir::NW => {
                    current_y -= 1;
                },
                Dir::SE => {
                    current_y += 1;
                },
                Dir::SW => {
                    current_y += 1;
                    current_x -= 1;
                }
            }
        }
        floor[current_y][current_x] = !floor[current_y][current_x];
    }

    floor
}

fn num_black_neighbors(floor: &Vec<Vec<bool>>, (y,x): (usize, usize)) -> usize {
    floor[y+1][x] as usize
    + floor[y][x+1] as usize
    + floor[y-1][x+1] as usize
    + floor[y-1][x] as usize
    + floor[y][x-1] as usize
    + floor[y+1][x-1] as usize
}

fn part_two(floor: &Vec<Vec<bool>>) -> Vec<Vec<bool>> {
    (0..100).fold(floor.clone(), |prev_floor, _| {
        let mut new_floor = (0..1000).map(|_| (0..1000).map(|_| false).collect::<Vec<_>>()).collect::<Vec<_>>();
        for y in 1..prev_floor.len()-1 {
            for x in 1..prev_floor.len()-1 {
                let black_neighbors = num_black_neighbors(&prev_floor, (y,x));
                match prev_floor[y][x] {
                    true => { //    black
                        new_floor[y][x] =
                            if black_neighbors == 0 || black_neighbors > 2 {
                                false
                            } else {
                                true
                            };
                    },
                    false => {     //white
                        new_floor[y][x] =
                            if black_neighbors == 2 {
                                true
                            } else {
                                false
                            };
                    }
                }
            }
        }
        new_floor
    })
}

fn main() {
    let lines = std::fs::read_to_string("./data.txt").unwrap().lines().map(|s| s.to_string()).collect::<Vec<_>>();
    let trajs: Vec<Vec<Dir>> = lines.iter().map(parse_traj).collect();
    //println!("{:#?}", trajs);

    let floor = part_one(&trajs);
    let result1: usize = floor.iter().map(|r| r.iter().filter(|&t| *t).collect::<Vec<_>>().len()).sum();
    println!("{}", result1);

    let floor2 = part_two(&floor);
    let result2: usize = floor2.iter().map(|r| r.iter().filter(|&t| *t).collect::<Vec<_>>().len()).sum();
    println!("{}", result2);
}

use std::collections::{HashMap, HashSet};

// 50254 too high
// 7567 too high

fn main() {
    let filepath = "input";
    let parsed_board = parse(&format!("./{filepath}.txt"));
    let path = parse_path(&format!("./{filepath}path.txt"));
    let start_x = parsed_board
        .iter()
        .filter(|(pc, _)| pc.y == 0)
        .map(|(pc, _)| pc.x)
        .min()
        .unwrap();
    let start_state = State {
        point: Point { x: start_x, y: 0 },
        rotation: Rotation::Right,
    };

    let (final_password1, _) = run_password(&Board::new(parsed_board.clone()), &start_state, &path);
    println!("Part 1: {}", final_password1);
    let board2 = Board::new2(parsed_board);
    let (final_password2, path_went) = run_password(&board2, &start_state, &path);
    println!("Part 2: {}", final_password2);
    //draw(&board2, &path_went);
}

fn run_password(board: &Board, start: &State, path: &Vec<Action>) -> (i64, HashSet<State>) {
    let mut current_state = start.clone();
    let mut path_went = HashSet::<State>::new();

    for action in path.iter() {
        match action {
            Action::RotateClockwise => {
                current_state.rotation = rotate(&current_state.rotation, &action)
            }
            Action::RotateCounterClockwise => {
                current_state.rotation = rotate(&current_state.rotation, &action)
            }
            Action::Forward(n_steps) => {
                current_state =
                    move_forward(&board, &current_state, n_steps.clone(), &mut path_went)
            }
        }
    }
    return (
        1000 * (current_state.point.y + 1)
            + 4 * (current_state.point.x + 1)
            + rotation_score(&current_state.rotation),
        path_went,
    );
}

fn draw(board: &Board, states: &HashSet<State>) {
    let max_x = board.board.iter().map(|(p, _)| p.x).max().unwrap();
    let max_y = board.board.iter().map(|(p, _)| p.y).max().unwrap();

    for y in 0..=max_y {
        for x in 0..=max_x {
            let p = Point { y: y, x: x };
            if let Some(state) = states.iter().find(|s| s.point == p) {
                print!(
                    "{}",
                    match state.rotation {
                        Rotation::Left => "<",
                        Rotation::Up => "^",
                        Rotation::Right => ">",
                        Rotation::Down => "v",
                    }
                );
            } else if let Some(t) = board.board.get(&p) {
                if t == &Thing::Open {
                    print!(".");
                } else {
                    print!("#");
                }
            } else {
                print!(" ");
            }
        }
        println!("");
    }
}

fn move_forward(
    board: &Board,
    state: &State,
    n_steps: i64,
    path_went: &mut HashSet<State>,
) -> State {
    let mut new_state = state.clone();
    for _ in 0..n_steps {
        path_went.insert(new_state.clone());
        let candidate_state = board
            .edges
            .get(&new_state.point)
            .unwrap()
            .get(&new_state.rotation)
            .expect(&format!(
                "Expected edge but found none. position: {:#?}",
                new_state
            ));
        if board.board.get(&candidate_state.point).unwrap() == &Thing::Open {
            new_state = candidate_state.clone();
        } else {
            return new_state;
        }
    }
    path_went.insert(new_state.clone());
    return new_state;
}

fn rotate(current_rotation: &Rotation, action: &Action) -> Rotation {
    match (current_rotation, action) {
        (Rotation::Left, Action::RotateClockwise) => Rotation::Up,
        (Rotation::Left, Action::RotateCounterClockwise) => Rotation::Down,
        (Rotation::Up, Action::RotateClockwise) => Rotation::Right,
        (Rotation::Up, Action::RotateCounterClockwise) => Rotation::Left,
        (Rotation::Right, Action::RotateClockwise) => Rotation::Down,
        (Rotation::Right, Action::RotateCounterClockwise) => Rotation::Up,
        (Rotation::Down, Action::RotateClockwise) => Rotation::Left,
        (Rotation::Down, Action::RotateCounterClockwise) => Rotation::Right,
        _ => panic!("Expected rotation but didnt get it"),
    }
}

fn rotation_score(rotation: &Rotation) -> i64 {
    match rotation {
        Rotation::Right => 0,
        Rotation::Down => 1,
        Rotation::Left => 2,
        Rotation::Up => 3,
    }
}

fn parse_path(filepath: &str) -> Vec<Action> {
    let path = std::fs::read_to_string(filepath).unwrap();
    let mut actions: Vec<Action> = vec![];
    let char_vec = path.chars().collect::<Vec<_>>();
    let mut current_index: usize = 0;
    loop {
        if current_index >= char_vec.len() {
            break;
        }
        if char_vec[current_index] == 'L' {
            actions.push(Action::RotateCounterClockwise);
            current_index += 1;
        } else if char_vec[current_index] == 'R' {
            actions.push(Action::RotateClockwise);
            current_index += 1;
        } else {
            for i in current_index..char_vec.len() {
                if i + 1 == char_vec.len() || char_vec[i + 1] == 'R' || char_vec[i + 1] == 'L' {
                    let number = char_vec[current_index..=i]
                        .iter()
                        .collect::<String>()
                        .parse::<i64>()
                        .unwrap();
                    actions.push(Action::Forward(number));
                    current_index += (i + 1) - current_index;
                    break;
                }
            }
        }
    }
    return actions;
}

fn parse(filepath: &str) -> HashMap<Point, Thing> {
    let input = std::fs::read_to_string(filepath).unwrap();
    let mut board: HashMap<Point, Thing> = HashMap::new();
    for (y, line) in input.lines().enumerate() {
        let line_chars = line.chars().collect::<Vec<_>>();
        for x in 0..line.len() {
            if line_chars[x] == '.' {
                board.insert(
                    Point {
                        x: x as i64,
                        y: y as i64,
                    },
                    Thing::Open,
                );
            } else if line_chars[x] == '#' {
                board.insert(
                    Point {
                        x: x as i64,
                        y: y as i64,
                    },
                    Thing::Wall,
                );
            }
        }
    }
    return board;
}

#[derive(Clone, Debug, Eq, PartialEq)]
struct Board {
    board: HashMap<Point, Thing>,
    edges: HashMap<Point, HashMap<Rotation, State>>,
}

#[derive(Clone, Debug, Eq, PartialEq, Hash)]
struct State {
    point: Point,
    rotation: Rotation,
}

impl Board {
    fn new(tiles: HashMap<Point, Thing>) -> Board {
        let max_x = tiles.iter().map(|(p, _)| p.x).max().unwrap();
        let max_y = tiles.iter().map(|(p, _)| p.y).max().unwrap();

        let mut all_edges: HashMap<Point, HashMap<Rotation, State>> = HashMap::new();

        for y in 0..=max_y {
            for x in 0..=max_x {
                let current_point = Point { y: y, x: x };

                let mut edges: HashMap<Rotation, State> = HashMap::new();
                let left_point = Point {
                    y: current_point.y,
                    x: current_point.x - 1,
                };
                edges.insert(
                    Rotation::Left,
                    if tiles.contains_key(&left_point) {
                        State {
                            point: left_point,
                            rotation: Rotation::Left,
                        }
                    } else {
                        State {
                            point: Point {
                                y: current_point.y,
                                x: tiles
                                    .iter()
                                    .filter(|(pc, _)| pc.y == y)
                                    .map(|(pc, _)| pc.x)
                                    .max()
                                    .unwrap(),
                            },
                            rotation: Rotation::Left,
                        }
                    },
                );

                let up_point = Point {
                    y: current_point.y - 1,
                    x: current_point.x,
                };
                edges.insert(
                    Rotation::Up,
                    if tiles.contains_key(&up_point) {
                        State {
                            point: up_point,
                            rotation: Rotation::Up,
                        }
                    } else {
                        State {
                            point: Point {
                                y: tiles
                                    .iter()
                                    .filter(|(pc, _)| pc.x == x)
                                    .map(|(pc, _)| pc.y)
                                    .max()
                                    .unwrap(),
                                x: current_point.x,
                            },
                            rotation: Rotation::Up,
                        }
                    },
                );

                let right_point = Point {
                    y: current_point.y,
                    x: current_point.x + 1,
                };
                edges.insert(
                    Rotation::Right,
                    if tiles.contains_key(&right_point) {
                        State {
                            point: right_point,
                            rotation: Rotation::Right,
                        }
                    } else {
                        State {
                            point: Point {
                                y: current_point.y,
                                x: tiles
                                    .iter()
                                    .filter(|(pc, _)| pc.y == y)
                                    .map(|(pc, _)| pc.x)
                                    .min()
                                    .unwrap(),
                            },
                            rotation: Rotation::Right,
                        }
                    },
                );

                let down_point = Point {
                    y: current_point.y + 1,
                    x: current_point.x,
                };
                edges.insert(
                    Rotation::Down,
                    if tiles.contains_key(&down_point) {
                        State {
                            point: down_point,
                            rotation: Rotation::Down,
                        }
                    } else {
                        State {
                            point: Point {
                                y: tiles
                                    .iter()
                                    .filter(|(pc, _)| pc.x == x)
                                    .map(|(pc, _)| pc.y)
                                    .min()
                                    .unwrap(),
                                x: current_point.x,
                            },
                            rotation: Rotation::Down,
                        }
                    },
                );
                all_edges.insert(current_point, edges);
            }
        }

        return Board {
            board: tiles,
            edges: all_edges,
        };
    }

    fn new2(tiles: HashMap<Point, Thing>) -> Board {
        let face_size = 50;
        let top = tiles
            .iter()
            .filter(|(p, _)| {
                face_size <= p.x && p.x < face_size * 2 && face_size <= p.y && p.y < face_size * 2
            })
            .map(|(p, _)| p.clone())
            .collect::<HashSet<_>>();
        let posterior = tiles
            .iter()
            .filter(|(p, _)| face_size <= p.x && p.x < face_size * 2 && p.y < face_size)
            .map(|(p, _)| p.clone())
            .collect::<HashSet<_>>();
        let right = tiles
            .iter()
            .filter(|(p, _)| face_size * 2 <= p.x && p.y < face_size)
            .map(|(p, _)| p.clone())
            .collect::<HashSet<_>>();
        let front = tiles
            .iter()
            .filter(|(p, _)| {
                face_size <= p.x
                    && p.x < face_size * 2
                    && face_size * 2 <= p.y
                    && p.y < face_size * 3
            })
            .map(|(p, _)| p.clone())
            .collect::<HashSet<_>>();
        let left = tiles
            .iter()
            .filter(|(p, _)| p.x < face_size && face_size * 2 <= p.y && p.y < face_size * 3)
            .map(|(p, _)| p.clone())
            .collect::<HashSet<_>>();
        let bottom = tiles
            .iter()
            .filter(|(p, _)| p.x < face_size && face_size * 3 <= p.y)
            .map(|(p, _)| p.clone())
            .collect::<HashSet<_>>();

        let mut all_edges: HashMap<Point, HashMap<Rotation, State>> = HashMap::new();

        /*
            TOP
        */
        for current_point in top.iter() {
            let xoffset = current_point.x % face_size;
            let yoffset = current_point.y % face_size;

            let mut edges: HashMap<Rotation, State> = HashMap::new();
            let left_point = Point {
                y: current_point.y,
                x: current_point.x - 1,
            };
            if tiles.contains_key(&left_point) {
                edges.insert(
                    Rotation::Left,
                    State {
                        point: left_point,
                        rotation: Rotation::Left,
                    },
                );
            } else {
                edges.insert(
                    Rotation::Left,
                    State {
                        point: Point {
                            y: left.iter().map(|p| p.y).min().unwrap(),
                            x: left.iter().map(|p| p.x).min().unwrap() + yoffset,
                        },
                        rotation: Rotation::Down,
                    },
                );
            }

            let up_point = Point {
                y: current_point.y - 1,
                x: current_point.x,
            };
            if tiles.contains_key(&up_point) {
                edges.insert(
                    Rotation::Up,
                    State {
                        point: up_point,
                        rotation: Rotation::Up,
                    },
                );
            } else {
                panic!("unexpected non-existance top up");
            }

            let right_point = Point {
                y: current_point.y,
                x: current_point.x + 1,
            };
            if tiles.contains_key(&right_point) {
                edges.insert(
                    Rotation::Right,
                    State {
                        point: right_point,
                        rotation: Rotation::Right,
                    },
                );
            } else {
                edges.insert(
                    Rotation::Right,
                    State {
                        point: Point {
                            y: right.iter().map(|p| p.y).max().unwrap(),
                            x: right.iter().map(|p| p.x).min().unwrap() + yoffset,
                        },
                        rotation: Rotation::Up,
                    },
                );
            }

            let down_point = Point {
                y: current_point.y + 1,
                x: current_point.x,
            };
            if tiles.contains_key(&down_point) {
                edges.insert(
                    Rotation::Down,
                    State {
                        point: down_point,
                        rotation: Rotation::Down,
                    },
                );
            } else {
                panic!("unexpected nopn-existance at top down");
            }
            all_edges.insert(current_point.clone(), edges);
        }

        /*
            FRONT
        */
        for current_point in front.iter() {
            let xoffset = current_point.x % face_size;
            let yoffset = current_point.y % face_size;

            let mut edges: HashMap<Rotation, State> = HashMap::new();
            let left_point = Point {
                y: current_point.y,
                x: current_point.x - 1,
            };
            if tiles.contains_key(&left_point) {
                edges.insert(
                    Rotation::Left,
                    State {
                        point: left_point,
                        rotation: Rotation::Left,
                    },
                );
            } else {
                panic!("Unexpewcted non-existance front-left");
            }

            let up_point = Point {
                y: current_point.y - 1,
                x: current_point.x,
            };
            if tiles.contains_key(&up_point) {
                edges.insert(
                    Rotation::Up,
                    State {
                        point: up_point,
                        rotation: Rotation::Up,
                    },
                );
            } else {
                panic!("unexpected non-existance top up");
            }

            let right_point = Point {
                y: current_point.y,
                x: current_point.x + 1,
            };
            if tiles.contains_key(&right_point) {
                edges.insert(
                    Rotation::Right,
                    State {
                        point: right_point,
                        rotation: Rotation::Right,
                    },
                );
            } else {
                edges.insert(
                    Rotation::Right,
                    State {
                        point: Point {
                            y: right.iter().map(|p| p.y).max().unwrap() - yoffset,
                            x: right.iter().map(|p| p.x).max().unwrap(),
                        },
                        rotation: Rotation::Left,
                    },
                );
            }

            let down_point = Point {
                y: current_point.y + 1,
                x: current_point.x,
            };
            if tiles.contains_key(&down_point) {
                edges.insert(
                    Rotation::Down,
                    State {
                        point: down_point,
                        rotation: Rotation::Down,
                    },
                );
            } else {
                edges.insert(
                    Rotation::Down,
                    State {
                        point: Point {
                            y: bottom.iter().map(|p| p.y).min().unwrap() + xoffset,
                            x: bottom.iter().map(|p| p.x).max().unwrap(),
                        },
                        rotation: Rotation::Left,
                    },
                );
            }

            all_edges.insert(current_point.clone(), edges);
        }

        /*
            LEFT
        */
        for current_point in left.iter() {
            let xoffset = current_point.x % face_size;
            let yoffset = current_point.y % face_size;

            let mut edges: HashMap<Rotation, State> = HashMap::new();
            let left_point = Point {
                y: current_point.y,
                x: current_point.x - 1,
            };
            if tiles.contains_key(&left_point) {
                edges.insert(
                    Rotation::Left,
                    State {
                        point: left_point,
                        rotation: Rotation::Left,
                    },
                );
            } else {
                edges.insert(
                    Rotation::Left,
                    State {
                        point: Point {
                            y: posterior.iter().map(|p| p.y).max().unwrap() - yoffset,
                            x: posterior.iter().map(|p| p.x).min().unwrap(),
                        },
                        rotation: Rotation::Right,
                    },
                );
            }

            let up_point = Point {
                y: current_point.y - 1,
                x: current_point.x,
            };
            if tiles.contains_key(&up_point) {
                edges.insert(
                    Rotation::Up,
                    State {
                        point: up_point,
                        rotation: Rotation::Up,
                    },
                );
            } else {
                edges.insert(
                    Rotation::Up,
                    State {
                        point: Point {
                            y: top.iter().map(|p| p.y).min().unwrap() + xoffset,
                            x: top.iter().map(|p| p.x).min().unwrap(),
                        },
                        rotation: Rotation::Right,
                    },
                );
            }

            let right_point = Point {
                y: current_point.y,
                x: current_point.x + 1,
            };
            if tiles.contains_key(&right_point) {
                edges.insert(
                    Rotation::Right,
                    State {
                        point: right_point,
                        rotation: Rotation::Right,
                    },
                );
            } else {
                panic!("Unreacable left right");
            }

            let down_point = Point {
                y: current_point.y + 1,
                x: current_point.x,
            };
            if tiles.contains_key(&down_point) {
                edges.insert(
                    Rotation::Down,
                    State {
                        point: down_point,
                        rotation: Rotation::Down,
                    },
                );
            } else {
                panic!("Unreacable left down");
            }

            all_edges.insert(current_point.clone(), edges);
        }

        /*
            BOTTOM
        */
        for current_point in bottom.iter() {
            let xoffset = current_point.x % face_size;
            let yoffset = current_point.y % face_size;

            let mut edges: HashMap<Rotation, State> = HashMap::new();
            let left_point = Point {
                y: current_point.y,
                x: current_point.x - 1,
            };
            if tiles.contains_key(&left_point) {
                edges.insert(
                    Rotation::Left,
                    State {
                        point: left_point,
                        rotation: Rotation::Left,
                    },
                );
            } else {
                edges.insert(
                    Rotation::Left,
                    State {
                        point: Point {
                            y: posterior.iter().map(|p| p.y).min().unwrap(),
                            x: posterior.iter().map(|p| p.x).min().unwrap() + yoffset,
                        },
                        rotation: Rotation::Down,
                    },
                );
            }

            let up_point = Point {
                y: current_point.y - 1,
                x: current_point.x,
            };
            if tiles.contains_key(&up_point) {
                edges.insert(
                    Rotation::Up,
                    State {
                        point: up_point,
                        rotation: Rotation::Up,
                    },
                );
            } else {
                panic!("unreachable bottom up");
            }

            let right_point = Point {
                y: current_point.y,
                x: current_point.x + 1,
            };
            if tiles.contains_key(&right_point) {
                edges.insert(
                    Rotation::Right,
                    State {
                        point: right_point,
                        rotation: Rotation::Right,
                    },
                );
            } else {
                edges.insert(
                    Rotation::Right,
                    State {
                        point: Point {
                            y: front.iter().map(|p| p.y).max().unwrap(),
                            x: front.iter().map(|p| p.x).min().unwrap() + yoffset,
                        },
                        rotation: Rotation::Up,
                    },
                );
            }

            let down_point = Point {
                y: current_point.y + 1,
                x: current_point.x,
            };
            if tiles.contains_key(&down_point) {
                edges.insert(
                    Rotation::Down,
                    State {
                        point: down_point,
                        rotation: Rotation::Down,
                    },
                );
            } else {
                edges.insert(
                    Rotation::Down,
                    State {
                        point: Point {
                            y: right.iter().map(|p| p.y).min().unwrap(),
                            x: right.iter().map(|p| p.x).min().unwrap() + xoffset,
                        },
                        rotation: Rotation::Down,
                    },
                );
            }

            all_edges.insert(current_point.clone(), edges);
        }

        /*
            POSTERIOR
        */
        for current_point in posterior.iter() {
            let xoffset = current_point.x % face_size;
            let yoffset = current_point.y % face_size;

            let mut edges: HashMap<Rotation, State> = HashMap::new();
            let left_point = Point {
                y: current_point.y,
                x: current_point.x - 1,
            };
            if tiles.contains_key(&left_point) {
                edges.insert(
                    Rotation::Left,
                    State {
                        point: left_point,
                        rotation: Rotation::Left,
                    },
                );
            } else {
                edges.insert(
                    Rotation::Left,
                    State {
                        point: Point {
                            y: left.iter().map(|p| p.y).max().unwrap() - yoffset,
                            x: left.iter().map(|p| p.x).min().unwrap(),
                        },
                        rotation: Rotation::Right,
                    },
                );
            }

            let up_point = Point {
                y: current_point.y - 1,
                x: current_point.x,
            };
            if tiles.contains_key(&up_point) {
                edges.insert(
                    Rotation::Up,
                    State {
                        point: up_point,
                        rotation: Rotation::Up,
                    },
                );
            } else {
                edges.insert(
                    Rotation::Up,
                    State {
                        point: Point {
                            y: bottom.iter().map(|p| p.y).min().unwrap() + xoffset,
                            x: bottom.iter().map(|p| p.x).min().unwrap(),
                        },
                        rotation: Rotation::Right,
                    },
                );
            }

            let right_point = Point {
                y: current_point.y,
                x: current_point.x + 1,
            };
            if tiles.contains_key(&right_point) {
                edges.insert(
                    Rotation::Right,
                    State {
                        point: right_point,
                        rotation: Rotation::Right,
                    },
                );
            } else {
                panic!("unreachable posterior right");
            }

            let down_point = Point {
                y: current_point.y + 1,
                x: current_point.x,
            };
            if tiles.contains_key(&down_point) {
                edges.insert(
                    Rotation::Down,
                    State {
                        point: down_point,
                        rotation: Rotation::Down,
                    },
                );
            } else {
                panic!("unreachable posterior down");
            }

            all_edges.insert(current_point.clone(), edges);
        }

        /*
            RIGHT
        */
        for current_point in right.iter() {
            let xoffset = current_point.x % face_size;
            let yoffset = current_point.y % face_size;

            let mut edges: HashMap<Rotation, State> = HashMap::new();
            let left_point = Point {
                y: current_point.y,
                x: current_point.x - 1,
            };
            if tiles.contains_key(&left_point) {
                edges.insert(
                    Rotation::Left,
                    State {
                        point: left_point,
                        rotation: Rotation::Left,
                    },
                );
            } else {
                panic!("unreachable right left");
            }

            let up_point = Point {
                y: current_point.y - 1,
                x: current_point.x,
            };
            if tiles.contains_key(&up_point) {
                edges.insert(
                    Rotation::Up,
                    State {
                        point: up_point,
                        rotation: Rotation::Up,
                    },
                );
            } else {
                edges.insert(
                    Rotation::Up,
                    State {
                        point: Point {
                            y: bottom.iter().map(|p| p.y).max().unwrap(),
                            x: bottom.iter().map(|p| p.x).min().unwrap() + xoffset,
                        },
                        rotation: Rotation::Up,
                    },
                );
            }

            let right_point = Point {
                y: current_point.y,
                x: current_point.x + 1,
            };
            if tiles.contains_key(&right_point) {
                edges.insert(
                    Rotation::Right,
                    State {
                        point: right_point,
                        rotation: Rotation::Right,
                    },
                );
            } else {
                edges.insert(
                    Rotation::Right,
                    State {
                        point: Point {
                            y: front.iter().map(|p| p.y).max().unwrap() - yoffset,
                            x: front.iter().map(|p| p.x).max().unwrap(),
                        },
                        rotation: Rotation::Left,
                    },
                );
            }

            let down_point = Point {
                y: current_point.y + 1,
                x: current_point.x,
            };
            if tiles.contains_key(&down_point) {
                edges.insert(
                    Rotation::Down,
                    State {
                        point: down_point,
                        rotation: Rotation::Down,
                    },
                );
            } else {
                edges.insert(
                    Rotation::Down,
                    State {
                        point: Point {
                            y: top.iter().map(|p| p.y).min().unwrap() + xoffset,
                            x: top.iter().map(|p| p.x).max().unwrap(),
                        },
                        rotation: Rotation::Left,
                    },
                );
            }

            all_edges.insert(current_point.clone(), edges);
        }

        return Board {
            board: tiles,
            edges: all_edges,
        };
    }
}

#[derive(Clone, Debug, Eq, PartialEq, Hash)]
struct Point {
    y: i64,
    x: i64,
}

#[derive(Clone, Debug, Eq, PartialEq, Hash)]
enum Thing {
    Open,
    Wall,
}

#[derive(Clone, Debug, Eq, PartialEq, Hash)]
enum Action {
    RotateClockwise,
    RotateCounterClockwise,
    Forward(i64),
}

#[derive(Clone, Debug, Eq, PartialEq, Hash)]
enum Rotation {
    Right,
    Down,
    Left,
    Up,
}

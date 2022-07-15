fn next(state: &Vec<Vec<String>>, char_to_move: &String) -> Vec<Vec<String>> {
    let mut next_state = (0..state.len())
        .map(|_| {
            (0..state[0].len())
                .map(|_| "x".to_string())
                .collect::<Vec<_>>()
        })
        .collect::<Vec<_>>();

    for y in 0..state.len() {
        for x in 0..state[0].len() {
            if &state[y][x] == char_to_move {
                if char_to_move == ">" {
                    if &state[y][(x + 1) % state[0].len()] == "." {
                        next_state[y][(x + 1) % state[0].len()] = ">".to_string();
                        next_state[y][x] = ".".to_string();
                    } else {
                        next_state[y][x] = ">".to_string();
                    }
                }
                if char_to_move == "v" {
                    if &state[(y + 1) % state.len()][x] == "." {
                        next_state[(y + 1) % state.len()][x] = "v".to_string();
                        next_state[y][x] = ".".to_string();
                    } else {
                        next_state[y][x] = "v".to_string();
                    }
                }
            } else if next_state[y][x] == "x".to_string() {
                next_state[y][x] = state[y][x].clone();
            }
        }
    }
    next_state
}

fn eql(state1: &Vec<Vec<String>>, state2: &Vec<Vec<String>>) -> bool {
    for y in 0..state1.len() {
        for x in 0..state1[0].len() {
            if state1[y][x] != state2[y][x] {
                return false;
            }
        }
    }
    return true;
}

fn print(state: &Vec<Vec<String>>) {
    println!("--------------------------------");
    for y in 0..state.len() {
        println!("{:?}", state[y]);
    }
    println!("--------------------------------");
}

fn main() {
    let filedata = std::fs::read_to_string("data.txt").unwrap();
    let mut prev_state = filedata
        .split("\n")
        .map(|line| {
            line.split("")
                .filter(|c| c.len() == 1)
                .map(|c| c.to_string())
                .collect::<Vec<_>>()
        })
        .collect::<Vec<_>>();
    for i in 1..10000 {
        //print(&prev_state);
        let next_state1 = next(&prev_state, &">".to_string());
        let next_state2 = next(&next_state1, &"v".to_string());
        if eql(&prev_state, &next_state2) {
            println!("Equal at: {}", i);
            break;
        }
        prev_state = next_state2;
    }
}

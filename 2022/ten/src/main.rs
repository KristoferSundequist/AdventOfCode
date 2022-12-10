fn main() {
    let instructions = parse_input("input.txt");

    let mut current_state = State {
        x_by_cycle: vec![1],
        screen: vec![],
    };

    for instruction in instructions {
        current_state = do_instruction(&instruction, &current_state);
    }

    get_signal_strength_sum(&current_state.x_by_cycle);
    draw_screen(&current_state.screen);
}

fn get_signal_strength_sum(x_by_cycle: &Vec<i64>) {
    let mut signal_strength_sum = 0;
    for i in 0..x_by_cycle.len() {
        if i == 20 {
            signal_strength_sum += (i as i64) * x_by_cycle[i - 1];
        } else if i > 20 && (i + 20) % 40 == 0 {
            signal_strength_sum += (i as i64) * x_by_cycle[i - 1];
        }
    }
    println!("Part 1: {}", signal_strength_sum);
}

fn draw_screen(screen: &Vec<bool>) {
    for i in 0..screen.len() {
        if i % 40 == 0 {
            print!("\n");
        }
        print!("{}", if screen[i] { "#" } else { "." });
    }
    print!("\n");
}

fn do_instruction(instruction: &Instruction, old_state: &State) -> State {
    let mut new_x_by_cycle = old_state.x_by_cycle.clone();
    let mut crt_position = (old_state.x_by_cycle.len() - 1) % 40;
    let mut new_screen = old_state.screen.clone();
    let current_x = old_state.x_by_cycle.last().unwrap().clone();
    if let Instruction::Add(amount) = instruction {
        if (current_x - crt_position as i64).abs() <= 1 {
            new_screen.push(true);
        } else {
            new_screen.push(false);
        }
        crt_position += 1;
        new_x_by_cycle.push(current_x);
        let new_x = current_x + amount;

        if (current_x - crt_position as i64).abs() <= 1 {
            new_screen.push(true);
        } else {
            new_screen.push(false);
        }

        new_x_by_cycle.push(new_x);

        return State {
            x_by_cycle: new_x_by_cycle,
            screen: new_screen,
        };
    } else {
        new_x_by_cycle.push(current_x);

        if (current_x - crt_position as i64).abs() <= 1 {
            new_screen.push(true);
        } else {
            new_screen.push(false);
        }

        return State {
            x_by_cycle: new_x_by_cycle,
            screen: new_screen,
        };
    }
}

fn parse_input(filepath: &str) -> Vec<Instruction> {
    let input = std::fs::read_to_string(filepath).unwrap();
    return input
        .lines()
        .map(|line| {
            if &line[0..4] == "addx" {
                return Instruction::Add(line[5..].parse::<i64>().unwrap());
            } else if &line[0..4] == "noop" {
                return Instruction::Noop;
            } else {
                panic!("Unexpected instruction when parsing: {}", &line[0..5]);
            }
        })
        .collect();
}

#[derive(Debug, Clone)]
enum Instruction {
    Add(i64),
    Noop,
}

#[derive(Debug, Clone)]
struct State {
    x_by_cycle: Vec<i64>,
    screen: Vec<bool>,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_parse() {
        let instructions = parse_input("./toyinput.txt");
        assert!(matches!(instructions[0], Instruction::Noop));
        assert!(matches!(instructions[1], Instruction::Add(3)));
        assert!(matches!(instructions[2], Instruction::Add(-5)));
    }
}

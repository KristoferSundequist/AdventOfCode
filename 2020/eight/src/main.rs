use std::fs;

#[derive(Debug, Clone)]
struct Instruction {
    instruction: String,
    value: i64,
}

fn part_one(instructions: &Vec<Instruction>) -> i64 {
    let mut visited: Vec<bool> = (0..instructions.len()).map(|_| false).collect();
    let mut current_instruction_index: i32 = 0;
    let mut accumulator: i64 = 0;
    while (current_instruction_index as usize) < instructions.len() {
        if visited[current_instruction_index as usize] {
            return accumulator;
        } else {
            visited[current_instruction_index as usize] = true;
        }
        let current_instruction = &instructions[current_instruction_index as usize];
        match &*current_instruction.instruction {
            "acc" => {
                accumulator += current_instruction.value;
                current_instruction_index += 1;
            }
            "jmp" => {
                current_instruction_index += current_instruction.value as i32;
            }
            "nop" => {
                current_instruction_index += 1;
            }
            _ => unreachable!("unexpected instruction."),
        }
    }
    return 666;
}

fn part_two(instructions: &Vec<Instruction>) -> (bool, i64) {
    let mut visited: Vec<bool> = (0..instructions.len()).map(|_| false).collect();
    let mut current_instruction_index: i32 = 0;
    let mut accumulator: i64 = 0;
    while (current_instruction_index as usize) < instructions.len() {
        if visited[current_instruction_index as usize] {
            return (false, accumulator);
        } else {
            visited[current_instruction_index as usize] = true;
        }
        let current_instruction = &instructions[current_instruction_index as usize];
        match &*current_instruction.instruction {
            "acc" => {
                accumulator += current_instruction.value;
                current_instruction_index += 1;
            }
            "jmp" => {
                current_instruction_index += current_instruction.value as i32;
            }
            "nop" => {
                current_instruction_index += 1;
            }
            _ => unreachable!("unexpected instruction."),
        }
    }
    return (true, accumulator);
}

fn main() {
    let instructions = fs::read_to_string("./data.txt")
        .unwrap()
        .lines()
        .map(|l| {
            let mut qwe = l.split(" ");
            Instruction {
                instruction: qwe.next().unwrap().to_string(),
                value: qwe.next().unwrap().parse::<i64>().unwrap(),
            }
        })
        .collect::<Vec<Instruction>>();

    let result = part_one(&instructions);
    println!("{:#?}", result);

    let all_one_change_instructions: Vec<Vec<Instruction>> =
        (0..instructions.len())
        .filter(|i| vec!["jmp", "nop"].contains(&&*instructions[i.to_owned()].instruction))
        .map(|i| {
            let mut instr = instructions.clone();
            if instr[i].instruction == "jmp" {
                instr[i].instruction = "nop".to_string();
            } else if instr[i].instruction == "nop"  {
                instr[i].instruction = "jmp".to_string();
            } else  {
                unreachable!("unexpctyed instestion when createing one change instraciotns");
            }
            instr
        }).collect();
    
    let result2 = all_one_change_instructions.iter().map(part_two).filter(|(e,_)| *e).collect::<Vec<(bool, i64)>>();
    println!("result 2: --- {:#?}", result2);
}

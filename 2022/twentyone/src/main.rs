use regex::Regex;
use std::collections::HashMap;

fn main() {
    let input = parse("./input.txt");
    let result = calculate(&input);
    println!("Part 1: {}", result);
    let result2 = part2(&input);
    println!("Part 2: {}", result2);
}

fn part2(program: &HashMap<String, Expression>) -> i64 {
    if let Expression::Operation(Operation::Add(v1, v2)) = program.get(&"root".to_string()).unwrap()
    {
        let new_root = Expression::Operation(Operation::Sub(v1.clone(), v2.clone()));
        let mut program2 = program.clone();
        program2.insert("root".to_string(), new_root);
        let mut current_value: i64 = 0;
        let mut last_diff: i64 = 0;
        let mut jump_vector: i64 = 1;
        let mut n_improvements_in_a_row: i64 = 0;

        loop {
            program2.insert("humn".to_string(), Expression::Const(current_value));
            let diff = calculate(&program2);
            if diff == 0 {
                return current_value;
            }
            if diff.abs() > last_diff.abs() {
                n_improvements_in_a_row = 0;
                jump_vector *= -1;
                jump_vector = if (jump_vector / 2).abs() < 1 {
                    jump_vector.signum()
                } else {
                    jump_vector / 2
                }
            } else if n_improvements_in_a_row > 10 {
                jump_vector = jump_vector * 2;
            } else {
                n_improvements_in_a_row += 1;
            }
            current_value += jump_vector;
            last_diff = diff;
        }
    } else {
        panic!("Unexpected root stuff");
    }
}

fn calculate(program: &HashMap<String, Expression>) -> i64 {
    let mut memory = HashMap::<String, i64>::new();
    return calculation_helper(&"root".to_string(), program, &mut memory);
}

fn calculation_helper(
    expr_id: &String,
    program: &HashMap<String, Expression>,
    memory: &mut HashMap<String, i64>,
) -> i64 {
    if let Some(already_calculated_value) = memory.get(expr_id) {
        return already_calculated_value.clone();
    }

    let expr = program.get(expr_id).unwrap();

    let value = match expr {
        Expression::Const(v) => v.clone(),
        Expression::Operation(Operation::Add(v1, v2)) => {
            calculation_helper(v1, program, memory) + calculation_helper(v2, program, memory)
        }
        Expression::Operation(Operation::Mult(v1, v2)) => {
            calculation_helper(v1, program, memory) * calculation_helper(v2, program, memory)
        }
        Expression::Operation(Operation::Sub(v1, v2)) => {
            calculation_helper(v1, program, memory) - calculation_helper(v2, program, memory)
        }
        Expression::Operation(Operation::Div(v1, v2)) => {
            calculation_helper(v1, program, memory) / calculation_helper(v2, program, memory)
        }
    };

    memory.insert(expr_id.clone(), value);
    return value;
}

fn parse(filepath: &str) -> HashMap<String, Expression> {
    let input = std::fs::read_to_string(filepath).unwrap();
    let const_re = Regex::new(r"([\D]+): ([0-9]+)$").unwrap();
    let operation_re = Regex::new(r"([\D]+): ([\D]+) (\+|\-|\*|/) ([\D]+)$").unwrap();

    return input
        .lines()
        .map(|line| {
            let maybe_const_captures = const_re.captures(line);
            if let Some(const_captures) = maybe_const_captures {
                let id = const_captures.get(1).unwrap().as_str().to_string();
                let expr = Expression::Const(
                    const_captures
                        .get(2)
                        .unwrap()
                        .as_str()
                        .parse::<i64>()
                        .unwrap(),
                );
                return (id, expr);
            } else {
                let operation_captures = operation_re.captures(line).unwrap();
                let id = operation_captures.get(1).unwrap().as_str().to_string();
                let value1 = operation_captures.get(2).unwrap().as_str().to_string();
                let value2 = operation_captures.get(4).unwrap().as_str().to_string();
                let op = match operation_captures.get(3).unwrap().as_str() {
                    "*" => Operation::Mult(value1, value2),
                    "+" => Operation::Add(value1, value2),
                    "-" => Operation::Sub(value1, value2),
                    "/" => Operation::Div(value1, value2),
                    _ => panic!("unexpected op"),
                };
                return (id, Expression::Operation(op));
            }
        })
        .collect();
}

#[derive(Debug, Clone, Hash, Eq, PartialEq)]
enum Expression {
    Const(i64),
    Operation(Operation),
}

#[derive(Debug, Clone, Hash, Eq, PartialEq)]
enum Operation {
    Add(String, String),
    Sub(String, String),
    Mult(String, String),
    Div(String, String),
}

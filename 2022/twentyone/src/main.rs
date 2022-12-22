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
        let lower_bound: i64 = 3876027196110;
        let uppper_bound: i64 = lower_bound * 2;
        let mut i: i64 = lower_bound;
        loop {
            if i > uppper_bound {
                panic!("Didnt find it")
            }
            if i % 100000 == 0 {
                println!("{}", (lower_bound + i) as f64 / uppper_bound as f64);
            }
            program2.insert("humn".to_string(), Expression::Const(i));
            let diff = calculate(&program2);
            let diff_num_digits = diff.to_string().len();
            println!("{i}: {diff} - num digits: {diff_num_digits}");
            let num_digit_search = 1;
            if diff_num_digits == num_digit_search {
                panic!("{num_digit_search}");
            }
            if diff == 0 {
                return i;
            }
            i += 1;
        }
    } else {
        panic!("Unexpected root stuff");
    }
    panic!("Didnt find the value");
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

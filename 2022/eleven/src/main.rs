fn main() {
    let monkeys = parse_input("input.txt");

    let mut part1_monkeys = monkeys.clone();
    for _ in 0..20 {
        part1_monkeys = round(&part1_monkeys);
    }
    let mut num_inspections = part1_monkeys
        .iter()
        .map(|m| m.num_inspects)
        .collect::<Vec<_>>();
    num_inspections.sort();
    let reversedinspections = num_inspections.iter().rev().collect::<Vec<_>>();
    println!(
        "Part 1: {}",
        reversedinspections[0] * reversedinspections[1]
    );

    let mut part2_monkeys = monkeys.clone();
    for _ in 0..10000 {
        part2_monkeys = round2(&part2_monkeys);
    }
    let mut num_inspections2 = part2_monkeys
        .iter()
        .map(|m| m.num_inspects)
        .collect::<Vec<_>>();
    num_inspections2.sort();
    let reversedinspections2 = num_inspections2.iter().rev().collect::<Vec<_>>();
    println!(
        "Part 2: {}",
        reversedinspections2[0] * reversedinspections2[1]
    );
}

fn round(monkeys: &Vec<Monkey>) -> Vec<Monkey> {
    let mut new_monkeys = monkeys.clone();
    for i in 0..new_monkeys.len() {
        for item_i in 0..new_monkeys[i].items.len() {
            let worry_level = {
                let item = new_monkeys[i].items[item_i];
                match (&new_monkeys[i].operation, &new_monkeys[i].operation_value) {
                    (Operation::Mult, OperationValue::Const(v)) => item * v,
                    (Operation::Mult, OperationValue::Old) => item * item,
                    (Operation::Add, OperationValue::Const(v)) => item + v,
                    (Operation::Add, OperationValue::Old) => item + item,
                }
            };
            let divided = worry_level / 3;
            let true_monkey_index = new_monkeys[i].true_monkey;
            let false_monkey_index = new_monkeys[i].false_monkey;
            if (divided % new_monkeys[i].test) == 0 {
                new_monkeys[true_monkey_index].items.push(divided);
            } else {
                new_monkeys[false_monkey_index].items.push(divided);
            }
            new_monkeys[i].num_inspects += 1;
        }
        new_monkeys[i].items = vec![];
    }
    return new_monkeys;
}

fn round2(monkeys: &Vec<Monkey>) -> Vec<Monkey> {
    let test_products = monkeys
        .iter()
        .map(|m| m.test)
        .fold(1, |product, v| product * v);
    let mut new_monkeys = monkeys.clone();
    for i in 0..new_monkeys.len() {
        for item_i in 0..new_monkeys[i].items.len() {
            let worry_level = {
                let item = new_monkeys[i].items[item_i];
                match (&new_monkeys[i].operation, &new_monkeys[i].operation_value) {
                    (Operation::Mult, OperationValue::Const(v)) => item * v,
                    (Operation::Mult, OperationValue::Old) => item * item,
                    (Operation::Add, OperationValue::Const(v)) => item + v,
                    (Operation::Add, OperationValue::Old) => item + item,
                }
            };
            let divided = worry_level % test_products;
            let true_monkey_index = new_monkeys[i].true_monkey;
            let false_monkey_index = new_monkeys[i].false_monkey;
            if (divided % new_monkeys[i].test) == 0 {
                new_monkeys[true_monkey_index].items.push(divided);
            } else {
                new_monkeys[false_monkey_index].items.push(divided);
            }
            new_monkeys[i].num_inspects += 1;
        }
        new_monkeys[i].items = vec![];
    }
    return new_monkeys;
}

fn parse_input(filepath: &str) -> Vec<Monkey> {
    let input = std::fs::read_to_string(filepath).unwrap();
    let lines = input.lines().collect::<Vec<_>>();
    lines
        .chunks(7)
        .into_iter()
        .map(|monkey_lines| parse_monkey_lines(monkey_lines))
        .collect::<Vec<_>>()
}

fn parse_monkey_lines(lines: &[&str]) -> Monkey {
    let items = lines[1].split(":").nth(1).unwrap();

    let parsed_items = items
        .split(",")
        .map(|item| item.trim().parse::<i64>().unwrap())
        .collect::<Vec<_>>();

    let right_part_of_operation_line = lines[2].split("=").nth(1).unwrap();

    let operation = match right_part_of_operation_line.chars().nth(5).unwrap() {
        '*' => Operation::Mult,
        '+' => Operation::Add,
        _ => panic!("Unexpecteed operation"),
    };

    let operation_value = match &right_part_of_operation_line[7..] {
        "old" => OperationValue::Old,
        _ => OperationValue::Const(right_part_of_operation_line[7..].parse::<i64>().unwrap()),
    };

    let divisible_by_factor = lines[3]
        .split("by")
        .nth(1)
        .unwrap()
        .trim()
        .parse::<i64>()
        .unwrap();

    let true_monkey = lines[4]
        .split("monkey")
        .nth(1)
        .unwrap()
        .trim()
        .parse::<usize>()
        .unwrap();

    let false_monkey = lines[5]
        .split("monkey")
        .nth(1)
        .unwrap()
        .trim()
        .parse::<usize>()
        .unwrap();

    return Monkey {
        items: parsed_items,
        operation: operation,
        operation_value: operation_value,
        test: divisible_by_factor,
        true_monkey: true_monkey,
        false_monkey: false_monkey,
        num_inspects: 0,
    };
}

#[derive(Debug, Clone)]
struct Monkey {
    items: Vec<i64>,
    operation: Operation,
    operation_value: OperationValue,
    test: i64,
    true_monkey: usize,
    false_monkey: usize,
    num_inspects: i64,
}

#[derive(Debug, Clone)]
enum Operation {
    Mult,
    Add,
}

#[derive(Debug, Clone)]
enum OperationValue {
    Const(i64),
    Old,
}

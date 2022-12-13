use std::cmp::Ordering;

fn main() {
    let parsed_lines = parse("./input.txt");
    part1(&parsed_lines);
    part2(&parsed_lines);
}

fn part2(input: &Vec<(Node, Node)>) {
    let mut sorted_input = input
        .iter()
        .map(|(node1, node2)| vec![node1.clone(), node2.clone()])
        .flatten()
        .collect::<Vec<_>>();
    let two = Node::List(vec![Node::List(vec![Node::Const(2)])]);
    let six = Node::List(vec![Node::List(vec![Node::Const(6)])]);
    sorted_input.push(two.clone());
    sorted_input.push(six.clone());
    sorted_input.sort_by(|node1, node2| match is_correct(node1, node2) {
        1 => Ordering::Less,
        0 => Ordering::Equal,
        -1 => Ordering::Greater,
        _ => panic!("Unexpetced iscorrect answer"),
    });
    let two_location = sorted_input.iter().position(|v| v == &two).unwrap() + 1;
    let six_location = sorted_input.iter().position(|v| v == &six).unwrap() + 1;
    println!("Part 2: {}", two_location * six_location);
}

fn part1(input: &Vec<(Node, Node)>) {
    let is_line_correct = input
        .iter()
        .map(|(node1, node2)| is_correct(node1, node2))
        .collect::<Vec<_>>();

    let mut correct_index_sum = 0;
    for i in 0..is_line_correct.len() {
        if is_line_correct[i] == 1 {
            correct_index_sum += i + 1;
        } else if is_line_correct[i] == 0 {
            panic!("lines are unexpectdedy equal");
        }
    }
    println!("Part 1: {:?}", correct_index_sum);
}

fn is_correct(node1: &Node, node2: &Node) -> i32 {
    match (node1, node2) {
        (Node::Const(v1), Node::Const(v2)) => {
            if v1 < v2 {
                1
            } else if v1 == v2 {
                0
            } else {
                -1
            }
        }
        (Node::List(l1), Node::List(l2)) => is_correct_list(l1, l2),
        (Node::Const(_), Node::List(l2)) => is_correct_list(&vec![node1.clone()], l2),
        (Node::List(l1), Node::Const(_)) => is_correct_list(l1, &vec![node2.clone()]),
    }
}

fn is_correct_list(node_list1: &Vec<Node>, node_list2: &Vec<Node>) -> i32 {
    for i in 0..node_list1.len() {
        if node_list2.len() <= i {
            return -1;
        }
        let is_correct = is_correct(&node_list1[i], &node_list2[i]);
        if is_correct == -1 {
            return -1;
        } else if is_correct == 1 {
            return 1;
        }
    }
    if node_list1.len() == node_list2.len() {
        return 0;
    } else {
        return 1;
    }
}

fn parse(filepath: &str) -> Vec<(Node, Node)> {
    let input = std::fs::read_to_string(filepath).unwrap();
    let lines = input.lines().collect::<Vec<_>>();
    let parsed_lines = lines
        .chunks(3)
        .map(|chunk| {
            (
                parse_line(chunk[0].chars().collect::<Vec<_>>()),
                parse_line(chunk[1].chars().collect::<Vec<_>>()),
            )
        })
        .collect::<Vec<_>>();
    return parsed_lines;
}

fn parse_line(line: Vec<char>) -> Node {
    let (node, parsed_chars) = parse_node(&line, 0);
    if parsed_chars != line.len() {
        panic!(
            "parsed_chars wasnt equal to line length, line_length: {}, parsed_chars: {}",
            line.len(),
            parsed_chars
        );
    }
    return node;
}

// returns a node and number of chars read
fn parse_node(line: &Vec<char>, start_index: usize) -> (Node, usize) {
    if line[start_index] == '[' {
        let mut current_index = start_index + 1;
        let mut list_nodes = vec![];
        loop {
            if line[current_index] == ']' {
                current_index += 1;
                break;
            } else if line[current_index] == ',' {
                let (new_node, new_index) = parse_node(&line, current_index + 1);
                current_index = new_index;
                list_nodes.push(new_node);
            } else {
                let (new_node, new_index) = parse_node(&line, current_index);
                current_index = new_index;
                list_nodes.push(new_node);
            }
        }
        return (Node::List(list_nodes), current_index);
    } else if line[start_index] == ']' {
        panic!("Unexpected ] at index {}", start_index);
    } else if line[start_index] == ',' {
        panic!("Unexpected , at index {}", start_index);
    } else {
        // it should be a number

        if let Ok(two_digit_number) = line[start_index..start_index + 2]
            .iter()
            .collect::<String>()
            .parse::<i32>()
        {
            return (Node::Const(two_digit_number), start_index + 2);
        } else if let Ok(one_digit_number) = line[start_index..start_index + 1]
            .iter()
            .collect::<String>()
            .parse::<i32>()
        {
            return (Node::Const(one_digit_number), start_index + 1);
        } else {
            panic!("Unexpected not number at index {}", start_index);
        }
    }
}

#[derive(Debug, Clone, Eq, PartialEq)]
enum Node {
    Const(i32),
    List(Vec<Node>),
}

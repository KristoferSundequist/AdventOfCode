use std::collections::{HashSet, HashMap};

#[derive(Debug, Clone)]
enum Rule {
    Character(char),
    Redirect(Vec<Vec<i32>>),
}

//1: 2 3 | 3 2
fn get_rules(str: &String) -> (usize, Rule) {
    let left: usize = str.split(":").nth(0).unwrap().parse().unwrap();
    let right = str.split(":").nth(1).unwrap();
    if right.chars().nth(1).unwrap().is_digit(10) {
        let parts = right.split("|");
        let rules = parts
            .map(|r| {
                r.split(" ")
                    .filter(|v| v.len() > 0)
                    .map(|n| n.parse::<i32>().unwrap())
                    .collect::<Vec<_>>()
            })
            .collect::<Vec<_>>();
        (left, Rule::Redirect(rules))
    } else {
        (left, Rule::Character(right.chars().nth(2).unwrap()))
    }
}

fn msg_is_match(
    rule: usize,
    rules: &HashMap<usize, Rule>,
    msgs: &Vec<String>,
    visited: &HashSet<(usize, String)>,
) -> Vec<String> {
    let mut new_visited = visited.clone();
    msgs.iter()
        .map(|msg| {
            if visited.contains(&(rule, msg.clone())) {
                return vec![];
            } else {
                new_visited.insert((rule, msg.clone()));
            }

            match rules.get(&rule).unwrap().clone() {
                Rule::Character(c) => {
                    if let Some(next) = msg.chars().nth(0) {
                        if next == c {
                            vec![msg[1..].to_string()]
                        } else {
                            vec![]
                        }
                    } else {
                        vec![]
                    }
                },
                Rule::Redirect(sub_rules) => {
                    let rems: Vec<String> = sub_rules
                        .iter()
                        .map(|rule_seq| {
                            rule_seq.iter().fold(vec![msg.clone()], |rem, &r| {
                                msg_is_match(r as usize, rules, &rem, &new_visited)
                            })
                        })
                        .flatten()
                        .collect();
                    rems
                }
            }
        })
        .flatten()
        .collect::<Vec<_>>()
}

fn part_one(rules: &HashMap<usize, Rule>, msgs: &Vec<String>) -> usize {
    let results = msgs
        .iter()
        .map(|msg| msg_is_match(0, rules, &vec![msg.clone()], &HashSet::new()))
        .collect::<Vec<_>>();

    //println!("{:?}", results);
    results
        .iter()
        .filter(|results| results.contains(&"".to_string()))
        .collect::<Vec<_>>()
        .len()
}

fn main() {
    let lines = std::fs::read_to_string("./dataupdated.txt")
        .unwrap()
        .lines()
        .map(|v| v.to_string())
        .collect::<Vec<String>>();

    let mut parts = lines.split(|l| l == "");
    let mut rules: HashMap<usize, Rule> = HashMap::new();
    parts
        .next()
        .unwrap()
        .iter()
        .map(|s| s.to_owned())
        .map(|r| get_rules(&r))
        .for_each(|(k, v)| {rules.insert(k, v); ()});

    let msgs = parts
        .next()
        .unwrap()
        .iter()
        .map(|s| s.to_owned())
        .collect::<Vec<_>>();

    let result1 = part_one(&rules, &msgs);
    println!("{}", result1);
}

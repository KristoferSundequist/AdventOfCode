fn main() {
    let result1: i64 = std::fs::read_to_string("./data.txt").unwrap().lines().map(|l| remove_white_space(l)).map(|l| l.to_string()).map(|l| parse(&l)).sum();
    println!("result1: {:?}", result1);

    let result2: i64 = std::fs::read_to_string("./data.txt").unwrap().lines().map(|l| remove_white_space(l)).map(|l| l.to_string()).map(|l| parse2(&l)).sum();
    println!("result2: {:?}", result2);
}



fn parse_parenthesis(inp: &String) -> (String, String) {
    match inp.chars().nth(0).unwrap() {
        '(' => {
            let mut parenthesis_count = 1;
            for (i, c) in inp.chars().enumerate().skip(1) {
                if c == '(' {
                    parenthesis_count += 1;
                }

                if c == ')' {
                    parenthesis_count -= 1;
                }

                if parenthesis_count == 0 {
                    return (inp[1..i].to_string(), inp[i + 1..].to_string());
                }
            }
            unreachable!("parenthesis didnt end")
        }
        _ => unreachable!("parenthesis error"),
    }
}

fn parse_digit(inp: &String) -> (i64, String) {
    for i in 0..inp.len() {
        if !inp.chars().nth(i).unwrap().is_digit(10) {
            return (inp[0..i].parse::<i64>().unwrap(), inp[i..].to_string());
        }
    }
    if !inp.chars().nth(0).unwrap().is_digit(10) {
        panic!("bad input")
    } else {
        (inp[0..].parse::<i64>().unwrap(), "".to_string())
    }
}

#[derive(Debug)]
enum Operator {
    Add,
    Mult
}

fn parse(inp: &String) -> i64 {
    parse_aux(0, Operator::Add, inp)
}

fn parse_aux(current_value: i64, last_operator: Operator, inp: &String) -> i64 {
    match inp.chars().nth(0) {
        Some('+') => parse_aux(current_value, Operator::Add, &inp[1..].to_string()),
        Some('*') => parse_aux(current_value, Operator::Mult, &inp[1..].to_string()),
        Some('(') => {
            let (inside, rem) = parse_parenthesis(&inp);
            let inside_v = parse_aux(0, Operator::Add, &inside);
            let new_current_value = 
                match last_operator {
                    Operator::Add => current_value + inside_v,
                    Operator::Mult => current_value * inside_v
                };
            parse_aux(new_current_value, last_operator, &rem)
        }
        Some(c) if c.is_digit(10) => {
            let (digit, rem) = parse_digit(&inp);
            let new_current_value = 
                match last_operator {
                    Operator::Add => current_value + digit,
                    Operator::Mult => current_value * digit
                };
            parse_aux(new_current_value, Operator::Add, &rem)
        }
        None => {
            return current_value;
        }
        _ => unreachable!("oqiwne parse unreach"),
    }
}

fn parse2(inp: &String) -> i64 {
    parse_aux2(0, Operator::Add, inp)
}

fn parse_aux2(current_value: i64, last_operator: Operator, inp: &String) -> i64 {
    match inp.chars().nth(0) {
        Some('+') => parse_aux2(current_value, Operator::Add, &inp[1..].to_string()),
        Some('*') => current_value * parse_aux2(0, Operator::Add, &inp[1..].to_string()),
        Some('(') => {
            let (inside, rem) = parse_parenthesis(&inp);
            let inside_v = parse_aux2(0, Operator::Add, &inside);
            let new_current_value = 
                match last_operator {
                    Operator::Add => current_value + inside_v,
                    Operator::Mult => current_value * inside_v
                };
            parse_aux2(new_current_value, last_operator, &rem)
        }
        Some(c) if c.is_digit(10) => {
            let (digit, rem) = parse_digit(&inp);
            let new_current_value = 
                match last_operator {
                    Operator::Add => current_value + digit,
                    Operator::Mult => current_value * digit
                };
            parse_aux2(new_current_value, Operator::Add, &rem)
        }
        None => {
            return current_value;
        }
        _ => unreachable!("oqiwne parse unreach"),
    }
}

fn remove_white_space(str: &str) -> String {
    str.split(" ").collect::<String>()
}


#[cfg(test)]
mod test {

    use super::*;

    #[test]
    fn test_parse_parenthesis() {
        let parsed = parse_parenthesis(&remove_white_space("(4*5)+33"));
        assert_eq!(parsed, ("4*5".to_string(), "+33".to_string()));
    }

    #[test]
    fn test_parse_parenthesis_two() {
        let parsed = parse_parenthesis(&remove_white_space("((4*5))+33"));
        assert_eq!(parsed, ("(4*5)".to_string(), "+33".to_string()));
    }

    #[test]
    #[should_panic]
    fn test_parse_digit() {
        let _ = parse_digit(&remove_white_space("((4*5))+33"));
    }

    #[test]
    fn test_parse_digit_two() {
        let parsed = parse_digit(&remove_white_space("99+((4*5))+33"));
        assert_eq!(parsed, (99, "+((4*5))+33".to_string()));
    }

    #[test]
    fn test_parse_digit_single_char() {
        let parsed = parse_digit(&remove_white_space("3"));
        assert_eq!(parsed, (3, "".to_string()));
    }

    #[test]
    fn test_parse() {
        let input = remove_white_space("99+(4*5)+33");
        let parsed = parse(&input);
        assert_eq!(parsed, 152);
    }

    #[test]
    fn test_parse_1() {
        let input = remove_white_space("2 * 3 + (4 * 5)");
        let parsed = parse(&input);
        assert_eq!(parsed, 26);
    }

    #[test]
    fn test_parse_2() {
        let input = remove_white_space("5 + (8 * 3 + 9 + 3 * 4 * 3)");
        let parsed = parse(&input);
        assert_eq!(parsed, 437);
    }

    #[test]
    fn test_parse_3() {
        let input = remove_white_space("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))");
        let parsed = parse(&input);
        assert_eq!(parsed, 12240);
    }

    #[test]
    fn test_parse_4() {
        let input = remove_white_space("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2");
        let parsed = parse(&input);
        assert_eq!(parsed, 13632);
    }

    ////////// PART 2

    #[test]
    fn test_parse2_0() {
        let input = remove_white_space("1 + (2 * 3) + (4 * (5 + 6))");
        let parsed = parse2(&input);
        assert_eq!(parsed, 51);
    }

    #[test]
    fn test_parse2_1() {
        let input = remove_white_space("2 * 3 + (4 * 5)");
        let parsed = parse2(&input);
        assert_eq!(parsed, 46);
    }

    #[test]
    fn test_parse2_2() {
        let input = remove_white_space("5 + (8 * 3 + 9 + 3 * 4 * 3)");
        let parsed = parse2(&input);
        assert_eq!(parsed, 1445);
    }

    #[test]
    fn test_parse2_3() {
        let input = remove_white_space("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))");
        let parsed = parse2(&input);
        assert_eq!(parsed, 669060);
    }

    #[test]
    fn test_parse2_4() {
        let input = remove_white_space("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2");
        let parsed = parse2(&input);
        assert_eq!(parsed, 23340);
    }
}

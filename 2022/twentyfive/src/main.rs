fn main() {
    let decimal_sum = std::fs::read_to_string("./input.txt")
        .unwrap()
        .lines()
        .map(snafu_to_decimal)
        .sum();
    let sum_as_snafu = decimal_to_snafu(decimal_sum);
    println!("sum snafu: {sum_as_snafu}");
}

fn snafu_to_decimal(str: &str) -> i64 {
    let mut decimal_number: i64 = 0;
    let max_power = str.len() - 1;
    for (i, c) in str.chars().enumerate() {
        let coef = match c {
            '2' => 2,
            '1' => 1,
            '0' => 0,
            '-' => -1,
            '=' => -2,
            _ => panic!("Unexpected char {c} at index {i}"),
        };
        decimal_number += coef * 5_i64.pow((max_power - i) as u32);
    }
    return decimal_number;
}

fn decimal_to_snafu(dec: i64) -> String {
    let max_power = {
        let mut current_max_power = 0;
        for i in 0..10000 {
            if dec.abs() - 5_i64.pow(i) < 0 {
                break;
            }
            current_max_power = i;
        }
        current_max_power + 1
    };

    return decimal_to_snafu_helper(dec, max_power, &vec![])
        .unwrap()
        .iter()
        .map(|coef| match coef {
            -2 => '=',
            -1 => '-',
            0 => '0',
            1 => '1',
            2 => '2',
            _ => panic!("Unexpected coef {coef}"),
        })
        .collect();
}

fn decimal_to_snafu_helper(rem_dec: i64, power: u32, coefs: &Vec<i32>) -> Option<Vec<i32>> {
    for coef in [0, 1, -1, -2, 2] {
        let mut new_coefs = coefs.clone();
        new_coefs.push(coef);
        let new_rem_dec = rem_dec - (coef as i64 * 5_i64.pow(power));
        if new_rem_dec.abs() > 5_i64.pow(power) * 2 {
            continue;
        }
        if new_rem_dec == 0 {
            for _ in 0..power {
                new_coefs.push(0);
            }
            if new_coefs[0] == 0 {
                new_coefs.remove(0);
            }
            return Some(new_coefs);
        }
        if power == 0 {
            continue;
        }
        let maybe_result = decimal_to_snafu_helper(new_rem_dec, power - 1, &new_coefs);
        if let Some(result) = maybe_result {
            return Some(result);
        }
    }
    return None;
}

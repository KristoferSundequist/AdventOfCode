use std::fs;
use std::collections::HashMap;
use regex::Regex;

#[derive(Debug)]
struct Passport {
    byr: String,
    iyr: String,
    eyr: String,
    hgt: String,
    hcl: String,
    ecl: String,
    pid: String,
    cid: Option<String>

}

fn to_passport(str: &String) -> Result<Passport, String> {
    let values: Vec<Vec<&str>> = str.split(" ").map(|v| v.split(":")).map(|v| v.collect::<Vec<&str>>()).collect();
    let mut passport_values = HashMap::new();
    for arr in values.iter() {
        passport_values.insert(arr[0], arr[1]);
    }

    Ok(Passport {
        byr: passport_values.get("byr").map(|v| v.to_string()).ok_or("missing value byr")?,
        iyr: passport_values.get("iyr").map(|v| v.to_string()).ok_or("missing value iyr")?,
        eyr: passport_values.get("eyr").map(|v| v.to_string()).ok_or("missing value eyr")?,
        hgt: passport_values.get("hgt").map(|v| v.to_string()).ok_or("missing value hgt")?,
        hcl: passport_values.get("hcl").map(|v| v.to_string()).ok_or("missing value hcl")?,
        ecl: passport_values.get("ecl").map(|v| v.to_string()).ok_or("missing value ecl")?,
        pid: passport_values.get("pid").map(|v| v.to_string()).ok_or("missing value pid")?,
        cid: passport_values.get("cid").map(|v| v.to_string())
    })
}

fn to_passport2(str: &String) -> Result<Passport, String> {
    let values: Vec<Vec<&str>> = str.split(" ").map(|v| v.split(":")).map(|v| v.collect::<Vec<&str>>()).collect();
    let mut passport_values = HashMap::new();
    for arr in values.iter() {
        passport_values.insert(arr[0], arr[1]);
    }

    Ok(Passport {
        byr: passport_values.get("byr").map(|v| v.to_string()).ok_or("missing value byr".to_string()).and_then(|v| {
            let num = v.parse::<i32>().map_err(|_| "error".to_string())?;
            if 1920 <= num && num <= 2002 {
                return Ok(v);
            } else {
                return Err("Not between 1920 and 2002".to_string())
            }
        })?,
        iyr: passport_values.get("iyr").map(|v| v.to_string()).ok_or("missing value iyr".to_string()).and_then(|v| {
            let num = v.parse::<i32>().map_err(|_| "error".to_string())?;
            if 2010 <= num && num <= 2020 {
                return Ok(v);
            } else {
                return Err("Not between 2010 and 2020".to_string())
            }
        })?,
        eyr: passport_values.get("eyr").map(|v| v.to_string()).ok_or("missing value eyr".to_string()).and_then(|v| {
            let num = v.parse::<i32>().map_err(|_| "error".to_string())?;
            if 2020 <= num && num <= 2030 {
                return Ok(v);
            } else {
                return Err("Not between 2020 and 2030".to_string())
            }
        })?,
        hgt: passport_values.get("hgt").map(|v| v.to_string()).ok_or("missing value hgt".to_string()).and_then(|v| {
            let re = Regex::new(r"^([0-9]*)(in|cm)$").unwrap();
            let result = re.captures(&v).ok_or("hgt regex fail")?;
            let number = result[1].parse::<i32>().map_err(|e| format!("{:#?} failed to get hgt: {:#?}", result[1].to_string(), e.to_string()))?;

            match &result[2] {
                "cm" => {
                    if 150 <= number && number <= 193 {
                        Ok(v)
                    }else {
                        Err("hgt cm not within threshold".to_string())
                    }
                },
                "in" => {
                    if 59 <= number && number <= 76 {
                        Ok(v)
                    }else {
                        Err("hgt in not within threshold".to_string())
                    }
                },
                _ => Err("hgt error: Expected cm or in".to_string())
            }
        })?,
        hcl: passport_values.get("hcl").map(|v| v.to_string()).ok_or("missing value hcl").and_then(|v| {
            let re = Regex::new(r"^#[0-9a-f]{6}$").unwrap();
            if re.is_match(&v) {
                Ok(v)
            } else {
                Err("hcl regex fail")
            }
        })?,
        ecl: passport_values.get("ecl").map(|v| v.to_string()).ok_or("missing value ecl").and_then(|v| {
            let re = Regex::new(r"^amb|blu|brn|gry|grn|hzl|oth$").unwrap();
            if re.is_match(&v) {
                Ok(v)
            } else {
                Err("ecl regex fail")
            }
        })?,
        pid: passport_values.get("pid").map(|v| v.to_string()).ok_or("missing value pid").and_then(|v| {
            let re = Regex::new(r"^[0-9]{9}$").unwrap();
            if re.is_match(&v) {
                Ok(v)
            } else {
                Err("pid regex fail")
            }
        })?,
        cid: passport_values.get("cid").map(|v| v.to_string())
    })
}

fn main() {
    let raw: String =
        fs::read_to_string("./data.txt").unwrap();

    let lines: Vec<&str> = raw.split("\n").collect();
    let passport_strings: Vec<String> =
        lines.split(|&s| s == "")
        .map(|qwe| qwe.to_owned())
        .filter(|v| v.len() > 0)
        .map(|abc| abc.join(" "))
        .collect();
    
    let passports: Vec<Result<Passport, String>> = passport_strings.iter().map(to_passport2).collect();

    println!("{:#?}", passports);

    let valid_passport_count = passports.iter().filter(|p| p.is_ok()).count();
    println!("{:#?}", valid_passport_count);
}

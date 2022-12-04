fn main() {
    let lines: Vec<String> = std::fs::read_to_string("input.txt")
        .unwrap()
        .lines()
        .map(|line| line.to_string())
        .collect();

    let pairs = lines
        .iter()
        .map(|line| {
            line.split(",")
                .map(|part| part.to_string())
                .collect::<Vec<_>>()
        })
        .collect::<Vec<_>>();

    let parsed_sections = pairs.iter().map(|sections| {
        sections.iter().map(|section_str| {
            section_str
                .split("-")
                .map(|section_number| section_number.parse::<i32>().unwrap())
                .collect::<Vec<_>>()
        }).collect::<Vec<_>>()
    }).collect::<Vec<_>>();

    part1(&parsed_sections);
    part2(&parsed_sections);
}

fn part1(sections: &Vec<Vec<Vec<i32>>>) {
    let mut sum = 0;
    for section in sections {
        let start_first = section[0][0];
        let start_second = section[1][0];
        let end_first = section[0][1];
        let end_second = section[1][1];
        if start_first <= start_second && end_second <= end_first {
            sum += 1;
        } else if start_second <= start_first && end_first <= end_second {
            sum += 1;
        }
    }
    println!("Part 1: {}", sum);
}

fn part2(sections: &Vec<Vec<Vec<i32>>>) {
    let mut sum = 0;
    for section in sections {
        let start_first = section[0][0];
        let start_second = section[1][0];
        let end_first = section[0][1];
        let end_second = section[1][1];
        if start_first <= start_second && start_second <= end_first {
            sum += 1;
        } else if start_second <= start_first && start_first <= end_second {
            sum += 1;
        }
    }
    println!("Part 2: {}", sum);
}

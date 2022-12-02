fn main() {
    let input = std::fs::read_to_string("./input.txt").unwrap();
    let lines: Vec<&str> = input.split("\n").collect();
    let rounds: Vec<Vec<&str>> = lines
        .iter()
        .map(|round_str| round_str.split(" ").collect::<Vec<&str>>())
        .collect();

    let parsed_rounds1: Vec<Round> = rounds
        .iter()
        .map(|moves| (parse_enemy_move(moves[0]), parse_your_move_part1(moves[1])))
        .collect();
    let score1 = calc_score(&parsed_rounds1);
    println!("Part 1: {}", score1);

    let parsed_rounds2: Vec<Round> = rounds
        .iter()
        .map(|moves| {
            let enemy_move = parse_enemy_move(moves[0]);
            return (
                enemy_move,
                parse_your_move_part2(&enemy_move, moves[1]),
            );
        })
        .collect();
    let score2 = calc_score(&parsed_rounds2);
    println!("Part 2: {}", score2);
}

#[derive(Debug, Clone, Copy)]
enum GameMove {
    Rock,
    Paper,
    Scissor,
}

type Round = (GameMove, GameMove);

fn parse_enemy_move(str: &str) -> GameMove {
    match str {
        "A" => GameMove::Rock,
        "B" => GameMove::Paper,
        "C" => GameMove::Scissor,
        _ => panic!("Unexpected enemy move {}", str),
    }
}

fn parse_your_move_part1(str: &str) -> GameMove {
    match str {
        "X" => GameMove::Rock,
        "Y" => GameMove::Paper,
        "Z" => GameMove::Scissor,
        _ => panic!("Unexpected 'your' move {}", str),
    }
}

/*
    X => Lose,
    Y => Draw,
    Z => Win
*/
fn parse_your_move_part2(enemy_move: &GameMove, str: &str) -> GameMove {
    match (enemy_move, str) {
        (GameMove::Rock, "X") => GameMove::Scissor,
        (GameMove::Rock, "Y") => GameMove::Rock,
        (GameMove::Rock, "Z") => GameMove::Paper,

        (GameMove::Paper, "X") => GameMove::Rock,
        (GameMove::Paper, "Y") => GameMove::Paper,
        (GameMove::Paper, "Z") => GameMove::Scissor,

        (GameMove::Scissor, "X") => GameMove::Paper,
        (GameMove::Scissor, "Y") => GameMove::Scissor,
        (GameMove::Scissor, "Z") => GameMove::Rock,

        _ => panic!("Unexpected inputs {:#?} {:#?}", enemy_move, str),
    }
}

fn calc_score(rounds: &Vec<Round>) -> i64 {
    let mut total_score: i64 = 0;
    for round in rounds {
        total_score += calc_round_score(round);
    }
    return total_score;
}

fn calc_round_score(round: &Round) -> i64 {
    let mut score: i64 = 0;

    // win or lose or draw score
    score += match (&round.0, &round.1) {
        (GameMove::Paper, GameMove::Rock) => 0,
        (GameMove::Paper, GameMove::Scissor) => 6,

        (GameMove::Rock, GameMove::Scissor) => 0,
        (GameMove::Rock, GameMove::Paper) => 6,

        (GameMove::Scissor, GameMove::Paper) => 0,
        (GameMove::Scissor, GameMove::Rock) => 6,
        _ => 3,
    };

    // you move score
    score += match round.1 {
        GameMove::Rock => 1,
        GameMove::Paper => 2,
        GameMove::Scissor => 3,
    };

    return score;
}

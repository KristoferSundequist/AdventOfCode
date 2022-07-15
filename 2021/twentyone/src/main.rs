use std::collections::HashMap;

#[derive(Debug)]
struct Dice {
    last_roll: i64,
    num_rolls: i64,
}

impl Dice {
    fn new() -> Dice {
        Dice {
            last_roll: 0,
            num_rolls: 0,
        }
    }

    fn roll(&mut self) -> i64 {
        //println!("rolling dice: {:?}", self);
        let next_roll = if self.last_roll == 100 {
            1
        } else {
            self.last_roll + 1
        };
        self.num_rolls += 1;
        self.last_roll = next_roll;
        return next_roll;
    }
}

#[derive(Debug, Hash, Clone, PartialEq, Eq)]
struct Player {
    score: i64,
    position: i64,
}

impl Player {
    fn new(starting_position: i64) -> Player {
        Player {
            score: 0,
            position: starting_position,
        }
    }

    fn increase(&mut self, value: i64) {
        let sum = self.position + value;
        let new_position = if sum > 10 {
            let rem = sum % 10;
            if rem == 0 {
                10
            } else {
                rem
            }
        } else {
            sum
        };

        // println!(
        //     "increasing player by {} from {} to {}. New score: {}",
        //     value,
        //     self.position,
        //     new_position,
        //     self.score + new_position
        // );

        self.score += new_position;
        self.position = new_position;
    }
}

fn part1() {
    let mut dice = Dice::new();
    let mut player1 = Player::new(4);
    let mut player2 = Player::new(8);

    loop {
        let rolls = dice.roll() + dice.roll() + dice.roll();

        player1.increase(rolls);

        if player1.score >= 1000 {
            println!(
                "player 1 wins: {:?}, player2: {:?}, dice: {:?}, answer: {:?}",
                player1,
                player2,
                dice,
                player2.score * dice.num_rolls
            );
            break;
        }

        let rolls2 = dice.roll() + dice.roll() + dice.roll();

        player2.increase(rolls2);

        if player2.score >= 1000 {
            println!(
                "player 2 wins: {:?}, player1: {:?}, dice: {:?}, answer: {:?}",
                player2,
                player1,
                dice,
                player1.score * dice.num_rolls
            );
            break;
        }
    }
}

#[derive(Debug, Hash, PartialEq, Eq)]
struct Game {
    player1: Player,
    player2: Player,
}

fn part2() {
    let mut scores = HashMap::new();
    scores.insert(
        Game {
            player1: Player::new(1),
            player2: Player::new(5),
        },
        1,
    );
    let mut player1wins: i64 = 0;
    let mut player2wins: i64 = 0;

    while scores.len() > 0 {
        let mut new_scores: HashMap<Game, i64> = HashMap::new();

        for (score, num) in scores {
            for i1 in [1, 2, 3] {
                for i2 in [1, 2, 3] {
                    for i3 in [1, 2, 3] {
                        let mut new_player1 = score.player1.clone();
                        new_player1.increase(i1 + i2 + i3);

                        if new_player1.score >= 21 {
                            player1wins += num;
                            continue;
                        }
                        for j1 in [1, 2, 3] {
                            for j2 in [1, 2, 3] {
                                for j3 in [1, 2, 3] {
                                    let mut new_player2 = score.player2.clone();
                                    new_player2.increase(j1 + j2 + j3);

                                    if new_player2.score >= 21 {
                                        player2wins += num;
                                        continue;
                                    }

                                    let new_score = Game {
                                        player1: new_player1.clone(),
                                        player2: new_player2,
                                    };

                                    if let Some(s) = new_scores.get(&new_score) {
                                        new_scores.insert(new_score, s + num);
                                    } else {
                                        new_scores.insert(new_score, num);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        scores = new_scores;
        println!(
            "player1 wins: {}, player2 wins: {}",
            player1wins, player2wins
        );

        // println!("----------------------------------------");
        // println!("{:#?}", scores);
    }

    // println!(
    //     "player1 wins: {}, player2 wins: {}",
    //     player1wins, player2wins
    // );
}

fn main() {
    part1();
    part2();
}

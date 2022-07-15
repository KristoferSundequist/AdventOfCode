use std::collections::{HashMap, VecDeque};

#[derive(Hash, PartialEq, Eq, Debug, Clone)]
enum AmphipodType {
    A,
    B,
    C,
    D,
}

impl AmphipodType {
    fn energy(&self) -> i64 {
        match self {
            AmphipodType::A => 1,
            AmphipodType::B => 10,
            AmphipodType::C => 100,
            AmphipodType::D => 1000,
        }
    }
}

#[derive(Hash, PartialEq, Eq, Debug, Clone)]
struct Game {
    hallway: Vec<Option<AmphipodType>>,
    a_room: Room,
    b_room: Room,
    c_room: Room,
    d_room: Room,
}

impl Game {
    fn get_possible_new_states(&self) -> Vec<(i64, Game)> {
        let mut new_states: Vec<(i64, Game)> = vec![];

        // try move all hallway amphipods into pure rooms
        for (i, location) in self.hallway.iter().enumerate() {
            if let Some(amp) = location {
                let (room, hallway_distance, is_blocked) = match amp {
                    &AmphipodType::A => (
                        &self.a_room,
                        ((i as i64) - 2).abs(),
                        self.hallway_section_blocked(i, 2, i),
                    ),
                    &AmphipodType::B => (
                        &self.b_room,
                        ((i as i64) - 4).abs(),
                        self.hallway_section_blocked(i, 4, i),
                    ),
                    &AmphipodType::C => (
                        &self.c_room,
                        ((i as i64) - 6).abs(),
                        self.hallway_section_blocked(i, 6, i),
                    ),
                    &AmphipodType::D => (
                        &self.d_room,
                        ((i as i64) - 8).abs(),
                        self.hallway_section_blocked(i, 8, i),
                    ),
                };

                if !is_blocked && room.is_pure {
                    let (room_insertion_energy, new_room) = room.add();
                    let mut new_game = self.clone();
                    match amp {
                        &AmphipodType::A => new_game.a_room = new_room,
                        &AmphipodType::B => new_game.b_room = new_room,
                        &AmphipodType::C => new_game.c_room = new_room,
                        &AmphipodType::D => new_game.d_room = new_room,
                    };
                    new_game.hallway[i] = None;
                    let total_energy = hallway_distance * amp.energy() + room_insertion_energy;
                    new_states.push((total_energy, new_game));
                }
            }
        }

        // try move all room amphipods into hallway
        if !self.a_room.is_pure && !self.a_room.is_complete && self.a_room.locations.len() > 0 {
            let (energy_used_to_exit_room, amph_type, new_room) = self.a_room.remove();
            for i in (0..=10).filter(|v| ![2, 4, 6, 8].contains(v)) {
                if !self.hallway_section_blocked(i, 2, 1000) {
                    let mut new_game = self.clone();
                    new_game.a_room = new_room.clone();
                    new_game.hallway[i] = Some(amph_type.clone());
                    let total_energy =
                        energy_used_to_exit_room + (1 + ((i as i64) - 2).abs()) * amph_type.energy();
                    new_states.push((total_energy, new_game));
                }
            }
        }
        if !self.b_room.is_pure && !self.b_room.is_complete && self.b_room.locations.len() > 0 {
            let (energy_used_to_exit_room, amph_type, new_room) = self.b_room.remove();
            for i in (0..=10).filter(|v| ![2, 4, 6, 8].contains(v)) {
                if !self.hallway_section_blocked(i, 4, 1000) {
                    let mut new_game = self.clone();
                    new_game.b_room = new_room.clone();
                    new_game.hallway[i] = Some(amph_type.clone());
                    let total_energy =
                        energy_used_to_exit_room + (1 + ((i as i64) - 4).abs()) * amph_type.energy();
                    new_states.push((total_energy, new_game));
                }
            }
        }
        if !self.c_room.is_pure && !self.c_room.is_complete && self.c_room.locations.len() > 0 {
            let (energy_used_to_exit_room, amph_type, new_room) = self.c_room.remove();
            for i in (0..=10).filter(|v| ![2, 4, 6, 8].contains(v)) {
                if !self.hallway_section_blocked(i, 6, 1000) {
                    let mut new_game = self.clone();
                    new_game.c_room = new_room.clone();
                    new_game.hallway[i] = Some(amph_type.clone());
                    let total_energy =
                        energy_used_to_exit_room + (1 + ((i as i64) - 6).abs()) * amph_type.energy();
                    new_states.push((total_energy, new_game));
                }
            }
        }
        if !self.d_room.is_pure && !self.d_room.is_complete && self.d_room.locations.len() > 0 {
            let (energy_used_to_exit_room, amph_type, new_room) = self.d_room.remove();
            for i in (0..=10).filter(|v| ![2, 4, 6, 8].contains(v)) {
                if !self.hallway_section_blocked(i, 8, 1000) {
                    let mut new_game = self.clone();
                    new_game.d_room = new_room.clone();
                    new_game.hallway[i] = Some(amph_type.clone());
                    let total_energy =
                        energy_used_to_exit_room + (1 + ((i as i64) - 8).abs()) * amph_type.energy();
                    new_states.push((total_energy, new_game));
                }
            }
        }
        new_states
    }

    fn hallway_section_blocked(&self, i: usize, j: usize, ignore: usize) -> bool {
        for q in i.min(j)..=i.max(j) {
            if q != ignore && self.hallway[q].is_some() {
                return true
            }
        }
        return false
    }

    fn is_complete(&self) -> bool {
        self.a_room.is_complete
            && self.b_room.is_complete
            && self.c_room.is_complete
            && self.d_room.is_complete
    }
}

#[derive(Hash, PartialEq, Eq, Debug, Clone)]
struct Room {
    amphipod_type: AmphipodType,
    locations: VecDeque<AmphipodType>,
    is_pure: bool,
    is_complete: bool,
    room_size: usize,
}

impl Room {
    fn add(&self) -> (i64, Room) {
        if !self.is_pure {
            panic!("trying to add amph to non pure room");
        }
        let dist = (self.room_size as i64) - (self.locations.len() as i64);
        let mut new_locations = self.locations.clone();
        new_locations.push_back(self.amphipod_type.clone());
        let new_room = Room {
            amphipod_type: self.amphipod_type.clone(),
            locations: new_locations,
            is_pure: true,
            is_complete: if self.locations.len() + 1 == (self.room_size as usize) {
                true
            } else {
                false
            },
            room_size: self.room_size,
        };
        (dist * self.amphipod_type.energy(), new_room)
    }

    fn remove(&self) -> (i64, AmphipodType, Room) {
        if self.is_pure {
            panic!("trying to remove amph from pure room");
        }
        let dist = (self.room_size as i64) - (self.locations.len() as i64);
        let mut new_locations = self.locations.clone();
        let removed_amph = new_locations.pop_front().unwrap();
        let is_pure = if new_locations.iter().any(|l| *l != self.amphipod_type) {
            false
        } else {
            true
        };
        let new_room = Room {
            amphipod_type: self.amphipod_type.clone(),
            locations: new_locations,
            is_pure: is_pure,
            is_complete: false,
            room_size: self.room_size,
        };
        (dist * removed_amph.energy(), removed_amph, new_room)
    }
}

fn part1() {
    let init = Game {
        hallway: vec![
            None, None, None, None, None, None, None, None, None, None, None,
        ],
        a_room: Room {
            amphipod_type: AmphipodType::A,
            locations: VecDeque::from(vec![AmphipodType::A, AmphipodType::B]),
            is_pure: false,
            is_complete: false,
            room_size: 2,
        },
        b_room: Room {
            amphipod_type: AmphipodType::B,
            locations: VecDeque::from(vec![AmphipodType::C, AmphipodType::D]),
            is_pure: false,
            is_complete: false,
            room_size: 2,
        },
        c_room: Room {
            amphipod_type: AmphipodType::C,
            locations: VecDeque::from(vec![AmphipodType::C, AmphipodType::A]),
            is_pure: false,
            is_complete: false,
            room_size: 2,
        },
        d_room: Room {
            amphipod_type: AmphipodType::D,
            locations: VecDeque::from(vec![AmphipodType::D, AmphipodType::B]),
            is_pure: false,
            is_complete: false,
            room_size: 2,
        },
    };
    calc(init);
}

fn part2() {
    let init = Game {
        hallway: vec![
            None, None, None, None, None, None, None, None, None, None, None,
        ],
        a_room: Room {
            amphipod_type: AmphipodType::A,
            locations: VecDeque::from(vec![AmphipodType::A, AmphipodType::D, AmphipodType::D, AmphipodType::B]),
            is_pure: false,
            is_complete: false,
            room_size: 4,
        },
        b_room: Room {
            amphipod_type: AmphipodType::B,
            locations: VecDeque::from(vec![AmphipodType::C, AmphipodType::C, AmphipodType::B, AmphipodType::D]),
            is_pure: false,
            is_complete: false,
            room_size: 4,
        },
        c_room: Room {
            amphipod_type: AmphipodType::C,
            locations: VecDeque::from(vec![AmphipodType::C, AmphipodType::B, AmphipodType::A, AmphipodType::A]),
            is_pure: false,
            is_complete: false,
            room_size: 4,
        },
        d_room: Room {
            amphipod_type: AmphipodType::D,
            locations: VecDeque::from(vec![AmphipodType::D, AmphipodType::A, AmphipodType::C, AmphipodType::B]),
            is_pure: false,
            is_complete: false,
            room_size: 4,
        },
    };
    calc(init);
}

fn calc(init: Game) {
    let mut visited: HashMap<Game, i64> = HashMap::new();
    let mut current_states: HashMap<Game, i64> = HashMap::new();
    current_states.insert(init.clone(), 0);
    visited.insert(init.clone(), 0);
    let mut best_cur_score = i64::MAX;
    let mut generation = 0;
    loop {
        generation += 1;
        println!("{}: num_states: {}", generation, current_states.len());

        let new_states = next(&current_states);
        let mut next_current_states: HashMap<Game, i64> = HashMap::new();
        for (new_state, score) in new_states.iter() {
            if new_state.is_complete() {
                println!("{}", score);
                if *score < best_cur_score {
                    best_cur_score = *score;
                    println!("best score: {}", best_cur_score);
                }
            }

            let maybe_old_score = visited.get(new_state).map(|v| v.clone());

            // if *score >= best_cur_score {
            //     continue;
            // }


            if let Some(old_score) = maybe_old_score {
                if *score < old_score {
                    visited.insert(new_state.clone(), *score);
                    next_current_states.insert(new_state.clone(), *score);
                }
            }

            if maybe_old_score.is_none() {
                visited.insert(new_state.clone(), *score);
                next_current_states.insert(new_state.clone(), *score);
            }
        }
        if next_current_states.len() == 0 {
            println!("no next states, exiting");
            break;
        }
        current_states = next_current_states;
    }
}

fn next(states: &HashMap<Game, i64>) -> HashMap<Game, i64> {
    let mut next_states: HashMap<Game, i64> = HashMap::new();
    for (state, total_energy) in states.iter() {
        let new_states = state.get_possible_new_states();
        for (energy_used, new_state) in new_states {
            let new_score = total_energy + energy_used;
            let already_there = next_states.insert(new_state.clone(), new_score);
            if let Some(yupp) = already_there {
                if yupp < new_score {
                    next_states.insert(new_state, yupp);
                }
            }
        }
    }
    next_states
}

fn main() {
    println!("--- PART 1 ---");
    part1();
    println!("--- PART 2 ---");
    part2();
}

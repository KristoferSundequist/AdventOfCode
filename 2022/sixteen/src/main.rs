use regex::Regex;
use std::cmp::Ordering;
use std::collections::{HashMap, HashSet};
use std::hash::Hash;

fn main() {
    let input = parse("./input.txt");
    calc(&input);
    calc2(&input);
}

struct SearchTally<T: Eq + Hash + Clone> {
    searches: HashMap<T, Stats>,
    current_best: Best,
}

impl<T: Eq + Hash + Clone> SearchTally<T> {
    fn new() -> SearchTally<T> {
        SearchTally {
            searches: HashMap::new(),
            current_best: Best { total_flow: 0 },
        }
    }

    fn insert(&mut self, state: &T, stats: &Stats) {
        if stats.total_flow >= self.current_best.total_flow {
            self.current_best = Best {
                total_flow: stats.total_flow,
            }
        }
        self.searches.insert(state.clone(), stats.clone());
    }
}

struct Best {
    total_flow: i64,
}

fn calc(cave: &HashMap<String, Valve>) {
    let mut state_tally: SearchTally<State> = SearchTally::new();
    calc_helper(
        cave,
        &mut state_tally,
        &State {
            current_valve: "AA".to_string(),
            valves_on: vec![],
        },
        &Stats {
            total_flow: 0,
            minute: 0,
        },
        30,
    );
    println!("Part1: {}", state_tally.current_best.total_flow);
}

fn calc2(cave: &HashMap<String, Valve>) {
    let mut state_tally: SearchTally<State2> = SearchTally::new();
    calc_helper2(
        cave,
        &mut state_tally,
        &State2 {
            current_valve: "AA".to_string(),
            current_elephant_location: "AA".to_string(),
            valves_on: vec![],
        },
        &Stats {
            total_flow: 0,
            minute: 0,
        },
        26,
    );
    println!("Part2: {}", state_tally.current_best.total_flow);
}

fn calc_helper(
    cave: &HashMap<String, Valve>,
    state_tally: &mut SearchTally<State>,
    state: &State,
    current_stats: &Stats,
    total_time: i64,
) {
    if current_stats.minute == total_time {
        return;
    } else if let Some(last_time) = state_tally.searches.get(state) {
        if current_stats.total_flow > last_time.total_flow
            || (current_stats.total_flow == last_time.total_flow
                && current_stats.minute < last_time.minute)
        {
            state_tally.insert(state, current_stats);
            let new_trajectories = search(cave, state, current_stats, total_time);
            for t in new_trajectories.iter() {
                calc_helper(cave, state_tally, &t.0, &t.1, total_time);
            }
        }
    } else {
        state_tally.insert(state, current_stats);
        let new_trajectories = search(cave, state, current_stats, total_time);
        for t in new_trajectories.iter() {
            calc_helper(cave, state_tally, &t.0, &t.1, total_time);
        }
    }
}

fn calc_helper2(
    cave: &HashMap<String, Valve>,
    state_tally: &mut SearchTally<State2>,
    state: &State2,
    current_stats: &Stats,
    total_time: i64,
) {
    let current_opened = state.valves_on.iter().collect::<HashSet<_>>();
    let mut valves_left = cave
        .iter()
        .filter(|(vid, valve)| !current_opened.contains(vid) && valve.flow_rate > 0)
        .map(|(q, v)| v.clone())
        .collect::<Vec<Valve>>();
    valves_left.sort_by(|v1, v2| {
        if v1.flow_rate > v2.flow_rate {
            Ordering::Less
        } else {
            Ordering::Greater
        }
    });
    let mut future_flow_upper_bound: i64 = 0;
    for (i, _) in valves_left.iter().enumerate() {
        future_flow_upper_bound +=
            valves_left[i].flow_rate * (total_time - (current_stats.minute + (i as i64) * 2))
    }

    if current_stats.minute == total_time {
        return;
    } else if current_stats.total_flow + future_flow_upper_bound
        <= state_tally.current_best.total_flow
    {
        return;
    } else if let Some(last_time) = state_tally.searches.get(state) {
        if current_stats.total_flow > last_time.total_flow
            || (current_stats.total_flow == last_time.total_flow
                && current_stats.minute < last_time.minute)
        {
            state_tally.insert(state, current_stats);
            calc_helper2_helper(cave, state_tally, state, current_stats, total_time);
        }
    } else {
        state_tally.insert(state, current_stats);
        calc_helper2_helper(cave, state_tally, state, current_stats, total_time);
    }
}

fn calc_helper2_helper(
    cave: &HashMap<String, Valve>,
    state_tally: &mut SearchTally<State2>,
    state: &State2,
    current_stats: &Stats,
    total_time: i64,
) {
    let human_trajectories = search(
        cave,
        &State {
            current_valve: state.current_valve.clone(),
            valves_on: state.valves_on.clone(),
        },
        current_stats,
        total_time,
    );
    let elepant_trajectories = search(
        cave,
        &State {
            current_valve: state.current_elephant_location.clone(),
            valves_on: state.valves_on.clone(),
        },
        current_stats,
        total_time,
    );

    for (hi, ht) in human_trajectories.iter().enumerate() {
        for (ei, et) in elepant_trajectories.iter().enumerate() {
            // No point in just swapping places with eachother
            if ht.0.current_valve == state.current_elephant_location
                && et.0.current_valve == state.current_valve
            {
                continue;
            }

            // IF AT SAME PLACE, prune symmetries
            if et.0.current_valve == ht.0.current_valve {
                // if at the same place, one going right and other left is the same as if the first go left and the other right
                // so we only do one of them
                if hi >= human_trajectories.len() / 2 {
                    continue;
                }
                if ei < elepant_trajectories.len() / 2 {
                    continue;
                }

                // if at the same place and both opens valve, thats illegal
                let valves_on_in_current_state = state.valves_on.iter().collect::<HashSet<_>>();
                let maybe_new_valve_human =
                    ht.0.valves_on
                        .iter()
                        .find(|v| !valves_on_in_current_state.contains(v));

                let maybe_new_valve_elephant =
                    et.0.valves_on
                        .iter()
                        .find(|v| !valves_on_in_current_state.contains(v));
                if match (maybe_new_valve_human, maybe_new_valve_elephant) {
                    (Some(nvh), Some(nve)) => nvh == nve,
                    _ => false,
                } {
                    continue;
                }
            }

            let mut combined_turned_on = [ht.0.valves_on.clone(), et.0.valves_on.clone()]
                .concat()
                .into_iter()
                .collect::<HashSet<_>>()
                .into_iter()
                .collect::<Vec<_>>();
            combined_turned_on.sort();
            let combined_state = State2 {
                current_valve: ht.0.current_valve.clone(),
                current_elephant_location: et.0.current_valve.clone(),
                valves_on: combined_turned_on,
            };
            let combined_stats = Stats {
                total_flow: current_stats.total_flow
                    + (ht.1.total_flow - current_stats.total_flow)
                    + (et.1.total_flow - current_stats.total_flow),
                minute: ht.1.minute,
            };
            calc_helper2(
                cave,
                state_tally,
                &combined_state,
                &combined_stats,
                total_time,
            );
        }
    }
}

fn search(
    cave: &HashMap<String, Valve>,
    state: &State,
    current_stats: &Stats,
    total_time: i64,
) -> Vec<(State, Stats)> {
    let mut new_trajectories: Vec<(State, Stats)> = vec![];

    // if not turned on, search with turn on
    if cave.get(&state.current_valve).unwrap().flow_rate > 0
        && state.valves_on.iter().find(|&v| v == &state.current_valve) == None
    {
        let mut new_valves_on = state.valves_on.clone();
        new_valves_on.push(state.current_valve.clone());
        new_valves_on.sort();
        new_trajectories.push((
            State {
                current_valve: state.current_valve.clone(),
                valves_on: new_valves_on,
            },
            Stats {
                total_flow: current_stats.total_flow
                    + cave.get(&state.current_valve).unwrap().flow_rate
                        * (total_time - (current_stats.minute + 1)),
                minute: current_stats.minute + 1,
            },
        ))
    }

    for valve in cave.get(&state.current_valve).unwrap().leads_to.iter() {
        new_trajectories.push((
            State {
                current_valve: valve.clone(),
                valves_on: state.valves_on.clone(),
            },
            Stats {
                total_flow: current_stats.total_flow,
                minute: current_stats.minute + 1,
            },
        ))
    }
    return new_trajectories;
}

#[derive(Debug, Clone, Hash, Eq, PartialEq)]
struct State {
    current_valve: String,
    valves_on: Vec<String>,
}

#[derive(Debug, Clone, Hash, Eq, PartialEq)]
struct State2 {
    current_valve: String,
    current_elephant_location: String,
    valves_on: Vec<String>,
}

#[derive(Debug, Clone, Hash, Eq, PartialEq)]
struct Stats {
    total_flow: i64,
    minute: i64,
}

fn parse(filepath: &str) -> HashMap<String, Valve> {
    let input = std::fs::read_to_string(filepath).unwrap();
    let re =
        Regex::new("Valve (..) has flow rate=([0-9]+); tunnels? leads? to valves? (.*)$").unwrap();

    input
        .lines()
        .map(|line| {
            let captures = re.captures(line).unwrap();

            let id = captures.get(1).unwrap().as_str().to_string();
            let flow_rate = captures.get(2).unwrap().as_str().parse::<i64>().unwrap();
            let leads_to = captures
                .get(3)
                .unwrap()
                .as_str()
                .split(",")
                .map(|v| v.trim().to_string())
                .collect::<Vec<_>>();

            return (
                id.clone(),
                Valve {
                    id: id,
                    flow_rate: flow_rate,
                    leads_to: leads_to,
                },
            );
        })
        .collect::<HashMap<_, _>>()
}

#[derive(Debug, Eq, PartialEq, Hash, Clone)]
struct Valve {
    id: String,
    flow_rate: i64,
    leads_to: Vec<String>,
}

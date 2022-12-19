use regex::Regex;
use std::collections::HashSet;

fn main() {
    let input = parse("./input.txt");
    let qualities_sum: i64 = input.iter().map(|b| calculate(b, 24) * b.id).sum();
    println!("Part 1: {:#?}", qualities_sum);

    let max_geode_product: i64 = input.iter().take(3).map(|b| calculate(b, 32)).product();
    println!("Part 2: {:#?}", max_geode_product);
}

fn calculate(blueprint: &Blueprint, minutes: i64) -> i64 {
    println!("Blueprint: {}", blueprint.id);
    let mut current_states = HashSet::<State>::new();
    current_states.insert(State {
        robots: Robots {
            ore: 1,
            clay: 0,
            obsidian: 0,
            geode: 0,
        },
        resources: Resources {
            ore: 0,
            clay: 0,
            obsidian: 0,
            geode: 0,
        },
    });
    for minute in 0..minutes {
        //println!("Minute: {}, num_states: {}", minute, current_states.len());
        let mut new_states = HashSet::<State>::new();
        for state_base in current_states.iter() {
            let state = state_base.clone();

            // create new ore robot
            let ore_production_limit_reached = state.robots.ore >= blueprint.ore.ore
                && state.robots.ore >= blueprint.clay.ore
                && state.robots.ore >= blueprint.obsidian.ore
                && state.robots.ore >= blueprint.geode.ore;

            if state.resources.ore >= blueprint.ore.ore && !ore_production_limit_reached {
                let mut state_with_new_ore_robot = state.clone().with_resources_gained();
                state_with_new_ore_robot.resources.ore -= blueprint.ore.ore;
                state_with_new_ore_robot.robots.ore += 1;
                new_states.insert(state_with_new_ore_robot);
            }

            // create new clay robot
            let clay_production_less_than_most_expensive_thing =
                state.robots.clay < blueprint.obsidian.clay;
            if state.resources.ore >= blueprint.clay.ore
                && clay_production_less_than_most_expensive_thing
            {
                let mut state_with_new_clay_robot = state.clone().with_resources_gained();
                state_with_new_clay_robot.resources.ore -= blueprint.clay.ore;
                state_with_new_clay_robot.robots.clay += 1;
                new_states.insert(state_with_new_clay_robot);
            }

            // create new obsidian robot
            let obsidian_production_less_than_most_expensive_thing =
                state.robots.obsidian < blueprint.geode.obsidian;
            if state.resources.ore >= blueprint.obsidian.ore
                && state.resources.clay >= blueprint.obsidian.clay
                && obsidian_production_less_than_most_expensive_thing
            {
                let mut state_with_new_obsidian_robot = state.clone().with_resources_gained();
                state_with_new_obsidian_robot.resources.ore -= blueprint.obsidian.ore;
                state_with_new_obsidian_robot.resources.clay -= blueprint.obsidian.clay;
                state_with_new_obsidian_robot.robots.obsidian += 1;
                new_states.insert(state_with_new_obsidian_robot);
            }

            // create new geode robot
            if state.resources.ore >= blueprint.geode.ore
                && state.resources.obsidian >= blueprint.geode.obsidian
            {
                let mut state_with_new_geode_robot = state.clone().with_resources_gained();
                state_with_new_geode_robot.resources.ore -= blueprint.geode.ore;
                state_with_new_geode_robot.resources.obsidian -= blueprint.geode.obsidian;
                state_with_new_geode_robot.robots.geode += 1;
                new_states.insert(state_with_new_geode_robot);
            }

            new_states.insert(state.with_resources_gained());
        }

        let max_geode_state = current_states
            .iter()
            .max_by(|s1, s2| s1.resources.geode.cmp(&s2.resources.geode))
            .unwrap();

        current_states = new_states
            .into_iter()
            .filter(|s| {
                let minutes_left = minutes - minute;
                let best_lower_bound =
                    max_geode_state.resources.geode + max_geode_state.robots.geode * minutes_left;

                let mut geode_potential_upper_bound = s.resources.geode;
                for i in 0..minutes_left {
                    geode_potential_upper_bound += s.robots.geode + i;
                }
                if geode_potential_upper_bound > best_lower_bound {
                    return true;
                } else {
                    return false;
                }
            })
            .collect::<HashSet<_>>();
    }
    let maybe_max_state = current_states
        .iter()
        .max_by(|s1, s2| s1.resources.geode.cmp(&s2.resources.geode));

    let max_geodes = match maybe_max_state {
        Some(max_state) => max_state.resources.geode,
        _ => 0,
    };

    return max_geodes;
}

fn parse(filepath: &str) -> Vec<Blueprint> {
    let input = std::fs::read_to_string(filepath).unwrap();
    let re = Regex::new("Blueprint ([0-9]+): Each ore robot costs ([0-9]+) ore. Each clay robot costs ([0-9]+) ore. Each obsidian robot costs ([0-9]+) ore and ([0-9]+) clay. Each geode robot costs ([0-9]+) ore and ([0-9]+) obsidian.$").unwrap();

    input
        .lines()
        .map(|line| {
            let captures = re.captures(line).unwrap();

            Blueprint {
                id: captures.get(1).unwrap().as_str().parse::<i64>().unwrap(),
                ore: OreCost {
                    ore: captures.get(2).unwrap().as_str().parse::<i64>().unwrap(),
                },
                clay: ClayCost {
                    ore: captures.get(3).unwrap().as_str().parse::<i64>().unwrap(),
                },
                obsidian: ObsidianCost {
                    ore: captures.get(4).unwrap().as_str().parse::<i64>().unwrap(),
                    clay: captures.get(5).unwrap().as_str().parse::<i64>().unwrap(),
                },
                geode: GeodeCost {
                    ore: captures.get(6).unwrap().as_str().parse::<i64>().unwrap(),
                    obsidian: captures.get(7).unwrap().as_str().parse::<i64>().unwrap(),
                },
            }
        })
        .collect::<Vec<_>>()
}

#[derive(Debug, Clone, Hash, PartialEq, Eq)]
struct State {
    robots: Robots,
    resources: Resources,
}

impl State {
    fn with_resources_gained(&self) -> State {
        let mut clone = self.clone();
        clone.resources.ore += clone.robots.ore;
        clone.resources.clay += clone.robots.clay;
        clone.resources.obsidian += clone.robots.obsidian;
        clone.resources.geode += clone.robots.geode;
        return clone;
    }
}

#[derive(Debug, Clone, Hash, PartialEq, Eq)]
struct Resources {
    ore: i64,
    clay: i64,
    obsidian: i64,
    geode: i64,
}

#[derive(Debug, Clone, Hash, PartialEq, Eq)]
struct Robots {
    ore: i64,
    clay: i64,
    obsidian: i64,
    geode: i64,
}

#[derive(Debug, Clone, Hash, PartialEq, Eq)]
struct Blueprint {
    id: i64,
    ore: OreCost,
    clay: ClayCost,
    obsidian: ObsidianCost,
    geode: GeodeCost,
}

#[derive(Debug, Clone, Hash, PartialEq, Eq)]
struct OreCost {
    ore: i64,
}

#[derive(Debug, Clone, Hash, PartialEq, Eq)]
struct ClayCost {
    ore: i64,
}

#[derive(Debug, Clone, Hash, PartialEq, Eq)]
struct ObsidianCost {
    ore: i64,
    clay: i64,
}

#[derive(Debug, Clone, Hash, PartialEq, Eq)]
struct GeodeCost {
    ore: i64,
    obsidian: i64,
}

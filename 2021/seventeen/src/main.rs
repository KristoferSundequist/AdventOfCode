fn main() {
    let goalqwe = Goal {
        min_x: 20,
        max_x: 30,
        min_y: -10,
        max_y: -5
    };

    let goal = Goal {
        min_x: 34,
        max_x: 67,
        min_y: -215,
        max_y: -186
    };

    let curstate = State {
        x: 0,
        y: 0,
        dx: 7,
        dy: 2,
    };
    //simulate(&curstate, &goal);

    let mut count = 0;
    let mut current_max = 0;
    for dx in -100..68 {
        //println!("dx: {}", dx);
        for dy in -300..10000 {
            let state = State {
                x: 0,
                y: 0,
                dx: dx,
                dy: dy,
            };
            if let Some(max_y) = simulate(&state, &goal) {
                current_max = std::cmp::max(current_max, max_y);
                count += 1;
            }
        }
    }

    println!("maxy: {}, totalamount: {}", current_max, count);
}

fn simulate(initial_state: &State, goal: &Goal) -> Option<i64> {
    let mut current_state = initial_state.clone();
    let mut max_y = 0;

    loop {
        //println!("{:#?}", current_state);
        max_y = std::cmp::max(max_y, current_state.y);

        if is_in_goal(&current_state, goal) {
            return Some(max_y);
        }

        if current_state.dx == 0 && current_state.x < goal.min_x {
            return None;
        }

        if current_state.x > goal.max_x {
            return None;
        }

        if current_state.y < goal.min_y {
            return None;
        }

        current_state = step(&current_state);
    }
}

fn is_in_goal(state: &State, goal: &Goal) -> bool {
    goal.min_x <= state.x && state.x <= goal.max_x && goal.min_y <= state.y && state.y <= goal.max_y
}

fn step(state: &State) -> State {
    State {
        x: state.x + state.dx,
        y: state.y + state.dy,
        dx: state.dx - state.dx.signum(),
        dy: state.dy - 1,
    }
}

#[derive(Clone, Debug, Hash)]
struct State {
    x: i64,
    y: i64,
    dx: i64,
    dy: i64,
}

#[derive(Clone, Debug, Hash)]
struct Goal {
    min_x: i64,
    max_x: i64,
    min_y: i64,
    max_y: i64,
}

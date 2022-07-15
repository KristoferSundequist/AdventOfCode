use std::{cell::RefCell, thread::current};
use std::rc::Rc;
use rayon::prelude::*;

fn mod_or_wrap(x: i64, modulu: i64) -> i64 {
    let v = x % modulu;
    if v < 0 {
        modulu + v
    } else {
        v
    }
}

struct Node {
    v: i64,
    next: Option<Rc<RefCell<Node>>>,
}

fn build_circle(labeling: &Vec<i64>) -> Rc<RefCell<Node>> {
    let first = Rc::new(RefCell::new(Node {
        v: labeling[0],
        next: None,
    }));
    let mut current = Rc::clone(&first);
    for i in 1..labeling.len() {
        let next = Rc::new(RefCell::new(Node {
            v: labeling[i],
            next: None,
        }));
        current.borrow_mut().next = Some(Rc::clone(&next));
        current = next;
    }
    current.borrow_mut().next = Some(Rc::clone(&first));
    first
}

fn part_one_v2(prev_current: &Rc<RefCell<Node>>, max_iter: i32, max_v: i64) -> Rc<RefCell<Node>> {
    let index = build_index(&prev_current, max_v);
    (0..max_iter).fold(prev_current.clone(), |prev_current, ii| {
        // if ii % 100 == 0 {
        //     println!("ii: {}", ii);
        // }
        let last_picked = (0..3).fold(Rc::clone(&prev_current), |prev, _| Rc::clone(&prev).borrow().next.as_ref().unwrap().clone());

        let first_picked = prev_current.borrow().next.clone().unwrap();
        //print_cycle(&first_picked, 3 as usize);
        prev_current.borrow_mut().next = Some(Rc::clone(&last_picked.borrow().next.clone().unwrap()));

        let dest_node = {
            let mut current_dest_value = prev_current.borrow().v - 1;
            if current_dest_value == 0 {
                current_dest_value = max_v;
            }
            while cycle_contains(&first_picked, 3, current_dest_value) {
                current_dest_value -= 1;
                if current_dest_value == 0 {
                    current_dest_value = max_v;
                }
            }
            //move_to_value(&prev_current, current_dest_value, max_v)
            Rc::clone(&index[current_dest_value as usize])
        };
        last_picked.borrow_mut().next = Some(Rc::clone(&dest_node.borrow().next.clone().unwrap()));
        dest_node.borrow_mut().next = Some(Rc::clone(&first_picked));
        prev_current.borrow().next.clone().unwrap()
    })
}

fn build_index(first: &Rc<RefCell<Node>>, len: i64) -> Vec<Rc<RefCell<Node>>> {
    let mut index_vec: Vec<Rc<RefCell<Node>>> = (0..=len).map(|_| Rc::new(RefCell::new(Node { v: 0, next: None }))).collect();
    let mut current = Rc::clone(first);
    for _ in 0..=len {
        index_vec[current.borrow().v as usize] = Rc::clone(&current);
        current = Rc::clone(&current).borrow().next.clone().unwrap();
    }
    index_vec
}

fn move_to_value(first: &Rc<RefCell<Node>>, value: i64, len: i64) -> Rc<RefCell<Node>> {
    //println!("entering move to value ---- searhing for: {}", value);
    let mut current = Rc::clone(first);
    for _ in 0..len {
        //println!("trying: {}", current.borrow().v);
        if current.borrow().v == value {
            return current;
        }
        let next = Rc::clone(&current).borrow().next.clone().unwrap();
        current = next;
    }
    unreachable!("unexpected not found in move_to_value");
}

fn cycle_contains(first: &Rc<RefCell<Node>>, len: usize, value: i64) -> bool {
    let mut current = Rc::clone(first);
    for _ in 0..len {
        if current.borrow().v == value {
            return true;
        }
        let next = Rc::clone(&current).borrow().next.clone().unwrap();
        current = next;
    }
    return false;
}

fn print_cycle(first: &Rc<RefCell<Node>>, len: usize) {
    let mut current = Rc::clone(first);
    for _ in 0..len {
        print!("{}, ", current.borrow().v);
        let next = current.borrow().next.clone().unwrap();
        current = next;
    }
    println!("");
}

fn part_two(labeling: &Vec<i64>, part_one: bool) -> Vec<i64> {
    let (the_labeling, biggest_v, iters) =
        if part_one {
            (labeling.clone(), 9, 100)
        } else {
            let biggest_v = 1000000;
            let rest = (10..=biggest_v).collect::<Vec<i64>>();
            (
                [&labeling[..], &rest[..]].concat().to_vec(),
                biggest_v,
                10000000,
            )
        };

    let start_time = std::time::Instant::now();
    let (final_labeling, i) = (0..iters).fold((the_labeling, 0), |(current_labeling, current_index), movei| {
        if movei % 100 == 0 {
            println!(
                "iter: {}/{}={}, elapsedtime(seconds): {:?}",
                movei,
                iters,
                (movei as f32) / (iters as f32),
                start_time.elapsed().as_secs()
            );
        }
        
            let current_value = current_labeling[current_index];
            //println!("\n {:?} ------------ current value: {:?}, labels: {:?}", movei+1, current_value, current_labeling);
            let mut picked_up_cups = vec![];
            for i in 1..4 {
                picked_up_cups
                    .push(current_labeling[((current_index + i) % (biggest_v as usize)) as usize]);
            }
            //println!("pickups: {:?}", picked_up_cups);

            let remaining = current_labeling
                .iter()
                .filter(|&v| !picked_up_cups.contains(v))
                .map(|v| v.clone())
                .collect::<Vec<i64>>();

            let destination_cup_index = {
                let mut destination_value = current_value - 1;
                if destination_value <= 0 {
                    destination_value = biggest_v;
                }
                while picked_up_cups.contains(&destination_value) {
                    destination_value -= 1;
                    if destination_value <= 0 {
                        destination_value = biggest_v;
                    }
                }
                
                remaining
                    .iter()
                    .position(|&v| v == destination_value)
                    .unwrap()
            };
            let index_to_put = destination_cup_index + 1;
            
            let new_labeling = [&remaining[0..index_to_put], picked_up_cups.as_slice(), &remaining[index_to_put..]].concat().to_vec();
            
            //let new_cup_index = ((new_labeling.iter().position(|&v| v == current_value).unwrap()+ 1) % (biggest_v as usize)) as usize;
            let new_cup_index = (
                if labeling.len() - 1 - current_index < 3 { labeling.len() - 1 - current_index }
                else if index_to_put <= current_index { 3 }
                else { 0 }
                + current_index + 1) % (biggest_v as usize);
            //println!("current_index: {:?}, index_to_put: {:?}, new_cup_index: {:?}", current_index, index_to_put, new_cup_index);
            (new_labeling, new_cup_index)
        });

    for zi in i..i + 3 {
        println!("{:?}", final_labeling[zi % (biggest_v as usize)]);
    }

    println!("part2: {}", final_labeling[(i + 1) % (biggest_v as usize)] * final_labeling[(i + 2) % (biggest_v as usize)]);
    final_labeling
}

fn main() {
    //rayon::ThreadPoolBuilder::new().num_threads(6).build_global().unwrap();
    //let labeling = vec![3, 8, 9, 1, 2, 5, 4, 6, 7];
    let labeling = vec![8,7,1,3,6,9,4,5,2];
    // let result1 = part_two(&labeling, true);
    // println!("{:?}", result1);

    //let result2 = part_two(&labeling, false);
    //println!("{:?}", result2);
    
    let first = build_circle(&labeling);
    let result12 = part_one_v2(&first, 100, 9);
    println!("printing cycle");
    print_cycle(&result12, 9);

    let biggest_v = 1000000;
    let rest = (10..=biggest_v).collect::<Vec<i64>>();
    let labeling2 = [&labeling[..], &rest[..]].concat().to_vec();
    let first2 = build_circle(&labeling2);
    let result13 = part_one_v2(&first2, 10000000, biggest_v);
    let one = move_to_value(&result13, 1, biggest_v);
    let (two, three) = {
        let two = Rc::clone(&one.borrow().next.clone().unwrap());
        let three = Rc::clone(&two.borrow().next.clone().unwrap());
        let (a,b) = (two.borrow().v.clone(), three.borrow().v.clone());
        (a,b)
    };
    println!("result2: {} {}", two, three);

}

#[cfg(test)]
mod test {
    use super::*;

    #[test]
    fn mod_or_wrap_test() {
        assert_eq!(mod_or_wrap(-3, 7), 4);
        assert_eq!(mod_or_wrap(3, 7), 3);
        assert_eq!(mod_or_wrap(11, 5), 1);
        assert_eq!(mod_or_wrap(5, 5), 0);
    }

    #[test]
    fn part_one_test() {
        assert_eq!(part_two(&vec![3, 8, 9, 1, 2, 5, 4, 6, 7], true), vec![2, 9, 1, 6, 7, 3, 8, 4, 5]);
    }

    // #[test]
    // fn part_one_v2_test() {
    //     let labeling = vec![3, 8, 9, 1, 2, 5, 4, 6, 7];
    //     let first = build_circle(&labeling);
    //     let result1 = part_one_v2(&first, 0, 100);
    //     print_cycle(&result1, 10);
    //     assert_eq!(true, false);
    // }

    #[test]
    fn test_cycle_contains() {
        let labeling = vec![3, 8, 9, 1, 2, 5, 4, 6, 7];
        let first = build_circle(&labeling);
        assert_eq!(cycle_contains(&first, labeling.len(), 10), false);
        assert_eq!(cycle_contains(&first, labeling.len(), 3), true);
        assert_eq!(cycle_contains(&first, labeling.len(), 6), true);
        assert_eq!(cycle_contains(&first, labeling.len(), 11), false);
        assert_eq!(cycle_contains(&first, labeling.len(), -5), false);
        assert_eq!(cycle_contains(&first, labeling.len(), 2), true);
    }
}

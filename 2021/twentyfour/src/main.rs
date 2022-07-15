#[derive(Debug)]
struct Block {
    z_division: i64,
    first_add: i64,
    last_add: i64,
}

fn calc(z: i64, w: i64, calc: &Block) -> i64 {
    let not_same = if (z % 26) + calc.first_add != w { 1 } else { 0 };
    let new_z = z / calc.z_division;
    return new_z + not_same * 25 * new_z + not_same * (w + calc.last_add);
}

fn get_valid(z: i64, ws: Vec<i64>, i: usize, blocks: &Vec<Block>, highest: bool) -> Option<i64> {
    if i == 14 {
        if z == 0 {
            let qwe = ws
                .iter()
                .map(|v| v.to_string())
                .collect::<Vec<_>>()
                .join("")
                .parse::<i64>()
                .unwrap();
            return Some(qwe);
        }
        return None;
    } else {
        let block = &blocks[i];

        let results = (1..=9)
            .filter(|&new_w| {
                if block.z_division == 26 && (z % 26) + block.first_add != new_w {
                    false
                } else {
                    true
                }
            })
            .map(|new_w| {
                let mut new_ws = ws.clone();
                new_ws.push(new_w);
                return get_valid(calc(z, new_w, block), new_ws, i + 1, blocks, highest);
            })
            .collect::<Vec<_>>();

        let mut cur_extreme = None;
        for result in results {
            if let Some(v) = result {
                if let Some(prev) = cur_extreme {
                    if highest {
                        if v > prev {
                            cur_extreme = result;
                        }
                    } else {
                        if v < prev {
                            cur_extreme = result;
                        }
                    }
                } else {
                    cur_extreme = result;
                }
            }
        }
        cur_extreme
    }
}

fn main() {
    let inputs = std::fs::read_to_string("data.txt").unwrap();
    let lines = inputs.split("\n").collect::<Vec<_>>();
    let blocks = lines
        .chunks(18)
        .map(|chunk| Block {
            z_division: (chunk[4].split(" ").nth(2).unwrap() as &str)
                .parse::<i64>()
                .unwrap(),
            first_add: (chunk[5].split(" ").nth(2).unwrap() as &str)
                .parse::<i64>()
                .unwrap(),
            last_add: (chunk[15].split(" ").nth(2).unwrap() as &str)
                .parse::<i64>()
                .unwrap(),
        })
        .collect::<Vec<_>>();

    let max = get_valid(0, vec![], 0, &blocks, true);
    let min = get_valid(0, vec![], 0, &blocks, false);
    println!("max: {:?}, min: {:?}", max, min);
}

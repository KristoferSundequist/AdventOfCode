fn main() {
    let input = std::fs::read_to_string("./input.txt").unwrap();
    let lines = input.lines().collect::<Vec<_>>();
    let grid = lines
        .iter()
        .map(|line| {
            line.chars()
                .map(|height| height.to_string().parse::<i32>().unwrap())
                .collect::<Vec<_>>()
        })
        .collect::<Vec<_>>();

    let mut visible_sum = grid.len() * 2 + grid[0].len() * 2 - 4;
    for y in 1..grid.len() - 1 {
        for x in 1..grid[0].len() - 1 {
            if is_visible(&grid, y, x) {
                visible_sum += 1;
            }
        }
    }
    println!("Part 1: {:#?}", visible_sum);

    let mut max_scenic: usize = 0;
    for y in 1..grid.len() - 1 {
        for x in 1..grid[0].len() - 1 {
            let score = scenic_score(&grid, y, x);
            if score > max_scenic {
                max_scenic = score;
            }
        }
    }
    println!("Part 2: {:#?}", max_scenic);
}

fn is_visible(grid: &Vec<Vec<i32>>, y: usize, x: usize) -> bool {
    let tree_height = grid[y][x];
    let visible_left = grid[y][0..x].iter().all(|height| height < &tree_height);
    if visible_left {
        return true;
    }
    let visible_right = grid[y][x + 1..grid[y].len()]
        .iter()
        .all(|height| height < &tree_height);
    if visible_right {
        return true;
    }
    let visible_up = grid[0..y].iter().all(|row| row[x] < tree_height);
    if visible_up {
        return true;
    }
    let visible_down = grid[y + 1..grid.len()]
        .iter()
        .all(|row| row[x] < tree_height);
    if visible_down {
        return true;
    }
    return false;
}

fn scenic_score(grid: &Vec<Vec<i32>>, y: usize, x: usize) -> usize {
    let mut up_trees: usize = 0;
    for i in 1..1000 {
        if y as i32 - i < 0 {
            break;
        }
        up_trees += 1;
        if grid[y - i as usize][x] >= grid[y][x] {
            break;
        }
    }

    let mut down_trees: usize = 0;
    for i in 1..1000 {
        if y + i >= grid.len() {
            break;
        }
        down_trees += 1;
        if grid[y + i][x] >= grid[y][x] {
            break;
        }
    }

    let mut right_trees: usize = 0;
    for i in 1..1000 {
        if x + i >= grid[0].len() {
            break;
        }
        right_trees += 1;
        if grid[y][x + i] >= grid[y][x] {
            break;
        }
    }

    let mut left_trees: usize = 0;
    for i in 1..1000 {
        if x as i32 - i < 0 {
            break;
        }
        left_trees += 1;
        if grid[y][x - i as usize] >= grid[y][x] {
            break;
        }
    }

    return up_trees * down_trees * left_trees * right_trees;
}

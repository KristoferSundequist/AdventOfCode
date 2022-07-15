use std::collections::HashMap;

fn rotate_90(tile: &Vec<String>) -> Vec<String> {
    let mut new_tile = tile
        .clone()
        .iter()
        .map(|s| s.chars().collect::<Vec<char>>())
        .collect::<Vec<_>>();
    let size = tile.len();
    for i in 0..size {
        for j in 0..size {
            new_tile[i][j] = tile[size - 1 - j].chars().nth(i).unwrap();
        }
    }
    new_tile
        .iter()
        .map(|r| r.iter().collect::<String>())
        .collect::<Vec<_>>()
}

fn flip(tile: &Vec<String>) -> Vec<String> {
    let mut new_tile = tile.clone();

    for i in 0..new_tile.len() {
        new_tile[i] = tile[tile.len() - 1 - i].clone();
    }
    new_tile
}

fn get_top_side(tile: &Vec<String>) -> String {
    tile[0].clone()
}

fn get_bottom_side(tile: &Vec<String>) -> String {
    tile.last().unwrap().clone()
}

fn get_right_side(tile: &Vec<String>) -> String {
    let mut str = "".to_string();
    for i in 0..tile.len() {
        str.push(tile[i].chars().last().unwrap());
    }
    str
}

fn get_left_side(tile: &Vec<String>) -> String {
    let mut str = "".to_string();
    for i in 0..tile.len() {
        str.push(tile[i].chars().nth(0).unwrap());
    }
    str
}

fn get_hash(side: &String) -> i64 {
    side.chars()
        .fold((0, 1), |(sum, power), c| {
            let on = match c {
                '.' => 0,
                '#' => 1,
                _ => unreachable!("unknown char in get_hash."),
            };
            (sum + on * power, power * 2)
        })
        .0
}

fn find_matches(
    available_tiles: &HashMap<i64, Vec<String>>,
    left: Option<i64>,
    top: Option<i64>,
) -> Vec<(i64, Vec<String>)> {
    let mut matches = vec![];
    for (id, tile) in available_tiles.iter() {
        let mut morphed_tile = tile.clone();
        for _ in 0..2 {
            for _ in 0..4 {
                let matches_left =
                    if let Some(l) = left {
                        l == get_hash(&get_left_side(&morphed_tile))
                    } else {
                        true
                    };
                
                let matches_top =
                    if let Some(t) = top {
                        t == get_hash(&get_top_side(&morphed_tile))
                    } else {
                        true
                    };
                
                if matches_left && matches_top {
                    matches.push((*id, morphed_tile.clone()));
                }
                
                morphed_tile = rotate_90(&morphed_tile);
            }
            morphed_tile = flip(&tile);
        }
    }
    matches
}

fn part_one(
    available_tiles: &HashMap<i64, Vec<String>>,
    side_size: usize,
    grid: &Vec<Vec<(i64, Vec<String>)>>,
    (y, x): (usize, usize),
) -> Option<(i64, Vec<Vec<Vec<String>>>)> {

    let (left, top) =
        if x == 0 && y == 0 {
            (None, None)
        } else {
            let left = if x == 0 { None } else { Some(get_hash(&get_right_side(&grid[y][x-1].1))) };
            let top = if y == 0 { None } else { Some(get_hash(&get_bottom_side(&grid[y-1][x].1))) };
            (left, top) 
        };
    
    let matches = find_matches(available_tiles, left, top);

    matches.iter().filter_map(|(id, tile)| {
        let mut remaining_tiles = available_tiles.clone();
        remaining_tiles.remove(&id);

        let mut new_grid = grid.clone();
        new_grid.iter_mut().last().unwrap().push((*id, tile.clone()));
        if new_grid[y].len() == side_size {
            new_grid.push(vec![]);
        }

        if remaining_tiles.len() == 0 {
            let v = new_grid[0][0].0 * new_grid[0][side_size-1].0 * new_grid[side_size-1][0].0 * new_grid[side_size-1][side_size-1].0;
            return Some((v, new_grid.iter().map(|r| r.iter().map(|(_, t)| t.clone()).collect::<Vec<_>>()).collect::<Vec<_>>()));
        };

        let (ny,nx) = {
            if x < side_size-1 {
                (y, x+1)
            } else {
                (y+1, 0)
            }
        };
        
        part_one(&remaining_tiles, side_size, &new_grid, (ny, nx))
    }).next()
}

fn crop(img: &Vec<String>) -> Vec<String> {
    let mut new_img = vec![];

    for i in 1..img.len()-1 {
        new_img.push(img[i][1..img.len()-1].to_string());
    }
    new_img
}

fn merge(grid: &Vec<Vec<Vec<String>>>) -> Vec<String> {

    let cropped = grid.iter().map(|r| r.iter().map(|t| crop(t)).collect::<Vec<_>>()).collect::<Vec<_>>();

    let mut img: Vec<String> = vec![];
    let tile_side_size = 8;
    for (i, tile_row) in cropped.iter().enumerate() {
        for _ in 0..tile_side_size {
            img.push("".to_string());
        }

        for (_, tile) in tile_row.iter().enumerate() {
            for (z, row) in tile.iter().enumerate() {
                img[tile_side_size*i + z].push_str(row);
            }
        }
    }

    img
}

fn found_monsters(img: &Vec<String>) -> (usize, usize) {
    //len == 20
    let sea_monster: Vec<&str> =
        vec![
            "                  # ",
            "#    ##    ##    ###",
            " #  #  #  #  #  #   "
        ];
    
    let mut monster_coords = vec![];
    for y in 0..(img.len() - 3) {
        for x in 0..(img[0].len() - 20) {
            let found_it = (0..3).all(|i| {
                (0..20).all(|sx| {
                    match (sea_monster[i].chars().nth(sx).unwrap(), img[y+i].chars().nth(x+sx).unwrap()) {
                        ('#', '.') => false,
                        _ => true
                    }
                })
            });
            if found_it {
                monster_coords.push((y,x));
            }
        }
    }
    
    let mut new_img = img.iter().map(|r| r.chars().collect::<Vec<_>>()).collect::<Vec<_>>();
    for (y,x) in monster_coords.iter() {
        for yi in 0..3 {
            for xi in 0..20 {
                if sea_monster[yi].chars().nth(xi).unwrap() == '#' {
                    new_img[y+yi][x+xi] = '.';
                }
            }
        }
    }
    let roughness = new_img.iter().flatten().collect::<String>().chars().filter(|&c| c == '#').collect::<String>().len();

    (monster_coords.len(), roughness)

}

fn part_two(img: &Vec<String>) -> Vec<(usize, usize)> {
    let mut sea_monster_info = vec![];
        let mut morphed_img = img.clone();
        for _ in 0..2 {
            for _ in 0..4 {
                sea_monster_info.push(found_monsters(&morphed_img));
                
                morphed_img = rotate_90(&morphed_img);
            }
            morphed_img = flip(&img);
        }
    sea_monster_info
}

fn main() {
    let lines = std::fs::read_to_string("./data.txt")
        .unwrap()
        .lines()
        .map(|l| l.to_string())
        .collect::<Vec<_>>();
    let tiles: HashMap<i64, Vec<String>> = lines
        .split(|l| l == "")
        .map(|t| (t[0][5..=8].parse::<i64>().unwrap(), t[1..].to_vec()))
        .collect();

    let side_size = (tiles.len() as f64).sqrt() as usize;

    let (value, new_grid) = part_one(&tiles, side_size, &vec![vec![]], (0, 0)).unwrap();
    println!("result1: {:#?}", value);

    let merged = merge(&new_grid[..new_grid.len()-1].to_vec());
    let result2 = part_two(&merged);
    println!("result2: {:#?}", result2);

}

#[cfg(test)]
mod test {
    use super::*;

    #[test]
    fn test_rotate() {
        let tile = vec![
            "..##".to_string(),
            "##..".to_string(),
            "#...".to_string(),
            "####".to_string(),
        ];

        let rotated = rotate_90(&tile);

        let expected = vec![
            "###.".to_string(),
            "#.#.".to_string(),
            "#..#".to_string(),
            "#..#".to_string(),
        ];

        assert_eq!(expected, rotated);
    }

    #[test]
    fn test_flip() {
        let tile = vec![
            "..##".to_string(),
            "##..".to_string(),
            "#...".to_string(),
            "####".to_string(),
        ];

        let rotated = flip(&tile);

        let expected = vec![
            "####".to_string(),
            "#...".to_string(),
            "##..".to_string(),
            "..##".to_string(),
        ];

        assert_eq!(expected, rotated);
    }

    #[test]
    fn test_get_hash() {
        assert_eq!(get_hash(&"..##".to_string()), 0 + 0 + 4 + 8);
        assert_eq!(get_hash(&"#...".to_string()), 1);
        assert_eq!(get_hash(&".#..".to_string()), 0 + 2);
        assert_eq!(get_hash(&"####..#".to_string()), 1 + 2 + 4 + 8 + 0 + 0 + 64);
    }

    #[test]
    fn test_get_sides() {
        let tile = vec![
            "..##".to_string(),
            "##..".to_string(),
            "#...".to_string(),
            "####".to_string(),
        ];

        assert_eq!(get_left_side(&tile), ".###".to_string());
        assert_eq!(get_top_side(&tile), "..##".to_string());
        assert_eq!(get_right_side(&tile), "#..#".to_string());
        assert_eq!(get_bottom_side(&tile), "####".to_string());
    }

    #[test]
    fn test_crop() {
        let tile = vec![
            "..##".to_string(),
            "##..".to_string(),
            "#...".to_string(),
            "####".to_string(),
        ];

        let expected = vec![
            "#.".to_string(),
            "..".to_string()
        ];

        assert_eq!(crop(&tile), expected);
    }

    
}
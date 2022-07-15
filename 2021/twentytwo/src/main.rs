use std::collections::HashSet;

#[derive(Debug, Hash, PartialEq, Eq, Clone)]
struct Cuboid {
    xmin: i64,
    xmax: i64,
    ymin: i64,
    ymax: i64,
    zmin: i64,
    zmax: i64,
}

fn split_dimension_with_other(
    min: i64,
    max: i64,
    other_min: i64,
    other_max: i64,
) -> Vec<(i64, i64)> {
    let mut splits: Vec<(i64, i64)> = vec![];
    if min < other_min && other_min <= max && max <= other_max {
        splits.push((min, other_min - 1));
        splits.push((other_min, max));
    } else if other_min <= min && max <= other_max {
        splits.push((min, max));
    } else if other_min <= min && min <= other_max && other_max < max {
        splits.push((min, other_max));
        splits.push((other_max + 1, max));
    } else {
        splits.push((min, other_min - 1));
        splits.push((other_min, other_max));
        splits.push((other_max + 1, max));
    }
    splits
}

impl Cuboid {
    fn intersects(&self, other: &Cuboid) -> bool {
        ((other.xmin <= self.xmin && self.xmin <= other.xmax)
            || (self.xmin <= other.xmin && other.xmin <= self.xmax))
            && ((other.ymin <= self.ymin && self.ymin <= other.ymax)
                || (self.ymin <= other.ymin && other.ymin <= self.ymax))
            && ((other.zmin <= self.zmin && self.zmin <= other.zmax)
                || (self.zmin <= other.zmin && other.zmin <= self.zmax))
    }

    fn volume(&self) -> i64 {
        (1 + self.xmax - self.xmin) * (1 + self.ymax - self.ymin) * (1 + self.zmax - self.zmin)
    }

    fn completely_inside(&self, other: &Cuboid) -> bool {
        other.xmin <= self.xmin
            && self.xmax <= other.xmax
            && other.ymin <= self.ymin
            && self.ymax <= other.ymax
            && other.zmin <= self.zmin
            && self.zmax <= other.zmax
    }

    fn split_with_other(&self, other: &Cuboid) -> HashSet<Cuboid> {
        let x_splits = split_dimension_with_other(self.xmin, self.xmax, other.xmin, other.xmax);
        let y_splits = split_dimension_with_other(self.ymin, self.ymax, other.ymin, other.ymax);
        let z_splits = split_dimension_with_other(self.zmin, self.zmax, other.zmin, other.zmax);

        let mut new_cuboids: HashSet<Cuboid> = HashSet::new();
        for xs in x_splits.iter() {
            for ys in y_splits.iter() {
                for zs in z_splits.iter() {
                    let nc = Cuboid {
                        xmin: xs.0,
                        xmax: xs.1,
                        ymin: ys.0,
                        ymax: ys.1,
                        zmin: zs.0,
                        zmax: zs.1,
                    };
                    if !nc.intersects(other) {
                        new_cuboids.insert(nc);
                    }
                }
            }
        }
        new_cuboids
    }
}

fn main() {
    println!("Parsing file");
    let steps = parse_file();

    //part1(&steps);
    part2(&steps);
}

fn split_cuboid(
    possible_intersections: &Vec<&Cuboid>,
    cubeoid_to_split: &Cuboid,
) -> HashSet<Cuboid> {
    let mut intersections: Vec<&Cuboid> = vec![];
    for pic in possible_intersections {
        if cubeoid_to_split.completely_inside(pic) {
            return HashSet::new();
        }
        if pic.intersects(cubeoid_to_split) {
            intersections.push(pic);
        }
    }

    let mut non_intersecting_cuboids: HashSet<Cuboid> = HashSet::new();

    if intersections.is_empty() {
        non_intersecting_cuboids.insert(cubeoid_to_split.clone());
    } else {
        //let splits = cubeoid_to_split.split();
        let splits = cubeoid_to_split.split_with_other(&intersections[0]);
        for split in splits {
            let rec_cuboids = split_cuboid(&intersections, &split);
            for newc in rec_cuboids {
                non_intersecting_cuboids.insert(newc);
            }
        }
    }
    non_intersecting_cuboids
}

fn part2(steps: &Vec<(bool, Cuboid)>) {
    let mut ons: HashSet<Cuboid> = HashSet::new();
    for (i, (on, cuboid)) in steps.iter().enumerate() {
        println!("Line {} of {}", i, steps.len());
        if *on {
            let new_non_overlapping_cuboids = split_cuboid(&ons.iter().collect::<Vec<_>>(), cuboid);
            for c in new_non_overlapping_cuboids {
                ons.insert(c);
            }
        } else {
            let cuboids_to_split: Vec<Cuboid> = ons
                .iter()
                .filter(|c| c.intersects(cuboid))
                .map(|c| c.clone())
                .collect();
            ons = ons.into_iter().filter(|c| !c.intersects(cuboid)).collect();

            for c in cuboids_to_split {
                let new_non_overlapping_cuboids = split_cuboid(&vec![&cuboid], &c);
                for c in new_non_overlapping_cuboids {
                    ons.insert(c);
                }
            }
        }
    }
    let sum: i64 = ons.iter().map(|c| c.volume()).sum();
    println!("part2: {}", sum);
}

fn part1(steps: &Vec<(bool, Cuboid)>) {
    println!("Adding stuff");
    let mut points: HashSet<Point> = HashSet::new();

    for (i, (on, step)) in steps.iter().enumerate() {
        println!("processing line {}", i);
        for x in step.xmin..=step.xmax {
            if !(-50 <= x && x <= 50) {
                continue;
            }
            for y in step.ymin..=step.ymax {
                if !(-50 <= y && y <= 50) {
                    continue;
                }
                for z in step.zmin..=step.zmax {
                    if !(-50 <= z && z <= 50) {
                        continue;
                    }
                    let p = Point { x: x, y: y, z: z };
                    if *on {
                        points.insert(p);
                    } else {
                        points.remove(&p);
                    }
                }
            }
        }
    }

    println!("points inserted");
    println!("better: {}", points.len());
}

fn parse_file() -> Vec<(bool, Cuboid)> {
    let file_str = std::fs::read_to_string("./data.txt").unwrap();
    let lines = file_str.split("\n");
    let steps = lines
        .map(|line| {
            let first_split = line.split(" ").collect::<Vec<_>>();
            let on = if first_split[0] == "on" { true } else { false };
            let coord_split = first_split[1].split(",").collect::<Vec<_>>();
            let stepz = coord_split
                .iter()
                .map(|coord_str| {
                    coord_str[2..]
                        .split("..")
                        .map(|siffer_str| siffer_str.parse::<i64>().unwrap())
                        .collect::<Vec<i64>>()
                })
                .collect::<Vec<_>>();
            return (
                on,
                Cuboid {
                    xmin: stepz[0][0],
                    xmax: stepz[0][1],
                    ymin: stepz[1][0],
                    ymax: stepz[1][1],
                    zmin: stepz[2][0],
                    zmax: stepz[2][1],
                },
            );
        })
        .collect::<Vec<_>>();
    return steps;
}

#[derive(Hash, Debug, PartialEq, Eq)]
struct Point {
    x: i64,
    y: i64,
    z: i64,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_split() {
        assert_eq!(
            Cuboid {
                xmin: 3,
                xmax: 10,
                ymin: 3,
                ymax: 20,
                zmin: 300,
                zmax: 500
            }
            .split()
            .len(),
            8
        );
    }

    #[test]
    fn test_split_when_one_dim_same() {
        assert_eq!(
            Cuboid {
                xmin: 3,
                xmax: 10,
                ymin: 3,
                ymax: 3,
                zmin: 300,
                zmax: 500
            }
            .split()
            .len(),
            4
        );
    }

    #[test]
    fn test_volume() {
        assert_eq!(
            Cuboid {
                xmin: 0,
                xmax: 3,
                ymin: 0,
                ymax: 4,
                zmin: -1,
                zmax: 2
            }
            .volume(),
            4 * 5 * 4
        );
    }

    #[test]
    fn test_split_with_other() {
        let c1 = Cuboid {
            xmin: 0,
            xmax: 10,
            ymin: 0,
            ymax: 10,
            zmin: 0,
            zmax: 10,
        };
        let c2 = Cuboid {
            xmin: 0,
            xmax: 10,
            ymin: 0,
            ymax: 10,
            zmin: 0,
            zmax: 10,
        };
        assert_eq!(c1.split_with_other(&c2).len(), 0);
    }

    #[test]
    fn test_split_with_other_2() {
        let c1 = Cuboid {
            xmin: 0,
            xmax: 10,
            ymin: 0,
            ymax: 10,
            zmin: 0,
            zmax: 20,
        };
        let c2 = Cuboid {
            xmin: 0,
            xmax: 10,
            ymin: 0,
            ymax: 10,
            zmin: 0,
            zmax: 10,
        };
        assert_eq!(c1.split_with_other(&c2).len(), 1);
    }
}

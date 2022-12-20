#[derive(Clone, Debug)]
pub struct CustomLinkedList {
    pub nodes: Vec<Node>,
    pub index_of_zero: usize,
}

#[derive(Clone, Debug)]
pub struct Node {
    value: i64,
    prev: Option<*mut Node>,
    next: Option<*mut Node>,
}

impl CustomLinkedList {
    pub fn new(numbers: &Vec<i64>) -> CustomLinkedList {
        let mut nodes: Vec<Node> = numbers
            .iter()
            .map(|&number| Node {
                value: number,
                prev: None,
                next: None,
            })
            .collect();

        let num_nodes = nodes.len();
        for i in 0..num_nodes {
            let prev_index = if i as i64 - 1 == -1 {
                nodes.len() - 1
            } else {
                (i - 1).try_into().unwrap()
            };
            nodes[i].prev = Some(&mut nodes[prev_index]);
            nodes[i].next = Some(&mut nodes[(i + 1) % num_nodes])
        }

        let index_of_zero = numbers.iter().position(|&n| n == 0).unwrap();

        return CustomLinkedList {
            nodes: nodes,
            index_of_zero: index_of_zero,
        };
    }

    pub fn move_index(&mut self, index: usize) {
        let mut node_to_move: *mut Node = &mut self.nodes[index];
        unsafe {
            let value = (*node_to_move).value % (self.nodes.len() as i64 - 1);
            if value == 0 {
                return;
            }
            (*(*node_to_move).prev.unwrap()).next = Some((*node_to_move).next.unwrap());
            (*(*node_to_move).next.unwrap()).prev = Some((*node_to_move).prev.unwrap());

            let mut current_node: *mut Node = node_to_move;
            for _ in 0..value.abs() {
                if value < 0 {
                    current_node = (*current_node).prev.unwrap();
                } else {
                    current_node = (*current_node).next.unwrap();
                }
            }
            if value < 0 {
                let mut tmp = (*current_node).prev.unwrap();
                (*tmp).next = Some(node_to_move);
                (*node_to_move).prev = Some(tmp);
                (*node_to_move).next = Some(current_node);
                (*current_node).prev = Some(node_to_move);
            } else {
                let mut tmp = (*current_node).next.unwrap();
                (*tmp).prev = Some(node_to_move);
                (*node_to_move).next = Some(tmp);
                (*node_to_move).prev = Some(current_node);
                (*current_node).next = Some(node_to_move);
            }
        }
    }

    pub fn groove_coordinates(&self) -> i64 {
        unsafe {
            let mut sum: i64 = 0;
            let mut current_node: *const Node = &self.nodes[self.index_of_zero];
            for _ in 0..3 {
                for _ in 0..1000 {
                    current_node = (*current_node).next.unwrap();
                }
                sum += (*current_node).value;
            }
            return sum;
        }
    }
}

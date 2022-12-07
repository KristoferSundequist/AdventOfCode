#[derive(Debug)]
pub enum Line {
    Cd(String),
    Ls,
    File(i64, String),
    Dir(String),
}

pub fn parse_line(line: &str) -> Line {
    if line.starts_with("$ ") {
        if &line[2..4] == "cd" {
            return Line::Cd(line[5..].to_string());
        } else if &line[2..4] == "ls" {
            return Line::Ls;
        } else {
            unreachable!("unknown line starting with $");
        }
    }
    // else its a file or a dir
    if &line[0..3] == "dir" {
        return Line::Dir(line[4..].to_string());
    } else {
        let file_parts = line.split(" ").collect::<Vec<_>>();
        return Line::File(
            file_parts[0].parse::<i64>().unwrap(),
            file_parts[1].to_string(),
        );
    }
}

#[cfg(test)]
mod tests {
    // Note this useful idiom: importing names from outer (for mod tests) scope.
    use super::*;

    #[test]
    fn test_parse_line() {
        let cdline = "$ cd /";
        let lsline = "$ ls";
        let fileline = "268495 jgfbgjdb";
        let dirline = "dir ltcqgnc";

        assert!(matches!(parse_line(cdline), Line::Cd(_)));
        assert!(matches!(parse_line(lsline), Line::Ls));
        assert!(matches!(parse_line(fileline), Line::File(268495, _)));

        if let Line::File(size, name) = parse_line(fileline) {
            assert!(size == 268495);
            assert!(name == "jgfbgjdb".to_string());
        } else {
            assert!(false, "expected file line typ but it wasnt");
        }

        if let Line::Dir(str) = parse_line(dirline) {
            assert!(str == "ltcqgnc".to_string());
        } else {
            assert!(false, "expected dir line typ but it wasnt");
        }
    }
}

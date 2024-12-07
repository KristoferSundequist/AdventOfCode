var equations =
    File.ReadAllLines("data.txt")
    .Select(Equation.Parse)
    .ToList();

var part1 = equations.Where(eq => eq.IsMatch(false)).Sum(eq => eq.TestValue);
Console.WriteLine($"Part 1: {part1}");

var part2 = equations.Where(eq => eq.IsMatch(true)).Sum(eq => eq.TestValue);
Console.WriteLine($"Part 2: {part2}");
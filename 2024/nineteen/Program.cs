var patterns = File.ReadAllText("patterns.txt").Split(", ").ToArray();
var designs = File.ReadAllLines("designs.txt").ToArray();

var numPossible = designs.Where(design => Matcher.IsPossible(design, patterns)).ToArray();
Console.WriteLine(numPossible.Count());

var numTotalSolutions = designs.Sum(design => Matcher.GetNumSolutions(design, patterns));
Console.WriteLine(numTotalSolutions);
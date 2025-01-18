
var program = new List<short> { 2, 4, 1, 1, 7, 5, 1, 5, 0, 3, 4, 3, 5, 5, 3, 0 };

var computer1 = new Computer(56256477, 0, 0, program);
computer1.Run();
Console.WriteLine($"Part 1: {string.Join(",", computer1.Output)}");


var solutions = new HashSet<string> { "" }; ;
for (int i = 0; i < 16; i++)
{
    var newSolutions = new HashSet<string>();
    for (long j = 0; j < 1024; j++)
    {
        var binaryReprJ = Convert.ToString(j, 2).PadLeft(10, '0');
        foreach (var currentSolution in solutions)
        {
            var candidateBinary = $"{binaryReprJ}{currentSolution}";
            var candidate = Convert.ToInt64(candidateBinary, 2);
            var computer = new Computer(candidate, 0, 0, program);
            if (SpecialPurposeComputer.IsCorrect(candidate, i))
            {
                var lastThreeBits = candidateBinary[^(3 * (i + 1))..];
                newSolutions.Add(lastThreeBits);
            }
        }
    }
    solutions = newSolutions;
}
Console.WriteLine($"Part 2: {Convert.ToInt64(solutions.Min(), 2)}");
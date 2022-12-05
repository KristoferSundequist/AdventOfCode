
var stacks = System.IO.File.ReadAllLines("./stacks.txt");
var moves = System.IO.File.ReadAllLines("./moves.txt");

var parsedStacks = stacks[7].Select(_ => new Stack<char> { }).ToList();
foreach (var line in stacks.Reverse())
{
    for (var i = 0; i < line.Length; i++)
    {
        if (char.IsUpper(line[i]))
        {
            parsedStacks[i].Push(line[i]);
        }
    }
}
var parsedMoves = new List<Move> { };
foreach (var line in moves)
{
    var match = System.Text.RegularExpressions.Regex.Match(line, "move ([0-9]*) from ([0-9]*) to ([0-9]*)");
    parsedMoves.Add(new Move(int.Parse(match.Groups[1].ToString()), int.Parse(match.Groups[2].ToString()), int.Parse(match.Groups[3].ToString())));
}

foreach (var move in parsedMoves)
{
    var queue = new List<char> { };
    for (var i = 0; i < move.quantity; i++)
    {
        queue.Add(parsedStacks[move.from - 1].Pop());
    }
    for (var i = 0; i < move.quantity; i++)
    {
        parsedStacks[move.to - 1].Push(queue[move.quantity - i - 1]);
    }
}
foreach (var stack in parsedStacks)
{
    if (stack.Count > 0)
    {
        Console.Write(stack.Peek());
    }
    else
    {
        Console.Write(" ");
    }
}
Console.Write("\n");

record Move(int quantity, int from, int to);
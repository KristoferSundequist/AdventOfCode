var lines = System.IO.File.ReadAllLines("data.txt");

var illegalScore = lines.Select(line =>
{
    var stack = new Stack<char> { };
    foreach (var c in line)
    {
        if (c == '{')
        {
            stack.Push('}');
        }
        else if (c == '(')
        {
            stack.Push(')');
        }
        else if (c == '[')
        {
            stack.Push(']');
        }
        else if (c == '<')
        {
            stack.Push('>');
        }
        else if (stack.TryPeek(out var expected) && expected != c)
        {
            return c;
        }
        else
        {
            stack.Pop();
        }
    }
    return 'x';
}).Select(c => c switch
{
    ')' => 3,
    ']' => 57,
    '}' => 1197,
    '>' => 25137,
    _ => 0
}).Sum();

Console.WriteLine(illegalScore);
Console.WriteLine("--------------------");

var outcompleteScores = lines.Select(line =>
{
    var stack = new Stack<char> { };
    foreach (var c in line)
    {
        if (c == '{')
        {
            stack.Push('}');
        }
        else if (c == '(')
        {
            stack.Push(')');
        }
        else if (c == '[')
        {
            stack.Push(']');
        }
        else if (c == '<')
        {
            stack.Push('>');
        }
        else if (stack.TryPeek(out var expected) && expected != c)
        {
            return new Stack<char> { };
        }
        else
        {
            stack.Pop();
        }
    }
    return stack;
}).Select(stack =>
{
    long score = 0;
    foreach (var item in stack)
    {
        score = score * 5;
        score += item switch
        {
            ')' => 1,
            ']' => 2,
            '}' => 3,
            '>' => 4,
            _ => throw new Exception("Unexpected expected char")
        };
    }
    return score;
}).Where(score => score > 0).OrderBy(score => score).ToList();

//outcompleteScores.ForEach(Console.WriteLine);
Console.WriteLine(outcompleteScores[outcompleteScores.Count / 2]);

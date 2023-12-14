var input = File.ReadAllText("data.txt");
var parts = input.Split($"{Environment.NewLine}{Environment.NewLine}");
var path = parts[0];
var nodes = parts[1].Split(Environment.NewLine).Select(Node.FromString).ToDictionary(node => node.Id);

var result1 = GetSteps(path, nodes);
Console.WriteLine($"Result 1: {result1}");

var starts = nodes.Where(kvp => kvp.Key[2] == 'A').Select(kvp => kvp.Key).ToArray();
var ends = nodes.Where(kvp => kvp.Key[2] == 'Z').Select(kvp => kvp.Key).ToHashSet();
var cycleLengths = GetCycleLengths(path, nodes, starts, ends);
var result2 = GetCyclesIntersection(cycleLengths);
Console.WriteLine($"Result 2: {result2}");


long GetSteps(string path, Dictionary<string, Node> nodes)
{
    long stepsSoFar = 0;
    var currentNode = nodes["AAA"];
    int instructionIndex = 0;
    while (currentNode.Id != "ZZZ")
    {
        var direction = path[instructionIndex];

        stepsSoFar += 1;
        currentNode = path[instructionIndex] == 'R' ? nodes[currentNode.Right] : nodes[currentNode.Left];
        instructionIndex = (instructionIndex + 1) % path.Length;
    }
    return stepsSoFar;
}

long[] GetCycleLengths(string path, Dictionary<string, Node> nodes, string[] startingIds, HashSet<string> endIds)
{
    long stepsSoFar = 0;
    var currentNodes = startingIds.Select(id => nodes[id]).ToArray();
    int instructionIndex = 0;
    var visited = startingIds.Select(id => new Dictionary<string, long> { { id, 0 } }).ToArray();
    var cycleLengths = startingIds.Select(_ => (long)0).ToArray();
    while (!currentNodes.All(n => endIds.Contains(n.Id)))
    {
        var direction = path[instructionIndex];

        stepsSoFar += 1;
        for (var i = 0; i < currentNodes.Length; i++)
        {
            currentNodes[i] = path[instructionIndex] == 'R' ? nodes[currentNodes[i].Right] : nodes[currentNodes[i].Left];
            if (cycleLengths[i] == 0 && endIds.Contains(currentNodes[i].Id) && visited[i].TryGetValue(currentNodes[i].Id, out var lastVisited))
            {
                cycleLengths[i] = stepsSoFar - lastVisited;
            }
            if (!visited[i].ContainsKey(currentNodes[i].Id))
            {
                visited[i].Add(currentNodes[i].Id, stepsSoFar);
            }
        }
        instructionIndex = (instructionIndex + 1) % path.Length;
        if (cycleLengths.All(v => v > 0))
        {
            return cycleLengths;
        }
    }
    return [];
}

long GetCyclesIntersection(long[] cycleLengths)
{
    long stepSize = 1;
    for (var i = 0; i < 6; i++)
    {
        for (long j = 1; j < 100000000000000; j++)
        {
            var place = j * stepSize;
            if (cycleLengths[..(i + 1)].All(len => place % len == 0))
            {
                stepSize = place;
                break;
            }
        }
    }
    return stepSize;
}



public record Node(string Id, string Left, string Right)
{
    public static Node FromString(string rawInput)
    {
        return new Node(rawInput[0..3], rawInput[7..10], rawInput[12..15]);
    }
}
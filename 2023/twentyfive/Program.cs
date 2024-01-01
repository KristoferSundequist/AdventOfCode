using System.Collections.Immutable;

var lines = File.ReadAllLines("data.txt");
var graph = new Graph(lines);

var iters = 100;
var foundCount = 0;
for (var i = 0; i < iters; i++)
{
    if (i % 100 == 0)
    {
        Console.WriteLine($"{i} / {iters}");
    }
    var result = graph.GetCut(3);
    if (result != -1)
    {
        foundCount += 1;
        Console.WriteLine(result);
    }
}
Console.WriteLine($"found {foundCount} of {iters} times");

public class Graph
{

    public ImmutableDictionary<string, ImmutableHashSet<string>> AllEdges;

    public Graph(string[] lines)
    {
        var dict = new Dictionary<string, HashSet<string>>();
        foreach (var line in lines)
        {
            var split = line.Split(": ");
            var from = split[0];
            var tos = split[1].Split(" ");
            foreach (var to in tos)
            {
                if (dict.TryGetValue(from, out var fromValues))
                {
                    fromValues.Add(to);
                }
                else
                {
                    dict[from] = [to];
                }

                if (dict.TryGetValue(to, out var toValues))
                {
                    toValues.Add(from);
                }
                else
                {
                    dict[to] = [from];
                }
            }
        }
        AllEdges = dict.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableHashSet());
    }

    public int GetCut(int cutSize)
    {
        var startNode = AllEdges.ElementAt(Random.Shared.Next(0, AllEdges.Count)).Key;
        var mergedNodes = new HashSet<string> { startNode };
        var totalSize = AllEdges.Count;
        var outEdges = AllEdges[startNode].ToDictionary(str => str, _ => 1);
        while (mergedNodes.Count < totalSize - 1)
        {
            if (outEdges.Sum(kvp => kvp.Value) == cutSize)
            {
                return mergedNodes.Count * (totalSize - mergedNodes.Count);
            }

            //var nodeToMerge = GetNodeToMerge(outEdges, cutSize);
            var nodeToMerge = outEdges.MaxBy(kvp => kvp.Value).Key;
            outEdges.Remove(nodeToMerge);
            mergedNodes.Add(nodeToMerge);
            foreach (var newNode in AllEdges[nodeToMerge].Where(node => !mergedNodes.Contains(node)))
            {
                if (outEdges.TryGetValue(newNode, out var prevCount))
                {
                    outEdges[newNode] += 1;
                }
                else
                {
                    outEdges.Add(newNode, 1);
                }
            }
        }
        return -1;
    }
}
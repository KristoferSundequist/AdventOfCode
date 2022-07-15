var lines = System.IO.File.ReadAllLines("data.txt");

var edges = lines.Select(line =>
{
    var split = line.Split("-");
    return (split[0], split[1]);
}).ToList();

var adjTable = new Dictionary<string, List<string>> { };
foreach (var edge in edges)
{
    if (adjTable.TryGetValue(edge.Item1, out var items))
    {
        items.Add(edge.Item2);
    }
    else
    {
        adjTable.Add(edge.Item1, new List<string> { edge.Item2 });
    }

    if (adjTable.TryGetValue(edge.Item2, out var items2))
    {
        items2.Add(edge.Item1);
    }
    else
    {
        adjTable.Add(edge.Item2, new List<string> { edge.Item1 });
    }
}


var found = 0;
void Visit(List<string> visited, HashSet<string> forbidden, string current)
{
    if (current == "end")
    {
        //visited.ForEach(str => Console.Write(str + ", "));
        //Console.WriteLine("");
        found += 1;
        return;
    }
    var adjs = adjTable!.GetValueOrDefault(current)!;
    foreach (var adj in adjs)
    {
        if (!forbidden.Contains(adj))
        {
            var newVisited = DeepClone(visited);
            newVisited.Add(adj);

            var newForbidden = DeepClone(forbidden);

            if (IsSmall(adj))
            {
                newForbidden.Add(adj);
            }
            Visit(newVisited, newForbidden, adj);
        }
    }
}

Visit(new List<string> { "start" }, new HashSet<string> { "start" }, "start");

Console.WriteLine(found);



var paths = new List<List<string>> { };

void Visit2(List<string> visited, HashSet<string> forbidden, bool spent, string current)
{
    if (current == "end")
    {
        //visited.ForEach(str => Console.Write(str + ", "));
        //Console.WriteLine("");
        paths.Add(visited);
        return;
    }
    var adjs = adjTable!.GetValueOrDefault(current)!;
    foreach (var adj in adjs)
    {
        if (!forbidden.Contains(adj))
        {
            var newForbidden = DeepClone(forbidden);
            var newVisited = DeepClone(visited);
            newVisited.Add(adj);

            if (IsSmall(adj))
            {
                if (spent == false)
                {
                    Visit2(DeepClone(newVisited), DeepClone(forbidden), true, adj);
                    newForbidden.Add(adj);
                    Visit2(DeepClone(newVisited), newForbidden, false, adj);
                }
                else
                {
                    newForbidden.Add(adj);
                    Visit2(newVisited, newForbidden, true, adj);
                }
            }
            else
            {
                Visit2(newVisited, newForbidden, spent, adj);
            }
        }
    }
}

Visit2(new List<string> { "start" }, new HashSet<string> { "start" }, false, "start");

Console.WriteLine(paths.Select(path => String.Join("|", path)).Distinct().Count());

bool IsSmall(string str) => Char.IsLower(str[0]);

T DeepClone<T>(T obj) =>
    System.Text.Json.JsonSerializer.Deserialize<T>(System.Text.Json.JsonSerializer.Serialize(obj))!;

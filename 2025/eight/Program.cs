/*
162,817,812 and 425,690,689 2,1,1,1...
162,817,812 and 431,825,988 3,1,1,1,1....
906,360,560 and 805,96,715 3,2,1,1,1...
431,825,988 and 425,690,689 nothing happens
*/

//var (filepath, numToMerge) = ("testinput.txt", 10);
var (filepath, numToMerge) = ("input.txt", 1000);
var lines = File.ReadAllLines(filepath);
var junctionBoxToCircuit = lines.Select(JunctionBox.FromString).ToDictionary(jb => jb, jb => Guid.NewGuid());

var pairs = new List<(JunctionBox, JunctionBox, double)>();

var junctionBoxList = junctionBoxToCircuit.Keys.ToList();
for (var jb1i = 0; jb1i < junctionBoxList.Count; jb1i++)
{
    for (var jb2i = jb1i + 1; jb2i < junctionBoxList.Count; jb2i++)
    {
        var jb1 = junctionBoxList[jb1i];
        var jb2 = junctionBoxList[jb2i];
        var distance = jb1.Distance(jb2);
        pairs.Add((jb1, jb2, distance));
    }
}

pairs = pairs.OrderBy(p => p.Item3).ToList();

var i = 0;
while (true)
{
    var (jb1, jb2, distance) = pairs[i];
    var circuitFrom = junctionBoxToCircuit[jb2];

    foreach (var kvp in junctionBoxToCircuit.Where(kvp => kvp.Value == circuitFrom))
    {
        junctionBoxToCircuit[kvp.Key] = junctionBoxToCircuit[jb1];
    }

    var shouldStop = junctionBoxToCircuit.Values.Distinct().Count() == 1;

    if (shouldStop)
    {
        Console.WriteLine($"{jb1} * {jb2} = {jb1.X * jb2.X}");
        break;
    }
    i++;
}

/* Console.WriteLine($"Circuit sizes: {String.Join(",", junctionBoxToCircuit.GroupBy(kvp => kvp.Value).Select(g => g.Count()).OrderByDescending(count => count))}");

var largestThree = junctionBoxToCircuit.GroupBy(kvp => kvp.Value)
    .Select(g => g.Count())
    .OrderByDescending(count => count)
    .Take(3)
    .ToArray();

var product = largestThree[0] * largestThree[1] * largestThree[2];
Console.WriteLine($"Product of sizes of largest three circuits {largestThree[0]} * {largestThree[1]} * {largestThree[2]} = {product}"); */

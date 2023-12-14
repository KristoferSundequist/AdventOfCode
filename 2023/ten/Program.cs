var input = File.ReadAllLines("./data.txt").ToArray();

var diagram = new Diagram(input, false);

var result1 = diagram.GetFurthestDistanceFromStart();
Console.WriteLine($"Result 1: {result1}");

diagram.Draw();


Console.WriteLine("-------------------------");
var expandedDiagram = new Diagram(input, true);
var result2 = expandedDiagram.GetNumEnclosed();
Console.WriteLine($"Result 2: {result2}");


// 616 too high
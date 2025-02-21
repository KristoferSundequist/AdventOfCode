var input = File.ReadAllLines("connections.txt").ToArray();
var network = new Network(input);
var numberOfCliques = network.FindThreeCliques("t");
Console.WriteLine($"Number of 3-cliques: {numberOfCliques}");

network.Visualize();

var found = new HashSet<string>();
network.GetBiggestClique(found, ["ka"]); // as all nodes are only connected to 13 other I just tried some and found that this one in part of a clique of size 13
var biggestClique = found.MaxBy(c => c.Length);
Console.WriteLine($"Biggest clique: {biggestClique}");
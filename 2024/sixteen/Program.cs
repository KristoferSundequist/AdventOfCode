var lines = File.ReadAllLines("data.txt").Select(x => x.ToCharArray()).ToArray();

var maze = new Maze(lines);

Console.WriteLine(maze.GetLowestScore());
Console.WriteLine(maze.GetNumTilesInShortestPaths());
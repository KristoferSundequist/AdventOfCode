var shapes = File.ReadAllText("shapes.txt")
    .Split($"{Environment.NewLine}{Environment.NewLine}")
    .Select(shape => shape.Count(c => c == '#'))
    .ToList();

var regions = File.ReadAllLines("regions.txt")
    .Select(line => Region.FromString(line))
    .ToList();

var numRegionsThatFit = regions.Count(region => region.CanFitShapes(shapes));
Console.WriteLine($"Number of regions that can fit shapes: {numRegionsThatFit}");
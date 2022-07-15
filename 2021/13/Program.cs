var dotlines = System.IO.File.ReadAllLines("data.txt");
var foldlines = System.IO.File.ReadAllLines("folds.txt");

var folds = foldlines.Select(line =>
{
    var split = line.Split("=");
    return new Fold(split[0].Last(), Int32.Parse(split[1]));
}).ToList();

var dots = dotlines.Select(line =>
{
    var split = line.Split(",");
    return new Point(Int32.Parse(split[0]), Int32.Parse(split[1]));
}).ToList();

var paper = dots.ToHashSet();

var firstFold = folds.First();

HashSet<Point> Fold(HashSet<Point> dots, Fold fold)
{
    var newPaper = new HashSet<Point> { };
    foreach (var dot in dots)
    {
        if (fold.Dim == 'y')
        {
            if (dot.Y < fold.Place)
            {
                newPaper.Add(dot);
            }
            else if (dot.Y > fold.Place)
            {
                var newY = fold.Place - (dot.Y - fold.Place);
                newPaper.Add(dot with { Y = newY });
            }
        }
        else if (fold.Dim == 'x')
        {
            if (dot.X < fold.Place)
            {
                newPaper.Add(dot);
            }
            else if (dot.X > fold.Place)
            {
                var newX = fold.Place - (dot.X - fold.Place);
                newPaper.Add(dot with { X = newX });
            }
        }
    }
    return newPaper;
}

var foldedPaper = Fold(paper, firstFold);
Console.WriteLine(foldedPaper.Count);

// copy
var foldpaper = System.Text.Json.JsonSerializer.Deserialize<HashSet<Point>>(System.Text.Json.JsonSerializer.Serialize(paper))!;
foreach (var fold in folds)
{
    foldpaper = Fold(foldpaper, fold);
}

Console.WriteLine(foldpaper.Count);

var grid =
    from y in Enumerable.Range(0, foldpaper.Max(p => p.Y))
    from x in Enumerable.Range(0, foldpaper.Max(p => p.X))
    select foldpaper.Contains(new Point(x, y)) ? '#' : '.';

foreach (var y in Enumerable.Range(0, foldpaper.Max(p => p.Y)+1))
{
    foreach (var x in Enumerable.Range(0, foldpaper.Max(p => p.X)+1))
    {
        Console.Write(foldpaper.Contains(new Point(x, y)) ? '#' : '.');
    }
    Console.Write("\n");
}


record Point(int X, int Y);
record Fold(char Dim, int Place);
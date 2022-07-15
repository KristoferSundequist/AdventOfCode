var lines = System.IO.File.ReadAllLines("data.txt").Select(l => Int32.Parse(l)).ToList();

var increases = 0;

for (var i = 1; i < lines.Count(); i++)
{
    if (lines[i] > lines[i - 1])
    {
        increases += 1;
    }
}

Console.WriteLine(increases);

var increases2 = 0;

for (var i = 3; i < lines.Count(); i++)
{
    if (lines[i] + lines[i - 1] + lines[i - 2] > lines[i - 1] + lines[i - 2] + lines[i - 3])
    {
        increases2 += 1;
    }
}

Console.WriteLine(increases2);


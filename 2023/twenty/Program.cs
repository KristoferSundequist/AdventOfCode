var lines = File.ReadAllLines("./data.txt");

var computer = new Computer(lines);
var iterations = 20000;
for (var i = 0; i < iterations; i++)
{
    if (i % (iterations / 10) == 0)
    {
        Console.WriteLine($"{(i / (float)iterations) * 100}%");
    }

    computer.PushButton(i, false);
}
Console.WriteLine($"Result 1: {computer.GetScore()}");
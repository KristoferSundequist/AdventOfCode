var lines = File.ReadAllLines("./data.txt");
var games = lines.Select(Game.FromString);

Console.WriteLine($"Part 1: {Part1(games)}");
Console.WriteLine($"Part 2: {Part2(games)}");

int Part1(IEnumerable<Game> games) => games.Where(game => game.IsPossible(12, 13, 14)).Select(g => g.Id).Sum();
int Part2(IEnumerable<Game> games) => games.Select(game => game.GetPower()).Sum();

public record Game(int Id, Hand[] Hands)
{
    public bool IsPossible(int red, int green, int blue) => Hands.All(hand => hand.red <= red && hand.green <= green && hand.blue <= blue);

    public int GetPower() => Hands.Max(hand => hand.red) * Hands.Max(hand => hand.green) * Hands.Max(hand => hand.blue);

    public static Game FromString(string str)
    {
        var parts = str.Split(":");
        var id = int.Parse(parts[0].Split(" ")[1]);
        var hands = parts[1].Split(";").Select(Hand.FromString).ToArray();
        return new Game(id, hands);
    }
}
public record Hand(int red, int green, int blue)
{
    public static Hand FromString(string str)
    {
        var colorParts = str.Split(",").Select(colorPart => colorPart.Trim());
        var colorNumbers = new Dictionary<string, int> { };
        foreach (var colorPart in colorParts)
        {
            if (colorPart.Split(" ") is [string valueStr, string color] && int.TryParse(valueStr, out int value))
            {
                colorNumbers[color] = value;
            }
            else
            {
                throw new Exception($"Parse error on: ${colorPart}");
            }
        }
        return new Hand(
            colorNumbers.TryGetValue("red", out var red) ? red : 0,
            colorNumbers.TryGetValue("green", out var green) ? green : 0,
            colorNumbers.TryGetValue("blue", out var blue) ? blue : 0
        );
    }
}
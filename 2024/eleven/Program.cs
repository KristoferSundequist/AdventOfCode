var stones = new List<string> { "1117", "0", "8", "21078", "2389032", "142881", "93", "385" };

var blinks = 75;
var currentStonesTally = stones.ToDictionary(x => x, _ => (long)1);
for (var i = 0; i < blinks; i++)
{
    var newStonesTally = new Dictionary<string, long>();
    foreach (var currentStone in currentStonesTally)
    {
        foreach (var newStone in TransformStone(currentStone.Key))
        {
            if (newStonesTally.ContainsKey(newStone))
            {
                newStonesTally[newStone] += currentStone.Value;
            }
            else
            {
                newStonesTally[newStone] = currentStone.Value;
            }
        }
    }
    currentStonesTally = newStonesTally;
}

Console.WriteLine(currentStonesTally.Values.Sum());

List<string> TransformStone(string stone)
{
    if (stone == "0")
    {
        return ["1"];
    }
    else if (stone.Length % 2 == 0)
    {
        var mid = stone.Length / 2;
        return [stone[..mid], long.Parse(stone[mid..]).ToString()];
    }
    else
    {
        return [(long.Parse(stone) * 2024).ToString()];
    }
}
var input = File.ReadAllLines("./data.txt");
var hands = input.Select(str => new Hand(str)).Order();

var totalWinnings = hands.Select((hand, rank) => hand.Bid * (rank + 1)).Sum();
Console.WriteLine($"Result: {totalWinnings}");


//254077318 too high
//252166561 too high
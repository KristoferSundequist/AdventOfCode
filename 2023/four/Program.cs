
var input = File.ReadAllLines("./data.txt");
var cards = input.Select(line => new Card(line)).ToArray();

var result1 = cards.Sum(card => card.GetWinnings());
Console.WriteLine($"Result 1: {result1}");

var result2 = CardCopier.GetTotalNumberOfCards(cards);
Console.WriteLine($"Result 2: {result2}");

public static class CardCopier
{
    public static long GetTotalNumberOfCards(Card[] cards)
    {
        var counts = Enumerable.Range(0, cards.Length).Select(_ => 1).ToArray();

        for (var i = 0; i < counts.Length; i++)
        {
            var nWinnings = cards[i].GetNumMatchingNumbers();
            for (var j = 1; j <= nWinnings; j++)
            {
                counts[i + j] += counts[i];
            }
        }
        return counts.Sum();
    }
}

public class Card
{
    public int CardNumber { get; set; }
    public HashSet<int> WinningNumbers { get; init; }
    public List<int> NumbersYouHave { get; init; }

    public Card(string rawLine)
    {
        var split = rawLine.Split(":");
        CardNumber = int.Parse(split[0].Split(" ").Last());
        if (split[1].Split("|") is [string winningNumbersStr, string numbersYouHave])
        {
            WinningNumbers = winningNumbersStr.Split(" ").Where(str => str != "").Select(int.Parse).ToHashSet();
            NumbersYouHave = numbersYouHave.Split(" ").Where(str => str != "").Select(int.Parse).ToList();
        }
        else
        {
            throw new Exception("parse error");
        }
    }
    public int GetNumMatchingNumbers() => NumbersYouHave.Where(n => WinningNumbers.Contains(n)).Count();

    public int GetWinnings()
    {
        return (int)Math.Pow(2, GetNumMatchingNumbers() - 1);
    }
}
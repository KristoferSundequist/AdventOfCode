public record Hand(string Input) : IComparable
{
    public int Bid = int.Parse(Input.Split(" ")[1]);
    public Dictionary<char, int> LabelCount = Input.Split(" ")[0].ToCharArray().ToLookup(v => v).ToDictionary(kvp => kvp.Key, kvp => kvp.Count());
    public Card[] Cards = Input.Split(" ")[0].Select(c => new Card(c)).ToArray();
    public bool IsFiveOfAKind() => LabelCount.Count == 1;
    public bool IsFourOfAKind() => LabelCount.Any(kvp => kvp.Value == 4);
    public bool IsFullHouse() => LabelCount.Count == 2 && LabelCount.Any(kvp => kvp.Value == 3);
    public bool IsThreeOfAKind() => LabelCount.Any(kvp => kvp.Value == 3) && LabelCount.Count == 3;
    public bool IsTwoPair() => LabelCount.Count == 3 && LabelCount.Any(kvp => kvp.Value == 2) && LabelCount.Any(kvp => kvp.Value == 1);
    public bool IsOnePair() => LabelCount.Count == 4 && LabelCount.Any(kvp => kvp.Value == 2);
    public bool IsHighCard() => LabelCount.Count == 5;

    public int GetTypeStrength()
    {
        if (IsFiveOfAKind()) return 7;
        if (IsFourOfAKind()) return 6;
        if (IsFullHouse()) return 5;
        if (IsThreeOfAKind()) return 4;
        if (IsTwoPair()) return 3;
        if (IsOnePair()) return 2;
        if (IsHighCard()) return 1;
        throw new Exception("This shouldnt happen");
    }

    private int GetTypeStrengthWithJoker()
    {
        if (IsFiveOfAKind())
        {
            return 7;
        }
        if (LabelCount.TryGetValue('J', out var nJokers))
        {
            var biggestNonJoker = LabelCount.Where(kvp => kvp.Key != 'J').MaxBy(kvp => kvp.Value).Key;
            var rewrittenHand = Input.Split(" ")[0].Select(c => c == 'J' ? biggestNonJoker : c);
            var rewrittenHandStr = String.Concat(rewrittenHand);
            return new Hand($"{rewrittenHandStr} 123").GetTypeStrength();
        }
        else
        {
            return GetTypeStrength();
        }
    }

    private int CompareToLexicographically(Hand other)
    {
        for (var i = 0; i < 5; i++)
        {
            var result = this.Cards[i].CompareTo(other.Cards[i]);
            if (result != 0)
            {
                return result;
            }
        }
        return 0;
    }

    private int CompareToLexicographicallyWithJoker(Hand other)
    {
        for (var i = 0; i < 5; i++)
        {
            var result = this.Cards[i].CompareToWithJoker(other.Cards[i]);
            if (result != 0)
            {
                return result;
            }
        }
        return 0;
    }

    public int CompareToNoJoker(object? obj)
    {
        if (obj is Hand other)
        {
            var thisTypeStrength = GetTypeStrength();
            var otherTypeStrength = other.GetTypeStrength();

            if (thisTypeStrength > otherTypeStrength)
            {
                return 1;
            }
            if (thisTypeStrength < otherTypeStrength)
            {
                return -1;
            }
            return this.CompareToLexicographically(other);
        }
        else
        {
            throw new Exception("compare to what");
        }
    }

    public int CompareTo(object? obj)
    {
        if (obj is Hand other)
        {
            var thisTypeStrength = GetTypeStrengthWithJoker();
            var otherTypeStrength = other.GetTypeStrengthWithJoker();

            if (thisTypeStrength > otherTypeStrength)
            {
                return 1;
            }
            if (thisTypeStrength < otherTypeStrength)
            {
                return -1;
            }
            return this.CompareToLexicographicallyWithJoker(other);
        }
        else
        {
            throw new Exception("compare to what");
        }
    }
}
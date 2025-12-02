public record Range(long Start, long End)
{
    public long Difference() => End - Start;

    public IEnumerable<long> GetFakeNumbersInRange(Func<long, bool> isFake)
    {
        for (long number = Start; number <= End; number++)
        {
            if (isFake(number))
            {
                yield return number;
            }
        }
    }

    public static bool IsFake(long number)
    {
        var numberStr = number.ToString();

        if (numberStr.Length % 2 != 0)
        {
            return false;
        }

        var leftPart = numberStr.Substring(0, numberStr.Length / 2);
        var rightPart = numberStr.Substring(numberStr.Length / 2);

        return leftPart == rightPart;
    }

    public static bool IsFake2(long number)
    {
        var numberStr = number.ToString();

        for (int i = 0; i < numberStr.Length / 2; i++)
        {
            if (numberStr.Length % (i+1) != 0)
            {
                continue;
            }
            var patternToLookFor = numberStr.Substring(0, i + 1);
            var numberOfRepeats = numberStr.Length / patternToLookFor.Length;
            var repeatedPattern = patternToLookFor.Repeat(numberOfRepeats);
            if (repeatedPattern == numberStr)
            {
                return true;
            }
        }
        return false;
    }
}
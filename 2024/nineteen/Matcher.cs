public static class Matcher
{
    public static bool IsPossible(string input, string[] patterns) => IsPossible(input, patterns, []);
    private static bool IsPossible(string input, string[] patterns, HashSet<string> found)
    {
        if (found.Contains(input))
        {
            return false;
        }

        if (input.Length == 0)
        {
            return true;
        }

        found.Add(input);
        return patterns.Any(pattern => TryMatch(input, pattern, out var remaining) && IsPossible(remaining, patterns, found));
    }

    public static long GetNumSolutions(string input, string[] patterns) => GetNumSolutions(input, patterns, []);
    private static long GetNumSolutions(string input, string[] patterns, Dictionary<string, long> memo)
    {
        if (memo.TryGetValue(input, out var result))
        {
            return result;
        }

        long numSolutions = 0;
        foreach (var pattern in patterns)
        {
            if (TryMatch(input, pattern, out var remaining))
            {
                if (remaining.Length == 0)
                {
                    numSolutions++;
                }
                else
                {
                    numSolutions += GetNumSolutions(remaining, patterns, memo);
                }
            }
        }

        memo[input] = numSolutions;
        return numSolutions;
    }

    private static bool TryMatch(string input, string pattern, out string remaining)
    {
        if (input.StartsWith(pattern))
        {
            remaining = input.Substring(pattern.Length);
            return true;
        }
        else
        {
            remaining = input;
            return false;
        }
    }
}
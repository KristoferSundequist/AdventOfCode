public static class SpecialPurposeComputer
{
    private static List<short> Program = new List<short> { 2, 4, 1, 1, 7, 5, 1, 5, 0, 3, 4, 3, 5, 5, 3, 0 };

    public static bool IsCorrect(long input, int numOutputs)
    {
        long A = input;

        for (var i = 0; i < numOutputs; i++)
        {
            var leftside = A >> (int)((A % 8) ^ 1);
            var output = (A ^ leftside ^ 4) % 8;

            if (output != Program[i])
            {
                return false;
            }
            A >>= 3;
        }
        return true;
    }
}
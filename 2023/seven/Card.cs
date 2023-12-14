public record Card(char Label)
{
    private const string Labels = "AKQJT98765432";
    private const string LabelsWithJoker = "AKQT98765432J";

    public int CompareTo(Card other)
    {
        var thisIndex = Labels.IndexOf(this.Label);
        var otherIndex = Labels.IndexOf(other.Label);

        if (thisIndex < otherIndex)
        {
            return 1;
        }
        if (thisIndex == otherIndex)
        {
            return 0;
        }
        return -1;
    }

    public int CompareToWithJoker(Card other)
    {
        var thisIndex = LabelsWithJoker.IndexOf(this.Label);
        var otherIndex = LabelsWithJoker.IndexOf(other.Label);

        if (thisIndex < otherIndex)
        {
            return 1;
        }
        if (thisIndex == otherIndex)
        {
            return 0;
        }
        return -1;
    }
}
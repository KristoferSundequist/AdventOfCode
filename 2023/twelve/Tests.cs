using NUnit.Framework;

[TestFixture]
public class Tests
{
    [TestCase("???.### 1,1,3", 1)]
    [TestCase(".??..??...?##. 1,1,3", 4)]
    [TestCase("?#?#?#?#?#?#?#? 1,3,1,6", 1)]
    [TestCase("????.#...#... 4,1,1", 1)]
    [TestCase("????.######..#####. 1,6,5", 4)]
    [TestCase("?###???????? 3,2,1", 10)]
    public void Simple(string line, long answer)
    {
        var record = ConditionRecord.FromString(line).Unfold(1);
        var numArrangements = Utils.GetNumArrangements(record.SpringConditions, record.DamageGroupSizes, false, 0, new());

        Assert.That(numArrangements, Is.EqualTo(answer));
    }
}

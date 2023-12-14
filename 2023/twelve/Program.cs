var lines = File.ReadAllLines("./data.txt");
var records = lines.Select(ConditionRecord.FromString).ToArray();

var totalArrangements = records.Sum(r => Utils.GetNumArrangements(r.SpringConditions, r.DamageGroupSizes, false, 0, new()));
Console.WriteLine($"Result 1: {totalArrangements}");


long totalUnfoldedArrangements = 0;
for (var i = 0; i < records.Length; i++)
{
    //Console.WriteLine($"{i} / {records.Length}");
    var unfoldedRecord = records[i].Unfold(5);
    //Console.WriteLine(Utils.GetHash(unfoldedRecord.SpringConditions, unfoldedRecord.DamageGroupSizes));
    var result = Utils.GetNumArrangements(unfoldedRecord.SpringConditions, unfoldedRecord.DamageGroupSizes, false, 0, new());
    //Console.WriteLine(result);
    totalUnfoldedArrangements += result;
}
Console.WriteLine($"Result 2: {totalUnfoldedArrangements}");

public static class Utils
{
    public static void MaybeWriteIndented(bool shouldWrite, int indent, string msg)
    {
        if (shouldWrite)
        {
            Console.WriteLine($"{string.Join("", Enumerable.Range(0, indent * 2).Select(_ => ' ').ToArray())} {msg}");
        }
    }

    public static bool IsPotentiallyDamage(ReadOnlySpan<char> str)
    {
        for (var i = 0; i < str.Length; i++)
        {
            if (str[i] == '.')
            {
                return false;
            }
        }
        return true;
    }

    public static string GetHash(ReadOnlySpan<char> SpringConditions, ReadOnlySpan<int> DamageGroupSizes, bool used)
    {
        return $"{SpringConditions.ToString()} | {string.Join(',', DamageGroupSizes.ToArray())} | {used}";
    }

    public static long GetNumArrangements(ReadOnlySpan<char> SpringConditions, ReadOnlySpan<int> DamageGroupSizes, bool used, int indent, Dictionary<string, long> memo)
    {
        var verbose = false;
        if (memo.TryGetValue(GetHash(SpringConditions, DamageGroupSizes, used), out var memoResult))
        {
            Utils.MaybeWriteIndented(verbose, indent, $"USING MEMOED VALUE HASH '{GetHash(SpringConditions, DamageGroupSizes, used)}' FOR VALUE {memoResult}");
            return memoResult;
        }

        Utils.MaybeWriteIndented(verbose, indent, SpringConditions.ToString());
        if (SpringConditions.Length == 0 && DamageGroupSizes.Length == 0)
        {
            Utils.MaybeWriteIndented(verbose, indent, "returning 1");
            return 1;
        }

        if (SpringConditions.Length == 0)
        {
            return 0;
        }

        if (SpringConditions[0] == '.')
        {
            Utils.MaybeWriteIndented(verbose, indent, "skipping .");
            return GetNumArrangements(SpringConditions[1..], DamageGroupSizes, false, indent, memo);
        }

        if (used && SpringConditions[0] == '#')
        {
            return 0;
        }

        if (used && SpringConditions[0] == '?')
        {
            Utils.MaybeWriteIndented(verbose, indent, "skipping since just used");
            return GetNumArrangements(SpringConditions[1..], DamageGroupSizes, false, indent, memo);
        }

        if (DamageGroupSizes is [int v, ..] && SpringConditions.Length >= v && Utils.IsPotentiallyDamage(SpringConditions[..v]))
        {
            Utils.MaybeWriteIndented(verbose, indent, "branch using damage group");
            var useDamageResult = GetNumArrangements(SpringConditions[v..], DamageGroupSizes[1..], true, indent + 1, memo);
            memo[GetHash(SpringConditions[v..], DamageGroupSizes[1..], true)] = useDamageResult;

            if (SpringConditions[0] == '?')
            {
                Utils.MaybeWriteIndented(verbose, indent, "branch dont use damage group");
                var dontUseDamageResult = GetNumArrangements(SpringConditions[1..], DamageGroupSizes, false, indent + 1, memo);
                memo[GetHash(SpringConditions[1..], DamageGroupSizes, false)] = dontUseDamageResult;
                return useDamageResult + dontUseDamageResult;
            }
            else
            {
                return useDamageResult;
            }
        }

        if (SpringConditions[0] == '?')
        {
            Utils.MaybeWriteIndented(verbose, indent, "skipping..");
            return GetNumArrangements(SpringConditions[1..], DamageGroupSizes, false, indent, memo);
        }

        return 0;
    }
}

public class ConditionRecord
{
    public required string SpringConditions { get; init; }
    public required int[] DamageGroupSizes { get; init; }

    public static ConditionRecord FromString(string input)
    {
        if (input.Split(" ") is [string springConditions, string groupStr])
        {
            return new ConditionRecord
            {
                SpringConditions = springConditions,
                DamageGroupSizes = groupStr.Split(",").Select(int.Parse).ToArray()
            };
        }
        else
        {
            throw new Exception("parse error");
        }
    }

    public ConditionRecord Unfold(int n) => new ConditionRecord
    {
        SpringConditions = string.Join("?", Enumerable.Range(0, n).Select(_ => SpringConditions)),
        DamageGroupSizes = Enumerable.Range(0, n).SelectMany(_ => DamageGroupSizes).ToArray()
    };

    public override string ToString() => $"{SpringConditions} - {string.Join(',', DamageGroupSizes)}";
}

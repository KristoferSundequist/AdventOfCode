using System.Text;

var lines = System.IO.File.ReadAllLines("data.txt");
//var start = "NNCB";
var start = "KHSNHFKVVSVPSCVHBHNP";
Dictionary<string, string> rules = lines.Select(line => line.Split(" -> ")).ToDictionary(split => split[0], split => split[1]);
//var newRules = rules.ToDictionary(kvp => kvp.Key, kvp => $"{kvp.Key[0]}{kvp.Value}{kvp.Key[1]}");

var nextStr = start;

for (var i = 0; i < 10; i++)
{
    Console.WriteLine($"Iteration: {i}");
    nextStr = Next(nextStr);
}

var tally = CountEm(nextStr);
Console.WriteLine($"Part1: {tally.MaxBy(kvp => kvp.Value).Value - tally.MinBy(kvp => kvp.Value).Value}");


// PART 2
Console.WriteLine("PART2");

var initCharCount = CountEm(start);
var initPaircount = CountPairs(start);
var initTally = new Tally(initPaircount, initCharCount);

for (var i = 0; i < 40; i++)
{
    initTally = GoodNext(initTally);
    // Console.WriteLine($"After iteration: {i + 1}");
    // Console.WriteLine($"------------ pair count -------------");
    // foreach (var qwe in initTally.pairCount)
    // {
    //     Console.WriteLine(qwe);
    // }
    // Console.WriteLine($"------------ char count -------------");
    // foreach (var qwe in initTally.charCount)
    // {
    //     Console.WriteLine(qwe);
    // }
}

/*
Template:     NNCB
After step 1: NCNBCHB
After step 2: NBCCNBBBCBHCB
After step 3: NBBBCNCCNBBNBNBBCHBHHBCHB
After step 4: NBBNBNBBCCNBCNCCNBBNBBNBBBNBBNBBCBHCBHHNHCBBCBHCB
*/

Console.WriteLine($"Part2: {initTally.charCount.MaxBy(kvp => kvp.Value).Value - initTally.charCount.MinBy(kvp => kvp.Value).Value}");

string Next(string current)
{
    var insertions = new List<string> { };
    for (var i = 0; i < current.Length - 1; i++)
    {
        var key = $"{current[i]}{current[i + 1]}";
        if (rules.TryGetValue(key, out var charToInsert))
        {
            insertions.Add(charToInsert);
        }
        else
        {
            insertions.Add("");
        }
    }

    var newStr = new StringBuilder();
    for (var i = 0; i < current.Length - 1; i++)
    {
        newStr.Append(current[i]);
        newStr.Append(insertions[i]);
    }
    newStr.Append(current.Last());

    return newStr.ToString();
}

Tally GoodNext(Tally current)
{
    var newPairCount = new Dictionary<string, long> { };
    var newCharCount = DeepCopy(current.charCount);

    foreach (var pair in current.pairCount)
    {
        // if there is a rule
        if (rules.TryGetValue(pair.Key, out var insertion))
        {
            // add the new char to charcount
            if (newCharCount.TryGetValue(insertion, out var currentCount))
            {
                newCharCount[insertion] = currentCount + pair.Value;
            }
            else
            {
                newCharCount.Add(insertion, pair.Value);
            }

            // also add two new pairs based on rule
            var newPair1 = $"{pair.Key[0]}{insertion}";
            if (newPairCount.TryGetValue(newPair1, out var currentCount1))
            {
                newPairCount[newPair1] = currentCount1 + pair.Value;
            }
            else
            {
                newPairCount.Add(newPair1, pair.Value);
            }

            var newPair2 = $"{insertion}{pair.Key[1]}";
            if (newPairCount.TryGetValue(newPair2, out var currentCount2))
            {
                newPairCount[newPair2] = currentCount2 + pair.Value;
            }
            else
            {
                newPairCount.Add(newPair2, pair.Value);
            }
        }

        // if no rule, just add old pair value
        else
        {
            if (newPairCount.TryGetValue(pair.Key, out var currentCount))
            {
                newPairCount[pair.Key] = currentCount + pair.Value;
            }
            else
            {
                newPairCount.Add(pair.Key, pair.Value);
            }
        }
    }
    return new Tally(newPairCount, newCharCount);
}

Dictionary<string, long> CountEm(string str)
{
    var tally = new Dictionary<string, long> { };
    foreach (var c in str)
    {
        if (tally.TryGetValue(c.ToString(), out var currentCount))
        {
            tally[c.ToString()] = currentCount + 1;
        }
        else
        {
            tally.Add(c.ToString(), 1);
        }
    }
    return tally;
}

Dictionary<string, long> CountPairs(string str)
{
    var tally = new Dictionary<string, long> { };
    for (var i = 0; i < str.Length - 1; i++)
    {
        var pair = $"{str[i]}{str[i + 1]}";
        if (tally.TryGetValue(pair, out var currentCount))
        {
            tally[pair] = currentCount + 1;
        }
        else
        {
            tally.Add(pair, 1);
        }
    }
    return tally;
}

T DeepCopy<T>(T obj) => System.Text.Json.JsonSerializer.Deserialize<T>(System.Text.Json.JsonSerializer.Serialize(obj)!)!;

record Tally(Dictionary<string, long> pairCount, Dictionary<string, long> charCount);
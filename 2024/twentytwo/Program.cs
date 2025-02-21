var secretInputs = File.ReadAllLines("secrets.txt").Select(long.Parse).ToArray();
var secrets = secretInputs.Select(s => new Secret(s, 2000)).ToArray();
var result = secrets.Sum(s => s.GetNthSecretNumber(2000));
Console.WriteLine(result);



var allDiffKeys = secrets.SelectMany(s => s.GetAllDiffKeys()).ToHashSet();
var result2 = allDiffKeys.Max(k => secrets.Sum(s => s.GetPriceByDiff(k)));
Console.WriteLine(result2);
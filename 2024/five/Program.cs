var updates = File.ReadAllLines("updates.txt")
    .Select(Update.FromString)
    .ToList();

var rawOrderingRules = File.ReadAllLines("ordering.txt")
    .Select(OrderingRule.FromString)
    .ToList();

var orderingRules = new OrderingRules(rawOrderingRules);

var correctLines = updates.Where(u => u.IsCorrect(orderingRules));
var correctMiddleNumbers = correctLines.Select(u => u.GetMiddleNumber());

Console.WriteLine($"Part1: {correctMiddleNumbers.Sum()}");

var incorrectLines = updates.Where(u => !u.IsCorrect(orderingRules));
var incorrectSortedMiddleNumbers = incorrectLines.Select(u => {
    u.SortPages(orderingRules.Compare);
    return u.GetMiddleNumber();
});

Console.WriteLine($"Part2: {incorrectSortedMiddleNumbers.Sum()}");
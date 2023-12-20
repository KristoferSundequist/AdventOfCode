var input = File.ReadAllText("./data.txt");
var inputParts = input.Split($"{Environment.NewLine}{Environment.NewLine}");
var workflows = inputParts[0].Split(Environment.NewLine).Select(Workflow.FromString).ToDictionary(kvp => kvp.Name);
var workflowCollection = new WorkflowCollection(workflows);
var partRatings = inputParts[1].Split(Environment.NewLine).Select(PartRating.FromString).ToList();

var result1 = partRatings.Where(partRating => workflowCollection.IsApproved(partRating)).Sum(p => p.Sum());
Console.WriteLine($"Result 1: {result1}");

var startRange = new Range(1, 4000);
var result2 = workflowCollection.NumApprovedCombos("in", new Ranges(startRange, startRange, startRange, startRange), 0, false);
Console.WriteLine($"Result 2: {result2}");
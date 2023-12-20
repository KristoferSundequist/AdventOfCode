public class WorkflowCollection
{
    private Dictionary<string, Workflow> _workflows;

    public WorkflowCollection(Dictionary<string, Workflow> workflows)
    {
        _workflows = workflows;
    }

    public bool IsApproved(PartRating part)
    {
        var currentWorkflow = "in";
        while (true)
        {
            var workflow = _workflows[currentWorkflow];
            var destination = workflow.Next(part);
            if (destination == "A")
            {
                return true;
            }
            else if (destination == "R")
            {
                return false;
            }
            currentWorkflow = destination;
        }
    }

    private string GetIndent(int level) => String.Join("", Enumerable.Range(0, level).Select(_ => "  "));
    public long NumApprovedCombos(string currentWorkflow, Ranges ranges, int level, bool verbose)
    {
        if (verbose)
        {
            Console.WriteLine($"{GetIndent(level)} {currentWorkflow}: {ranges}");
        }
        var workflow = _workflows[currentWorkflow];
        var nextWorkflows = workflow.NextRanges(ranges);
        long numCombos = 0;
        foreach (var nextWorkflow in nextWorkflows)
        {
            if (nextWorkflow.newDestination == "A")
            {
                if (verbose)
                {
                    Console.WriteLine($"{GetIndent(level + 1)} {nextWorkflow.ranges}: adding {nextWorkflow.ranges.Product()}");
                }
                numCombos += nextWorkflow.ranges.Product();
            }
            else if (nextWorkflow.newDestination == "R")
            {
                // do nothing
            }
            else
            {
                numCombos += NumApprovedCombos(nextWorkflow.newDestination, nextWorkflow.ranges, level + 1, verbose);
            }
        }
        return numCombos;
    }
}
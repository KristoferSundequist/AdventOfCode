using Google.OrTools.Sat;

public class Machine
{
    public int[] IndicatorLights { get; set; }
    public int[][] Buttons { get; set; }
    public int[] JoltageRequirement { get; set; }

    // [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
    public Machine(string configuration)
    {
        int lightStart = configuration.IndexOf('[') + 1;
        int lightEnd = configuration.IndexOf(']');
        string lightsStr = configuration.Substring(lightStart, lightEnd - lightStart);
        IndicatorLights = lightsStr.Select(c => c == '#' ? 1 : 0).ToArray();

        var buttonParts = configuration.Split('(').Skip(1);
        Buttons = buttonParts.Select(p =>
        {
            string content = p.Split(')')[0];
            int[] button = new int[IndicatorLights.Length];
            var indices = content.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            foreach (int index in indices)
            {
                button[index] = 1;
            }
            return button;
        }).ToArray();

        int amountStart = configuration.IndexOf('{') + 1;
        int amountEnd = configuration.IndexOf('}');
        string amountsStr = configuration.Substring(amountStart, amountEnd - amountStart);
        JoltageRequirement = amountsStr.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
    }

    public int Part1()
    {
        var currentState = Enumerable.Range(0, IndicatorLights.Length).Select(_ => 0).ToArray();
        for (var numButtonClicks = 0; numButtonClicks < Buttons.Length; numButtonClicks++)
        {
            foreach (var state in GetNext1(currentState, 0, numButtonClicks))
            {
                if (Equals(state, IndicatorLights))
                {
                    return numButtonClicks + 1;
                }
            }
        }
        throw new Exception("Expected to find asnwer but didnt part1");
    }

    public long Part2()
    {
        var model = new CpModel();

        var buttonPresses = Buttons.Select((_, i) => model.NewIntVar(0, JoltageRequirement.Max() * 1000, $"btn_{i}")).ToArray();

        for (var ji = 0; ji < JoltageRequirement.Length; ji++)
        {
            var sum = LinearExpr.Sum(Buttons.Select((btn, bi) => buttonPresses[bi] * btn[ji]));
            model.Add(sum == JoltageRequirement[ji]);
        }

        model.Minimize(LinearExpr.Sum(buttonPresses));

        var solver = new CpSolver();
        var status = solver.Solve(model);

        if (status == CpSolverStatus.Optimal)
        {
            return buttonPresses.Sum(btn => solver.Value(btn));
        }
        else
        {
            throw new Exception($"No optimal solution found. Status: {status}");
        }
    }

    public IEnumerable<int[]> GetNext1(int[] currentState, int currentDepth, int maxDepth)
    {
        for (var i = 0; i < Buttons.Length; i++)
        {
            var nextState = Add(currentState, Buttons[i]);
            if (currentDepth < maxDepth)
            {
                foreach (var next in GetNext1(nextState, currentDepth + 1, maxDepth))
                {
                    yield return next;
                }
            }
            else
            {
                yield return nextState;
            }
        }
    }

    private int[] Add(int[] a, int[] b)
    {
        int[] c = new int[a.Length];
        for (var i = 0; i < a.Length; i++)
        {
            c[i] = a[i] + b[i];
        }
        return c;
    }

    private static bool Equals(int[] lights, int[] requirement)
    {
        for (var i = 0; i < lights.Length; i++)
        {
            if (lights[i] % 2 != requirement[i])
            {
                return false;
            }
        }
        return true;
    }
}

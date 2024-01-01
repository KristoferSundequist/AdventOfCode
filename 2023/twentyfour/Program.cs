using Google.OrTools.LinearSolver;
using Google.OrTools.Sat;

var lines = File.ReadAllLines("data.txt").Select(Line.FromString).ToArray();

var result1 = GetNumIntersectingLinesInArea(lines, 200000000000000, 400000000000000);
Console.WriteLine($"Result 1: {result1}");

var result2 = GetIntersectionLine(lines);
Console.WriteLine($"Result 2: {result2.x + result2.y + result2.z}");


int GetNumIntersectingLinesInArea(Line[] lines, long min, long max)
{
    var numWithin = 0;
    for (var i = 0; i < lines.Length; i++)
    {
        for (var j = i + 1; j < lines.Length; j++)
        {
            var line = lines[i];
            var line2 = lines[j];
            var result = line.Get2dIntersection(line2);
            if (result.success)
            {
                var intersectionX = line.x + line.dx * result.t1;
                var intersectionY = line.y + line.dy * result.t1;
                if (
                    min <= intersectionX && intersectionX <= max &&
                    min <= intersectionY && intersectionY <= max
                )
                {
                    numWithin++;
                }
            }
        }
    }
    return numWithin;
}

Line GetIntersectionLine(Line[] lines)
{
    var model = new CpModel();
    long bigNumber = 900000000000000;
    var ts = Enumerable.Range(0, lines.Length).Select(i => model.NewIntVar(0, bigNumber, $"t{i}")).ToArray();
    var x = model.NewIntVar(-bigNumber, bigNumber, $"x");
    var y = model.NewIntVar(-bigNumber, bigNumber, $"y");
    var z = model.NewIntVar(-bigNumber, bigNumber, $"z");
    var dx = model.NewIntVar(-bigNumber, bigNumber, $"dx");
    var dy = model.NewIntVar(-bigNumber, bigNumber, $"dy");
    var dz = model.NewIntVar(-bigNumber, bigNumber, $"dz");
    for (var i = 0; i < lines.Length; i++)
    {
        var xt = model.NewIntVar(-bigNumber, bigNumber, $"xt");
        model.AddMultiplicationEquality(xt, [dx, ts[i]]);
        model.Add(x + xt == lines[i].x + lines[i].dx * ts[i]);

        var yt = model.NewIntVar(-bigNumber, bigNumber, $"yt");
        model.AddMultiplicationEquality(yt, [dy, ts[i]]);
        model.Add(y + yt == lines[i].y + lines[i].dy * ts[i]);

        var zt = model.NewIntVar(-bigNumber, bigNumber, $"zt");
        model.AddMultiplicationEquality(zt, [dz, ts[i]]);
        model.Add(z + zt == lines[i].z + lines[i].dz * ts[i]);
    }
    var solver = new CpSolver();
    var resultStatus = solver.Solve(model);
    if (resultStatus == CpSolverStatus.Feasible || resultStatus == CpSolverStatus.Optimal)
    {
        return new Line(solver.Value(x), solver.Value(y), solver.Value(z), solver.Value(dx), solver.Value(dy), solver.Value(dz));
    }
    throw new Exception($"unkown solverstatus {resultStatus}");
}

public readonly record struct Line(long x, long y, long z, long dx, long dy, long dz)
{
    public static Line FromString(string str)
    {
        var parts = str.Split(" @ ");
        var starts = parts[0].Split(", ").Select(long.Parse).ToArray();
        var velocities = parts[1].Split(", ").Select(long.Parse).ToArray();
        return new Line(starts[0], starts[1], starts[2], velocities[0], velocities[1], velocities[2]);
    }

    public IntersectionResult Get2dIntersection(Line other)
    {
        var solver = Solver.CreateSolver("GLOP");
        var t1 = solver.MakeNumVar(0.0, double.PositiveInfinity, "t1");
        var t2 = solver.MakeNumVar(0.0, double.PositiveInfinity, "t2");
        solver.Add(x + dx * t1 == other.x + other.dx * t2);
        solver.Add(y + dy * t1 == other.y + other.dy * t2);
        var resultStatus = solver.Solve();
        if (resultStatus == Solver.ResultStatus.FEASIBLE || resultStatus == Solver.ResultStatus.OPTIMAL)
        {
            return new IntersectionResult(true, t1.SolutionValue(), t2.SolutionValue());
        }
        else if (resultStatus == Solver.ResultStatus.INFEASIBLE)
        {
            return new IntersectionResult(false, -1, -1);
        }
        throw new Exception($"unkown solverstatus {resultStatus}");
    }

}

public readonly record struct IntersectionResult(bool success, double t1, double t2);
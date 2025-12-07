using System.Runtime.CompilerServices;

public class ManifoldDiagram
{
    private readonly HashSet<Position> _splitters = new();
    private readonly Position _startBeam;
    private readonly int _manifoldHeight;

    public ManifoldDiagram(string[] lines)
    {
        for (var y = 0; y < lines.Length; y++)
        {
            for (var x = 0; x < lines[0].Length; x++)
            {
                if (lines[y][x] == 'S')
                {
                    if (_startBeam != null)
                    {
                        throw new ArgumentException("Only expected to find 'S' once");
                    }
                    _startBeam = new Position(y, x);
                }
                else if (lines[y][x] == '^')
                {
                    _splitters.Add(new Position(y, x));
                }
            }
        }

        if (_startBeam == null)
        {
            throw new ArgumentException("Did not find 'S'");
        }

        _manifoldHeight = lines.Length;
    }

    public (int numSplits, long numTimeLines) GetNumSplits()
    {
        var currentBeamCounts = new Dictionary<Position, long> { { _startBeam, 1 } };
        var totalNumSplits = 0;
        for (var y = 0; y < _manifoldHeight; y++)
        {
            var (newBeams, numSplits) = AdvanceBeams(currentBeamCounts);
            currentBeamCounts = newBeams;
            totalNumSplits += numSplits;
        }
        var numTimeLines = currentBeamCounts.Sum(kvp => kvp.Value);
        return (totalNumSplits, numTimeLines);
    }

    public (Dictionary<Position, long>, int) AdvanceBeams(Dictionary<Position, long> beamCounts)
    {
        var newBeamCounts = new Dictionary<Position, long>();
        var numSplits = 0;

        foreach (var (beam, count) in beamCounts)
        {
            var newBeamCandidate = beam.Down();
            if (_splitters.Contains(newBeamCandidate))
            {
                var (left, right) = newBeamCandidate.Split();

                newBeamCounts[left] = newBeamCounts.GetValueOrDefault(left) + count;
                newBeamCounts[right] = newBeamCounts.GetValueOrDefault(right) + count;

                numSplits++;
            }
            else
            {
                newBeamCounts[newBeamCandidate] = newBeamCounts.GetValueOrDefault(newBeamCandidate) + count;
            }
        }

        return (newBeamCounts, numSplits);
    }
}

public record Position(int Y, int X)
{
    public (Position, Position) Split()
    {
        return (this with { X = X - 1 }, this with { X = X + 1 });
    }

    public Position Down()
    {
        return this with { Y = Y + 1 };
    }
}
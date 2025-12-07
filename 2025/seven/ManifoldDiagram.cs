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

    public int GetNumSplits()
    {
        var currentBeams = new HashSet<Position> { _startBeam };
        var totalNumSplits = 0;
        for (var y = 0; y < _manifoldHeight; y++)
        {
            var (newBeams, numSplits) = AdvanceBeams(currentBeams);
            currentBeams = newBeams;
            totalNumSplits += numSplits;
        }
        return totalNumSplits;
    }

    public (HashSet<Position>, int) AdvanceBeams(HashSet<Position> beams)
    {
        var newBeams = new HashSet<Position>();
        var numSplits = 0;

        foreach (var beam in beams)
        {
            var newBeamCandidate = beam.Down();
            if (_splitters.Contains(newBeamCandidate))
            {
                var (left, right) = newBeamCandidate.Split();
                newBeams.Add(left);
                newBeams.Add(right);
                numSplits++;
            }
            else
            {
                newBeams.Add(newBeamCandidate);
            }
        }

        return (newBeams, numSplits);
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
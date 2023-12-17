using System.Collections.Frozen;
using System.Collections.Immutable;

var lines = File.ReadAllLines("./data.txt");
var cityBlocks = new Dictionary<Coordinate, int>();
for (var y = 0; y < lines.Length; y++)
{
    for (var x = 0; x < lines[0].Length; x++)
    {
        cityBlocks.Add(new Coordinate(y, x), int.Parse(lines[y][x].ToString()));
    }
}

var minHeatLosses = new Dictionary<State, int>();
var startState = new State(Direction.Right, new Coordinate(0, 0), 0);
var end = new Coordinate(lines.Length - 1, lines[0].Length - 1);
var states = GetMinHeatLosses(cityBlocks.ToFrozenDictionary(), startState, end, 3, 0);
var result1 = states.Where(kvp => kvp.Value.path.Last().coordinate == end).Select(kvp => kvp.Value.heatLoss).Min();
Console.WriteLine($"Result 1: {result1}");

var states2 = GetMinHeatLosses(cityBlocks.ToFrozenDictionary(), startState, end, 10, 4);
var result2 = states2.Where(kvp => kvp.Value.path.Last().coordinate == end).Select(kvp => kvp.Value.heatLoss).Min();
Console.WriteLine($"Result 2: {result2}");


Dictionary<State, PathResult> GetMinHeatLosses(FrozenDictionary<Coordinate, int> cityBlocks, State start, Coordinate end, int maxSteps, int minSteps)
{
    var states = new Dictionary<State, PathResult>();
    var statesToExpand = new Dictionary<State, PathResult> { { start, new PathResult(0, []) } };
    var iters = 0;
    while (statesToExpand.Count > 0)
    {
        iters++;
        var nextStatesToExpand = new Dictionary<State, PathResult>();
        foreach (var stateToExpand in statesToExpand)
        {
            var expandedStates = stateToExpand.Key.Next(cityBlocks, stateToExpand.Value.path, stateToExpand.Value.heatLoss, maxSteps, minSteps);
            foreach (var expandedStateKvp in expandedStates)
            {
                if (expandedStateKvp.Key.coordinate == end && expandedStateKvp.Key.numSameDirection < minSteps)
                {
                    continue;
                }
                if (states.TryGetValue(expandedStateKvp.Key, out var prevResult))
                {
                    if (expandedStateKvp.Value.heatLoss < prevResult.heatLoss)
                    {
                        states[expandedStateKvp.Key] = expandedStateKvp.Value;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    states[expandedStateKvp.Key] = expandedStateKvp.Value;
                }

                if (nextStatesToExpand.TryGetValue(expandedStateKvp.Key, out var prevResultThisGeneration))
                {
                    if (expandedStateKvp.Value.heatLoss < prevResultThisGeneration.heatLoss)
                    {
                        nextStatesToExpand[expandedStateKvp.Key] = expandedStateKvp.Value;
                    }
                }
                else
                {
                    nextStatesToExpand[expandedStateKvp.Key] = expandedStateKvp.Value;
                }
            }
        }
        statesToExpand = nextStatesToExpand;
    }
    return states;
}


record State(Direction direction, Coordinate coordinate, int numSameDirection)
{
    public Dictionary<State, PathResult> Next(FrozenDictionary<Coordinate, int> cityBlocks, ImmutableList<State> path, int currentCost, int maxSteps, int minSteps)
    {
        var newStates = new Dictionary<State, PathResult>();
        if (!(numSameDirection < minSteps && direction != Direction.Up) && !(direction == Direction.Up && numSameDirection >= maxSteps) && direction == Direction.Up || direction == Direction.Left || direction == Direction.Right)
        {
            var newCoordinate = coordinate with { y = coordinate.y - 1 };
            if (cityBlocks.TryGetValue(newCoordinate, out var cost))
            {
                var newNumSameDirection = direction == Direction.Up ? numSameDirection + 1 : 1;
                var newState = new State(Direction.Up, newCoordinate, newNumSameDirection);
                newStates.Add(newState, new PathResult(currentCost + cost, path.Add(newState)));
            }
        }
        if (!(numSameDirection < minSteps && direction != Direction.Left) && !(direction == Direction.Left && numSameDirection >= maxSteps) && (direction == Direction.Left || direction == Direction.Up || direction == Direction.Down))
        {
            var newCoordinate = coordinate with { x = coordinate.x - 1 };
            if (cityBlocks.TryGetValue(newCoordinate, out var cost))
            {
                var newNumSameDirection = direction == Direction.Left ? numSameDirection + 1 : 1;
                var newState = new State(Direction.Left, newCoordinate, newNumSameDirection);
                newStates.Add(newState, new PathResult(currentCost + cost, path.Add(newState)));
            }
        }
        if (!(numSameDirection < minSteps && direction != Direction.Down) && !(direction == Direction.Down && numSameDirection >= maxSteps) && (direction == Direction.Down || direction == Direction.Left || direction == Direction.Right))
        {
            var newCoordinate = coordinate with { y = coordinate.y + 1 };
            if (cityBlocks.TryGetValue(newCoordinate, out var cost))
            {
                var newNumSameDirection = direction == Direction.Down ? numSameDirection + 1 : 1;
                var newState = new State(Direction.Down, newCoordinate, newNumSameDirection);
                newStates.Add(newState, new PathResult(currentCost + cost, path.Add(newState)));
            }
        }
        if (!(numSameDirection < minSteps && direction != Direction.Right) && !(direction == Direction.Right && numSameDirection >= maxSteps) && (direction == Direction.Right || direction == Direction.Down || direction == Direction.Up))
        {
            var newCoordinate = coordinate with { x = coordinate.x + 1 };
            if (cityBlocks.TryGetValue(newCoordinate, out var cost))
            {
                var newNumSameDirection = direction == Direction.Right ? numSameDirection + 1 : 1;
                var newState = new State(Direction.Right, newCoordinate, newNumSameDirection);
                newStates.Add(newState, new PathResult(currentCost + cost, path.Add(newState)));
            }
        }
        return newStates;
    }
}
record Coordinate(int y, int x);
enum Direction
{
    Up,
    Left,
    Down,
    Right
}
record PathResult(int heatLoss, ImmutableList<State> path);

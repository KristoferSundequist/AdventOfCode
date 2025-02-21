using System.Collections.Immutable;

public class Network
{
    private Dictionary<string, HashSet<string>> _connections = new();

    public Network(string[] connections)
    {
        foreach (var connection in connections)
        {
            var parts = connection.Split('-');

            if (!_connections.ContainsKey(parts[0]))
            {
                _connections[parts[0]] = [parts[1]];
            }
            else
            {
                _connections[parts[0]].Add(parts[1]);
            }

            if (!_connections.ContainsKey(parts[1]))
            {
                _connections[parts[1]] = [parts[0]];
            }
            else
            {
                _connections[parts[1]].Add(parts[0]);
            }
        }
    }

    public void Visualize()
    {
        foreach (var connection in _connections)
        {
            Console.WriteLine($"{connection.Key} -> {string.Join(", ", connection.Value)}");
        }
    }

    public int FindThreeCliques(string prefix)
    {
        HashSet<string> cliques = [];

        foreach (var connection in _connections.Where(c => c.Key.StartsWith(prefix)))
        {
            foreach (var friend in connection.Value)
            {
                var mutualFriends = connection.Value.Intersect(_connections[friend]).ToList();
                foreach (var mutualFriend in mutualFriends)
                {
                    var cliqueKey = GetKey([connection.Key, friend, mutualFriend]);
                    cliques.Add(cliqueKey);
                }
            }
        }

        return cliques.Count;
    }

    public static string GetKey(IEnumerable<string> set) => string.Join(",", set.Order());

    public void GetBiggestClique(HashSet<string> found, ImmutableHashSet<string> set)
    {
        if (found.Contains(GetKey(set)))
        {
            return;
        }
        found.Add(GetKey(set));
        var expansionNodes = _connections.Keys.Where(k => !set.Contains(k)).Where(k => set.IsSubsetOf(_connections[k])).ToList();
        var newCliques = expansionNodes.Select(n => set.Add(n));

        foreach (var newClique in newCliques)
        {
            GetBiggestClique(found, newClique);
        }
    }
}
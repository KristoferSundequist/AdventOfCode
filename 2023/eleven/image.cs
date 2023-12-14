public class Image
{
    private List<Coordinate> _stars = new();
    private int[] _emptyRowsByIndex;
    private int[] _emptyColsByIndex;

    public Image(string[] lines)
    {
        _emptyRowsByIndex = new int[lines.Length];
        _emptyRowsByIndex[0] = 0;
        for (var y = 1; y < lines.Length; y++)
        {
            if (lines[y].All(c => c == '.'))
            {
                _emptyRowsByIndex[y] = _emptyRowsByIndex[y - 1] + 1;
            }
            else
            {
                _emptyRowsByIndex[y] = _emptyRowsByIndex[y - 1];
            }
        }

        _emptyColsByIndex = new int[lines.Length];
        _emptyColsByIndex[0] = 0;
        for (var x = 1; x < lines[0].Length; x++)
        {
            if (lines.Select(line => line[x]).All(c => c == '.'))
            {
                _emptyColsByIndex[x] = _emptyColsByIndex[x - 1] + 1;
            }
            else
            {
                _emptyColsByIndex[x] = _emptyColsByIndex[x - 1];
            }
        }

        for (var y = 0; y < lines.Length; y++)
        {
            for (var x = 0; x < lines[0].Length; x++)
            {
                if (lines[y][x] == '#')
                {
                    _stars.Add(new Coordinate(y, x));
                }
            }
        }
    }

    public long GetPairDistanceSum(long emptyExpansion)
    {
        long totalDistance = 0;
        for (var i = 0; i < _stars.Count; i++)
        {
            for (var j = i + 1; j < _stars.Count; j++)
            {
                var expanded_star_i_x = _stars[i].x + _emptyColsByIndex[_stars[i].x] * emptyExpansion;
                var expanded_star_j_x = _stars[j].x + _emptyColsByIndex[_stars[j].x] * emptyExpansion;
                var expanded_star_i_y = _stars[i].y + _emptyRowsByIndex[_stars[i].y] * emptyExpansion;
                var expanded_star_j_y = _stars[j].y + _emptyRowsByIndex[_stars[j].y] * emptyExpansion;
                totalDistance += Math.Abs(expanded_star_i_x - expanded_star_j_x) + Math.Abs(expanded_star_i_y - expanded_star_j_y);
            }
        }
        return totalDistance;
    }

    private record Coordinate(int y, int x);
}
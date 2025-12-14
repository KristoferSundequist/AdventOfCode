public record struct Rectangle
{
    private Vec2 _first;
    private Vec2 _second;

    private List<Line> _lines;

    public Rectangle(Vec2 first, Vec2 second)
    {
        _first = first;
        _second = second;

        var topLeft = new Vec2(Math.Min(_first.X, _second.X), Math.Min(_first.Y, _second.Y));
        var bottomRight = new Vec2(Math.Max(_first.X, _second.X), Math.Max(_first.Y, _second.Y));
        var topRight = new Vec2(bottomRight.X, topLeft.Y);
        var bottomLeft = new Vec2(topLeft.X, bottomRight.Y);

        _lines = [
            new Line(topLeft, topRight),
            new Line(topRight, bottomRight),
            new Line(bottomRight, bottomLeft),
            new Line(bottomLeft, topLeft)
        ];
    }

    public long Area() => (Math.Abs(_first.X - _second.X) + 1) * (Math.Abs(_first.Y - _second.Y) + 1);

    public bool CrossesLine(Line line)
    {
        foreach (var rectLine in _lines)
        {
            if (rectLine.Crosses(line))
            {
                return true;
            }
        }
        return false;
    }

    public override string ToString()
    {
        return $"Rectangle({_first}, {_second})";
    }
}
public record Vec2(int X, int Y)
{
    public Vec2 Add(Vec2 other) => new Vec2(X + other.X, Y + other.Y);
    public Vec2 Subtract(Vec2 other) => new Vec2(X - other.X, Y - other.Y);
    public Vec2 Multiply(int scalar) => new Vec2(X * scalar, Y * scalar);
}
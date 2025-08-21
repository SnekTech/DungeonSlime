using Microsoft.Xna.Framework;

namespace MonoGameLibrary;

public readonly record struct Circle(int X, int Y, int Radius)
{
    public Circle(Point location, int radius) : this(location.X, location.Y, radius)
    {
    }

    public static Circle Empty { get; } = new();

    public Point Location => new(X, Y);

    public bool IsEmpty => X == 0 && Y == 0 && Radius == 0;

    public int Top => Y - Radius;
    public int Bottom => Y + Radius;
    public int Left => X - Radius;
    public int Right => X + Radius;

    public bool Intersects(Circle other)
    {
        var radiiSquared = (Radius + other.Radius) * (Radius + other.Radius);
        var distanceSquared = Vector2.DistanceSquared(Location.ToVector2(), other.Location.ToVector2());
        return distanceSquared < radiiSquared;
    }
}
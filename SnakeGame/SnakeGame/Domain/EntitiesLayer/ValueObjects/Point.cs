using System;

struct Point
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool Overlaps(Point point)
    {
        return point.X == X && point.Y == Y;
    }

    public bool IsIn(Frame frame)
    {
        return X < frame.MaxX && X > frame.MinX &&
               Y < frame.MaxY && Y > frame.MinY;
    }

    public Point Moved(Direction direction)
    {
        switch (direction)
        {
            case Direction.Top: return new Point(X, Y + 1);
            case Direction.Bottom: return new Point(X, Y - 1);
            case Direction.Right: return new Point(X + 1, Y);
            case Direction.Left: return new Point(X - 1, Y);
            default: throw new ArgumentException("Unknown direction");
        }
    }

    public static Point RandomIn(Frame frame)
    {
        var random = new Random();
        return new Point(random.Next(frame.MinX + 1, frame.MaxX - 1), random.Next(frame.MinY + 1, frame.MaxY - 1));
    }
}

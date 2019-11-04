﻿
//Infrastructure ---------------------------------------------------------------------------
// 1) Db -----------------------------
class PointDbDto
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point To()
    {
        return new Point(X, Y);
    }

    public static PointDbDto From(Point point)
    {
        return new PointDbDto
        {
            X = point.X,
            Y = point.Y
        };
    }
}

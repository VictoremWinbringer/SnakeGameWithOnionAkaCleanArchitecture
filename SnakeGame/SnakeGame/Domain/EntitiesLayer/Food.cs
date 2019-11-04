using System;

class Food
{
    public Guid Id { get; set; }
    public Point Body { get; private set; }

    public void MoveRandomIn(Frame frame)
    {
        Body = Point.RandomIn(frame);
    }

    public Food(Guid id, Point body)
    {
        Body = body;
        Id = id;
    }

    public Food(Frame frame)
    {
        Id = Guid.NewGuid();
        Body = Point.RandomIn(frame);
    }
}

abstract class BaseId
{
    protected BaseId(Guid value)
    {
        if (value == default)
            throw new ArgumentOutOfRangeException(nameof(value));
        Value = value;
    }

    public Guid Value { get; }
}
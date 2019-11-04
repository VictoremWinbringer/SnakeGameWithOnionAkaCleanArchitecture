using System;
using System.Collections.Generic;

class SnakeBody
{
    private readonly LinkedList<Point> _value;
    public IReadOnlyCollection<Point> Value => _value;
    public SnakeBody(IEnumerable<Point> value)
    {
        _value = new LinkedList<Point>(value);
        if (_value.Count < 3)
            throw new ArgumentOutOfRangeException(nameof(_value.Count));
    }

    public Point Last => _value.Last.Value;

    public Point Head => _value.First.Value;

    public void AddLast(Point point) => _value.AddLast(point);
    public void RemoveLast() => _value.RemoveLast();
    public void SetHead(Point point) => _value.AddFirst(point);
    public int Count => _value.Count;
    public bool BitesSelf => _value.Count(p => p.Overlaps(Head)) > 1;
}

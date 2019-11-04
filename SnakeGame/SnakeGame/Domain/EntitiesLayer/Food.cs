using System;

class Food
{
    public FoodId Id { get; set; }
    public Point Body { get; private set; }

    public void MoveRandomIn(Frame frame)
    {
        Body = Point.RandomIn(frame);
    }

    public Food(FoodId id, Point body)
    {
        Body = body;
        Id = id ?? throw new ArgumentNullException(nameof(id));
    }

    public Food(Frame frame)
    {
        Id = new FoodId(Guid.NewGuid());
        Body = Point.RandomIn(frame);
    }
}

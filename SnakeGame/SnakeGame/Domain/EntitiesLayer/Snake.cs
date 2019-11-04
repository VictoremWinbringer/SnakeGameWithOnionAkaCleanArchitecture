using System;

class Snake
{
    public SnakeId Id { get; }
    public SnakeBody Body { get; }

    public Direction Direction { get; private set; }

    public Snake(SnakeId id, SnakeBody body, Direction direction)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Body = body ?? throw new ArgumentNullException(nameof(body));
        Direction = direction;
    }

    public Snake(SnakeBody body) : this(new SnakeId(Guid.NewGuid()), body, Direction.Right)
    {
    }

    public void Turn(Direction direction) => Direction = direction;

    public void Eat(Food food)
    {
        if (!CanEat(food))
        {
            var ex = new DomainException(DomainExceptionCode.SnakeFromFoodToFar);
            ex.Data[nameof(food.Body)] = food.Body;
            ex.Data[nameof(Body.Last)] = Body.Last;
            throw ex;
        }

        Body.AddLast(food.Body);
    }

    public bool CanEat(Food food) => food.Body.Overlaps(Body.Head);

    public void Move()
    {
        var head = Body.Head;
        Body.RemoveLast();
        var point = head.Moved(Direction);
        Body.SetHead(point);
    }

    public bool IsBitingTail => Body.BitesSelf;

    public bool IsHeadIn(Frame frame) => Body.Head.IsIn(frame);
}

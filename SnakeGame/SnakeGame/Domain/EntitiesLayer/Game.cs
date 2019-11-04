using System;

class Game
{
    public GameId Id { get; }
    public Snake Snake { get; }
    public Frame Frame { get; }
    public Food Food { get; }

    public bool GameOver { get; private set; }

    public Game(GameId id, Snake snake, Frame frame, Food food)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Snake = snake ?? throw new ArgumentNullException(nameof(snake));
        Frame = frame ?? throw new ArgumentNullException(nameof(frame));
        Food = food ?? throw new ArgumentNullException(nameof(food));
        if (Math.Abs(Frame.MaxX - Frame.MinX) * Math.Abs(Frame.MaxY - Frame.MinY) < Snake.Body.Count * Snake.Body.Count)
        {
            var ex = new DomainException(DomainExceptionCode.GameFrameToSmallForSnake);
            ex.Data[nameof(Frame)] = frame;
            ex.Data[nameof(Snake.Body.Count)] = Snake.Body.Count;
            throw ex;
        }
    }

    public Game(Snake snake, Frame frame, Food food) : this(new GameId(Guid.NewGuid()), snake, frame, food)
    {
    }

    public void Input(Direction direction)
    {
        Snake.Turn(direction);
    }

    public void Logic()
    {
        if (GameOver)
            return;

        if (!Snake.IsHeadIn(Frame) ||
            Snake.IsBitingTail)
        {
            GameOver = true;
            return;
        }

        if (Snake.CanEat(Food))
        {
            Snake.Eat(Food);
            Food.MoveRandomIn(Frame);
        }

        Snake.Move();
    }
}

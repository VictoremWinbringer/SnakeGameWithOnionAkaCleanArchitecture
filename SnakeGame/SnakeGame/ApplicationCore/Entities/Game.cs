using System;
using SnakeGame.ApplicationCore.Entities.ValueObjects;

namespace SnakeGame.ApplicationCore.Entities
{
    class Game
    {
        private readonly Func<Point> _createNewFoodPosition;
        public Guid Id { get; }
        public Snake Snake { get; }
        public Frame Frame { get; }
        public Food Food { get; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        public Game(Guid id, Snake snake, Frame frame, Food food, int score)
        {
            Id = id;
            Snake = snake ?? throw new ApplicationException("Snake is null!");
            Frame = frame ?? throw new ApplicationException("Frame is null!");
            Food = food ?? throw new ApplicationException("Food is null!");
            Score = score;
            if (Math.Abs(Frame.MaxX - Frame.MinX) * Math.Abs(Frame.MaxY - Frame.MinY) < Snake.Body.Count * Snake.Body.Count)
            {
                var ex = new ApplicationException("Game frame to small for snake");
                ex.Data[nameof(Frame)] = frame;
                ex.Data[nameof(Snake.Body.Count)] = Snake.Body.Count;
                throw ex;
            }
        }

        public Game(Snake snake, Frame frame, Food food) : this(Guid.NewGuid(), snake, frame, food, 0)
        {
        }

        public void Input(Direction direction)
        {
            Snake.Turn(direction);
        }

        public void Logic(Func<Point> createNewFoodPosition)
        {
            if (GameOver)
                return;
            if (createNewFoodPosition == null)
                throw new ApplicationException($"{nameof(createNewFoodPosition)} is Null!");
            if (!Snake.IsHeadIn(Frame) ||
                Snake.IsBitingTail)
            {
                GameOver = true;
                return;
            }
            if (Snake.CanEat(Food))
            {
                Snake.Eat(Food);
                Score++;
                Food.MoveTo(_createNewFoodPosition());
            }
            Snake.Move();
        }
    }
}

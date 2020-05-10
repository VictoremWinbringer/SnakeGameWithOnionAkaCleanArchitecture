using System;
using SnakeGame.ApplicationCore.Entities.ValueObjects;

namespace SnakeGame.ApplicationCore.Entities
{
    sealed class Food
    {
        private readonly Random _random;
        public Guid Id { get; }
        public Point Body { get; private set; }

        public void MoveRandomIn(Frame frame)
        {
            Body = new Point(
                _random.Next(frame.MinX+1, frame.MaxX),
                _random.Next(frame.MinY+1, frame.MaxY)
                );
        }

        public Food(Guid id, Point body, Random random)
        {
            if (id == default)
                throw new ApplicationException("Id is default!");
            _random = random;
            Body = body;
            Id = id;
        }

        public Food(Point body) : this(Guid.NewGuid(), body, new Random())
        {
        }
    }
}

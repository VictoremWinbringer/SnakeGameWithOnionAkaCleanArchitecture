using System;
using SnakeGame.Entities.ValueObjects;

namespace SnakeGame.Entities
{
    sealed class Food
    {
        public Guid Id { get; }
        public Point Body { get; private set; }

        public void MoveRandomIn(Frame frame)
        {
            Body = Point.RandomIn(frame);
        }

        public Food(Guid id, Point body)
        {
            if (id == default)
                throw new ApplicationException("Id is default!");
            Body = body;
            Id = id;
        }

        public Food(Frame frame) : this(Guid.NewGuid(), Point.RandomIn(frame))
        {
        }
    }
}

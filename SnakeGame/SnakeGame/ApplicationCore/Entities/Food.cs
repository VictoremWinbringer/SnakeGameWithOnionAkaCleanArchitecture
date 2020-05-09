using System;
using SnakeGame.ApplicationCore.Entities.ValueObjects;

namespace SnakeGame.ApplicationCore.Entities
{
    sealed class Food
    {
        public Guid Id { get; }
        public Point Body { get; private set; }

        public void MoveTo(Point point)
        {
            Body = point;
        }

        public Food(Guid id, Point body)
        {
            if (id == default)
                throw new ApplicationException("Id is default!");
            Body = body;
            Id = id;
        }

        public Food(Point body) : this(Guid.NewGuid(), body)
        {
        }
    }
}

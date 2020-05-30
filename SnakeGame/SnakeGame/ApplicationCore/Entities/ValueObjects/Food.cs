using System;

namespace SnakeGame.ApplicationCore.Entities.ValueObjects
{
    sealed class Food
    {
        private readonly Random _random;
        public Point Body { get; private set; }

        public void MoveRandomIn(Frame frame)
        {
            Body = new Point(
                _random.Next(frame.MinX + 1, frame.MaxX),
                _random.Next(frame.MinY + 1, frame.MaxY)
                );
        }

        public Food(Point body, Random random)
        {
            if(random == null)
                throw new ApplicationException("random is null!");
            _random = random;
            Body = body;
        }

        public Food(Point body) : this(body, new Random())
        {
        }
    }
}

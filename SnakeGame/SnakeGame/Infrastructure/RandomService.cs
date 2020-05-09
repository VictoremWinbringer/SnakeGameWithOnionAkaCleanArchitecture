using System;
using System.Collections.Generic;
using System.Text;
using SnakeGame.ApplicationCore.Entities.ValueObjects;

namespace SnakeGame.Infrastructure
{
    class RandomService : IRandomService
    {
        private readonly Random _random;

        public RandomService(Random random)
        {
            _random = random ?? throw new ApplicationException("Random is Null!");
        }
        public Point Next(Frame inFrame)
        {
            var x = _random.Next(inFrame.MinX, inFrame.MaxX);
            var y = _random.Next(inFrame.MinY, inFrame.MaxY);
            return new Point(x, y);
        }
    }
}

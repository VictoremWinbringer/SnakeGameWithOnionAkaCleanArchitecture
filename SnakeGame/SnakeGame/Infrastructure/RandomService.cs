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

        public int Next(int min, int max) => _random.Next(min, max);
    }
}

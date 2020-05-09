using SnakeGame.ApplicationCore.Entities.ValueObjects;

namespace SnakeGame.Infrastructure
{
    interface IRandomService
    {
        int Next(int min, int max);
    }
}
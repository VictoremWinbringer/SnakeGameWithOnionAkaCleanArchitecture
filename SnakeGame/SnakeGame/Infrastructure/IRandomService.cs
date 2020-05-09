using SnakeGame.ApplicationCore.Entities.ValueObjects;

namespace SnakeGame.Infrastructure
{
    interface IRandomService
    {
        Point Next(Frame inFrame);
    }
}
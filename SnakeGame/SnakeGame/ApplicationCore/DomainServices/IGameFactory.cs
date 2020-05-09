using System;
using SnakeGame.ApplicationCore.Entities;
using SnakeGame.ApplicationCore.Entities.ValueObjects;

namespace SnakeGame.ApplicationCore.DomainServices
{
    interface IGameFactory
    {
        Game Create(Func<Frame, Point> createFoodBody);
    }
}

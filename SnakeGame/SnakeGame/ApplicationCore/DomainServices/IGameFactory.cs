using SnakeGame.ApplicationCore.Entities;

namespace SnakeGame.ApplicationCore.DomainServices
{
    interface IGameFactory
    {
        Game Create();
    }
}

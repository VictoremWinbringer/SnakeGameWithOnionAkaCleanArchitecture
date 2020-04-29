using SnakeGame.Entities;

namespace SnakeGame.DomainServices
{
    interface IGameFactory
    {
        Game Create();
    }
}

using System.Collections.Generic;
using SnakeGame.Entities;

// UseCase ----------------------------------------------------------------------------------------------

namespace SnakeGame.Infrastructure
{
    interface IGameRepository
    {
        List<Game> All();
        void Add(Game game);
    }
}

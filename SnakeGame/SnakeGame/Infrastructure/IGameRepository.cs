using System.Collections.Generic;
using SnakeGame.ApplicationCore.Entities;

// UseCase ----------------------------------------------------------------------------------------------

namespace SnakeGame.Infrastructure
{
    interface IGameRepository
    {
        List<Game> All();
        void Add(Game game);
    }
}

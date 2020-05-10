using System.Collections.Generic;
using SnakeGame.ApplicationCore.Entities;

// UseCase ----------------------------------------------------------------------------------------------

namespace SnakeGame.ApplicationCore.ApplicationServices.Infrastructure.Interfaces
{
    interface IGameRepository
    {
        List<Game> All();
        void Add(Game game);
    }
}

using System.Collections.Generic;
// UseCase ----------------------------------------------------------------------------------------------

interface IGameRepository
{
    List<Game> All();
    void Add(Game game);
}

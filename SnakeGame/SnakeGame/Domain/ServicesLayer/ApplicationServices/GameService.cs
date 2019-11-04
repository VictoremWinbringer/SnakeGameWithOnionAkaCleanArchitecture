using System;
using System.Linq;

class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameFactory _factory;
    private Game _game;

    public GameService(IGameRepository gameRepository, IGameFactory factory)
    {
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _game = _factory.Create();
    }

    public int CurrentScore => _game.Score;

    public int MaxScore
    {
        get
        {
            var all = _gameRepository.All();
            return all.Count > 0 ? all.Max(g => g.Score) : 0;
        }
    }

    public Game Get => _game;

    public void Input(Direction direction)
    {
        _game.Input(direction);
    }

    public void Logic()
    {
        _game.Logic();
        if (_game.GameOver)
        {
            _gameRepository.Add(_game);
            _game = _factory.Create();
        }
    }
}

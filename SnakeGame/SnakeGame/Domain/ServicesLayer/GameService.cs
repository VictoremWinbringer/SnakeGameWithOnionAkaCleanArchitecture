using System;
using System.Collections.Generic;
using System.Linq;

interface IGameFactory
{
    Game Create();
}

class GameFactory
{
    private readonly int snakeLength;
    private readonly int height;

    public GameFactory(int snakeLength, int height)
    {
        this.snakeLength = snakeLength;
        this.height = height;
    }
    public Game Create()
    {
        var frame = new Frame(0, 0, height, height);
        var food = new Food(frame);
        var list = new LinkedList<Point>();
        for (int i = 1; i < height; i++)
        {
            for (int j = 1; j < height; j++)
            {
                if (i == j && list.Count < snakeLength)
                    list.AddLast(new Point(i, j));
            }
        }
        var snake = new Snake(new SnakeBody(list));
        return new Game(snake, frame, food);
    }
}

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

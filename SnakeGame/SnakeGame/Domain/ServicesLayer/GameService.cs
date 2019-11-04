using System.Collections.Generic;
using System.Linq;

class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly int _snakeLength;
    private readonly int _height;
    private Game _game;
    private int _maxScore;

    public GameService(IGameRepository gameRepository, int snakeLength, int height)
    {
        _gameRepository = gameRepository;
        _snakeLength = snakeLength;
        _height = height;
        _game = CreateGame(snakeLength, height);
    }

    private Game CreateGame(int snakeLength, int height)
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
        _maxScore = InnerMaxScore();
        return new Game(snake, frame, food);
    }

    public int GetCurrentScore()
    {
        return _game.Snake.Body.Count - _snakeLength;
    }

    public int MaxScore()
    {
        return _maxScore;
    }

    private int InnerMaxScore()
    {
        var all = _gameRepository.All();
        return all.Count > 0 ? all.Max(g => g.Snake.Body.Count) - _snakeLength : 0;
    }

    public Game Draw()
    {
        return _game;
    }

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
            _maxScore = InnerMaxScore();
            _game = CreateGame(_snakeLength, _height);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using LiteDB;


enum DomainExceptionCode
{
}

[Serializable]
sealed class DomainException : Exception
{
    public DomainException(DomainExceptionCode code) : base() { Code = code; }
    public DomainException(DomainExceptionCode code, string message) : base(message) { Code = code; }
    public DomainException(DomainExceptionCode code, string message, Exception inner) : base(message, inner) { Code = code; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Code), Code);
        base.GetObjectData(info, context);
    }

    public DomainExceptionCode Code { get; }
}

class SnakeBodyNullException : Exception
{
}

class SnakeIdEmptyException : Exception
{
}

class SnakeBodyEptyException : Exception
{
    public SnakeBodyEptyException()
    {
    }
}

class SnakeFromFoodToFarException : Exception
{
    public Point Head { get; }
    public Point Food { get; }

    public SnakeFromFoodToFarException(Point head, Point food)
    {
        Head = head;
        Food = food;
    }
}

class Snake
{
    public Guid Id { get; }
    public LinkedList<Point> Body { get; }

    public Direction Direction { get; private set; }

    public Snake(Guid id, LinkedList<Point> body, Direction direction)
    {
        if (body == null)
            throw new SnakeBodyNullException();

        if (body.Count < 3)
            throw new SnakeBodyEptyException();

        if (id == Guid.Empty)
            throw new SnakeIdEmptyException();

        Id = id;
        Body = body;
        Direction = direction;
    }

    public Snake(LinkedList<Point> body) : this(Guid.NewGuid(), body, Direction.Right)
    {
    }

    public void Turn(Direction direction)
    {
        Direction = direction;
    }

    public void Eat(Food food)
    {
        if (!CanEat(food))
            throw new SnakeFromFoodToFarException(Body.Last.Value, food.Body);

        Body.AddLast(food.Body);
    }

    public bool CanEat(Food food)
    {
        return food.Body.Overlaps(Body.First.Value);
    }

    public void Move()
    {
        var head = Body.First.Value;
        Body.RemoveLast();
        var point = head.Moved(Direction);
        Body.AddFirst(point);
    }

    public bool IsBitingTail()
    {
        return Body.Count(p => p.Overlaps(Body.First.Value)) > 1;
    }

    public bool IsHeadIn(Frame frame)
    {
        return Body.First.Value.IsIn(frame);
    }
}

class GameIdEmptyException : Exception
{
    public GameIdEmptyException()
    {
    }
}

class GameSnakeNullExpection : Exception
{
    public GameSnakeNullExpection()
    {
    }
}

class GameFrameNullException : Exception
{
    public GameFrameNullException()
    {
    }
}

class GameFoodIsNullException : Exception
{
    public GameFoodIsNullException()
    {
    }
}

class GameFrameToSmallForSnakeException : Exception
{
    public Snake Snake { get; }
    public Frame Frame { get; }

    public GameFrameToSmallForSnakeException(Snake snake, Frame frame)
    {
        Snake = snake;
        Frame = frame;
    }
}

class Game
{
    public Guid Id { get; }
    public Snake Snake { get; }
    public Frame Frame { get; }
    public Food Food { get; }

    public bool GameOver { get; private set; }

    public Game(Guid id, Snake snake, Frame frame, Food food)
    {
        if (id == Guid.Empty)
            throw new GameIdEmptyException();

        Id = id;
        Snake = snake ?? throw new GameSnakeNullExpection();
        Frame = frame ?? throw new GameFrameNullException();
        Food = food ?? throw new GameFoodIsNullException();

        if (Math.Abs(Frame.MaxX - Frame.MinX) * Math.Abs(Frame.MaxY - Frame.MinY) < Snake.Body.Count * Snake.Body.Count)
            throw new GameFrameToSmallForSnakeException(snake, frame);
    }

    public Game(Snake snake, Frame frame, Food food) : this(Guid.NewGuid(), snake, frame, food)
    {
    }

    public void Input(Direction direction)
    {
        Snake.Turn(direction);
    }

    public void Logic()
    {
        if (GameOver)
            return;

        if (!Snake.IsHeadIn(Frame) ||
            Snake.IsBitingTail())
        {
            GameOver = true;
            return;
        }

        if (Snake.CanEat(Food))
        {
            Snake.Eat(Food);
            Food.MoveRandomIn(Frame);
        }

        Snake.Move();
    }
}

// UseCase ----------------------------------------------------------------------------------------------

interface IGameRepository
{
    List<Game> All();
    void Add(Game game);
}

interface IGameService
{
    int GetCurrentScore();
    int MaxScore();
    Game Draw();
    void Input(Direction direction);
    void Logic();
}

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
        var snake = new Snake(list);
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

//Infrastructure ---------------------------------------------------------------------------
// 1) Db -----------------------------
class PointDbDto
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point To()
    {
        return new Point(X, Y);
    }

    public static PointDbDto From(Point point)
    {
        return new PointDbDto
        {
            X = point.X,
            Y = point.Y
        };
    }
}

class FrameDbDto
{
    public PointDbDto Min { get; set; }
    public PointDbDto Max { get; set; }

    public Frame To()
    {
        return new Frame(Min.X, Min.Y, Max.X, Max.Y);
    }

    public static FrameDbDto From(Frame frame)
    {
        return new FrameDbDto
        {
            Max = new PointDbDto { X = frame.MaxX, Y = frame.MaxY },
            Min = new PointDbDto { X = frame.MinX, Y = frame.MinY }
        };
    }
}

class FoodBdDto
{
    public Guid Id { get; set; }
    public PointDbDto Body { get; set; }

    public Food To()
    {
        return new Food(Id, Body.To());
    }

    public static FoodBdDto From(Food food)
    {
        return new FoodBdDto
        {
            Id = food.Id.Value,
            Body = PointDbDto.From(food.Body)
        };
    }
}

class SnakeDbDto
{
    public Guid Id { get; set; }
    public List<PointDbDto> Body { get; set; }
    public Direction Direction { get; set; }

    public Snake To()
    {
        return new Snake(Id, new LinkedList<Point>(Body.Select(p => p.To())), Direction);
    }

    public static SnakeDbDto From(Snake snake)
    {
        return new SnakeDbDto
        {
            Id = snake.Id,
            Body = snake.Body.Select(PointDbDto.From).ToList(),
            Direction = snake.Direction
        };
    }
}

class GameDbDto
{
    public Guid Id { get; set; }
    public SnakeDbDto Snake { get; set; }
    public FrameDbDto Frame { get; set; }
    public FoodBdDto Food { get; set; }

    public Game To()
    {
        return new Game(Id, Snake.To(), Frame.To(), Food.To());
    }

    public static GameDbDto From(Game game)
    {
        return new GameDbDto
        {
            Id = game.Id,
            Snake = SnakeDbDto.From(game.Snake),
            Food = FoodBdDto.From(game.Food),
            Frame = FrameDbDto.From(game.Frame)
        };
    }
}

class GameRepository : IGameRepository, IDisposable
{
    private readonly LiteDatabase _db;

    public GameRepository()
    {
        _db = new LiteDatabase("Filename=./game.db;Mode=Exclusive");
    }

    public List<Game> All()
    {
        return _db.GetCollection<GameDbDto>().FindAll()
            .Select(g => g.To()).ToList();
    }

    public void Add(Game game)
    {
        _db.GetCollection<GameDbDto>().Insert(GameDbDto.From(game));
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}


// UI-----------------------------------------
public class PointModel
{
    public int X { get; set; }
    public int Y { get; set; }
    public char Sym { get; set; }
}

class ConsoleGameController
{
    private readonly IGameService _service;

    public ConsoleGameController(IGameService service)
    {
        _service = service;
    }

    public void Input(ConsoleKey key)
    {
        var direction = Parse(key);
        if (direction.HasValue)
            _service.Input(direction.Value);
    }

    public int Score()
    {
        return _service.GetCurrentScore();
    }

    public int MaxScore()
    {
        return _service.MaxScore();
    }

    public List<PointModel> Draw()
    {
        var game = _service.Draw();
        List<PointModel> points = new List<PointModel>();
        var frameSym = 's';
        var snakeSym = 'c';
        var foodSym = '#';

        for (int i = game.Frame.MinX; i <= game.Frame.MaxX; i++)
        {
            for (int j = game.Frame.MinY; j <= game.Frame.MaxY; j++)
            {
                if (i == game.Frame.MinX ||
                    i == game.Frame.MaxX ||
                    j == game.Frame.MinY ||
                    j == game.Frame.MaxY)
                    points.Add(new PointModel { X = i, Y = j, Sym = frameSym });
            }
        }

        foreach (var point in game.Snake.Body)
        {
            points.Add(new PointModel { X = point.X, Y = point.Y, Sym = snakeSym });
        }

        points.Add(new PointModel { X = game.Food.Body.X, Y = game.Food.Body.Y, Sym = foodSym });

        return points;
    }

    public void Logic()
    {
        _service.Logic();
    }

    private Direction? Parse(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
                return Direction.Bottom;
            case ConsoleKey.DownArrow:
                return Direction.Top;
            case ConsoleKey.LeftArrow:
                return Direction.Left;
            case ConsoleKey.RightArrow:
                return Direction.Right;
            default: return null;
        }
    }
}


namespace SnakeGame
{
    class Program
    {
        static void Main()
        {
            using (var repository = new GameRepository())
            {
                var game = new ConsoleGameController(new GameService(repository, 3, 30));
                Console.CursorVisible = false;
                while (true)
                {
                    Console.Clear();

                    Console.WriteLine($"MaxScore:{game.MaxScore()}");
                    Console.WriteLine($"CurrentScore:{game.Score()}");

                    var points = game.Draw();

                    foreach (var pointModel in points)
                    {
                        Console.SetCursorPosition(pointModel.X, pointModel.Y + 2);
                        Console.Write(pointModel.Sym);
                    }

                    if (Console.KeyAvailable)
                        game.Input(Console.ReadKey().Key);

                    game.Logic();

                    Thread.Sleep(100);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LiteDB;
//Domen -----------------------------------------------
enum Direction
{
    Top,
    Bottom,
    Left,
    Right
}

class Frame
{
    public int MinX { get; }
    public int MinY { get; }
    public int MaxX { get; }
    public int MaxY { get; }

    public Frame(int minX, int minY, int maxX, int maxY)
    {
        if (maxX <= minX)
            throw new FrameXValidationException(minX,maxX);
        if (maxY <= minY)
            throw new FrameYValidationException(minY,maxY);

        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
    }
}

class FrameXValidationException:Exception
{
    public int StratX { get; set; }
    public int EndX { get; set; }

    public FrameXValidationException(int stratX, int endX)
    {
        StratX = stratX;
        EndX = endX;
    }
}

class FrameYValidationException : Exception
{
    public int StratY { get; set; }
    public int EndY { get; set; }

    public FrameYValidationException(int stratY, int endY)
    {
        StratY = stratY;
        EndY = endY;
    }
}

struct Point
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool Overlaps(Point point)
    {
        return point.X == X && point.Y == Y;
    }

    public bool IsIn(Frame frame)
    {
        return X < frame.MaxX && X > frame.MinX &&
               Y < frame.MaxY && Y > frame.MinY;
    }

    public Point Moved(Direction direction)
    {
        switch (direction)
        {
            case Direction.Top: return new Point(X, Y + 1);
            case Direction.Bottom: return new Point(X, Y - 1);
            case Direction.Right: return new Point(X + 1, Y);
            case Direction.Left: return new Point(X - 1, Y);
            default: throw new ArgumentException("Unknown direction");
        }
    }

    public static Point RandomIn(Frame frame)
    {
        var random = new Random();
        return new Point(random.Next(frame.MinX + 1, frame.MaxX - 1), random.Next(frame.MinY + 1, frame.MaxY - 1));
    }
}

class Food
{
    public Guid Id { get; set; }
    public Point Body { get; private set; }

    public void MoveRandomIn(Frame frame)
    {
        Body = Point.RandomIn(frame);
    }

    public Food(Guid id, Point body)
    {
        Body = body;
        Id = id;
    }

    public Food(Frame frame)
    {
        Id = Guid.NewGuid();
        Body = Point.RandomIn(frame);
    }
}

class SnakeBodyNullException:Exception
{
    
}

class SnakeIdEmptyException:Exception
{
    
}

class SnakeBodyEptyException:Exception
{
    public SnakeBodyEptyException()
    {
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

class GameIdEmptyException:Exception
{
    public GameIdEmptyException()
    {
        
    }
}

class GameSnakeNullExpection:Exception
{
    public GameSnakeNullExpection()
    {
        
    }
}

class GameFrameNullException:Exception
{
    public GameFrameNullException()
    {
        
    }
}

class GameFoodIsNullException:Exception
{
    public GameFoodIsNullException()
    {
        
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
        if(id == Guid.Empty)
            throw new GameIdEmptyException();
        
        if(snake == null)
            throw new GameSnakeNullExpection();
        
        if(frame == null)
            throw new GameFrameNullException();
        
        if(food == null)
            throw new GameFoodIsNullException();
        
        Id = id;
        Snake = snake;
        Frame = frame;
        Food = food;
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
    private readonly int _maxX;
    private readonly int _maxY;
    private Game _game;

    public GameService(IGameRepository gameRepository, int maxX, int maxY)
    {
        _gameRepository = gameRepository;
        _maxX = maxX;
        _maxY = maxY;
        _game = CreateGame(maxX, maxY);
    }

    private Game CreateGame(int maxX, int maxY)
    {
        if (maxX < 4)
            throw new ArgumentException("maxX < 4");
        if (maxY < 4)
            throw new ArgumentException("maxY < 4");

        var frame = new Frame(0, 0, maxX, maxY);
        var food = new Food(frame);
        var list = new LinkedList<Point>();
        list.AddLast(new Point(1, 1));
        list.AddLast(new Point(2, 2));
        list.AddLast(new Point(3, 3));

        var snake = new Snake(list);
        return new Game(snake, frame, food);
    }

    public int GetCurrentScore()
    {
        return _game.Snake.Body.Count - 3;
    }

    public int MaxScore()
    {
        var all = _gameRepository.All();
        return all.Count > 0 ? all.Max(g => g.Snake.Body.Count) - 3 : 0;
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
        if (_game == null || _game.GameOver)
            _game = CreateGame(_maxX, _maxY);
        _game.Logic();
        if (_game.GameOver)
            _gameRepository.Add(_game);
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
        return new Frame(Min.X,Min.Y,Max.X,Max.Y);
    }

    public static FrameDbDto From(Frame frame)
    {
        return new FrameDbDto
        {
            Max = new PointDbDto {X = frame.MaxX, Y = frame.MaxY},
            Min = new PointDbDto {X = frame.MinX, Y = frame.MinY}
        };
    }
}

class FoodBdDto
{
    public Guid Id { get; set; }
    public PointDbDto Body { get; set; }

    public Food To()
    {
        return new Food(Id,Body.To());
    }

    public static FoodBdDto From(Food food)
    {
        return new FoodBdDto
        {
            Id = food.Id,
            Body = PointDbDto.From(food.Body)
        };
    }
}

class SnakeDbDto
{
    public Guid Id { get; set; }
    public List<PointDbDto> Body { get; set; }
    public Direction Direction { get;  set; }

    public Snake To()
    {
        return new Snake(Id,new LinkedList<Point>(Body.Select(p=>p.To())),Direction);
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
        return new Game(Id,Snake.To(),Frame.To(),Food.To());
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
            .Select(g=>g.To()).ToList();
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
                    points.Add(new PointModel {X = i, Y = j, Sym = frameSym});
            }
        }

        foreach (var point in game.Snake.Body)
        {
            points.Add(new PointModel {X = point.X, Y = point.Y, Sym = snakeSym});
        }

        points.Add(new PointModel {X = game.Food.Body.X, Y = game.Food.Body.Y, Sym = foodSym});

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
                var game = new ConsoleGameController(new GameService(repository, 40, 30));
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
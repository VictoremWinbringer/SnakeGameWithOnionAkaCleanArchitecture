using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;

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
        if (maxX <= minY)
            throw new ArgumentException("start.X < start.Y");
        if (maxY <= minY)
            throw new ArgumentException("start.Y < start.Y");

        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
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
        return new Point(random.Next(frame.MinX, frame.MaxX), random.Next(frame.MinY, frame.MaxY));
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

class Snake
{
    public Guid Id { get; }
    public LinkedList<Point> Body { get; }

    public Direction Direction { get; private set; }

    public Snake(Guid id, LinkedList<Point> body, Direction direction)
    {
        if (body == null)
            throw new ArgumentNullException(nameof(body));

        if (body.Count < 3)
            throw new ArgumentException("body.Count < 3");
        
        if(id == Guid.Empty)
            throw new ArgumentException("Empty Id");

        Id = id;
        Body = body;
        Direction = direction;
    }
    
    public Snake(LinkedList<Point> body): this(Guid.NewGuid(), body,Direction.Right){}

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
        var tail = Body.Last.Value;
        Body.RemoveLast();
        tail = tail.Moved(Direction);
        Body.AddFirst(tail);
    }

    public bool IsBitingTail()
    {
        return Body.Any(p => p.Overlaps(Body.First.Value));
    }

    public bool IsHeadIn(Frame frame)
    {
        return Body.First.Value.IsIn(frame);
    }
}

class Game
{
    public Guid Id { get;}
    public Snake Snake { get; }
    public Frame Frame { get; }
    public Food Food { get; }
    
    public bool GameOver { get; private set; }

    private Stopwatch Stopwatch { get; }

    public Game(Guid id, Snake snake, Frame frame, Food food)
    {
        Id = id;
        Snake = snake;
        Frame = frame;
        Food = food;
        Stopwatch = Stopwatch.StartNew();
    }
    
    public Game(Snake snake, Frame frame, Food food):this(Guid.NewGuid(), snake,frame,food){}

    public void Input(Direction direction)
    {
        Snake.Turn(direction);
    }
    
    public void Logic()
    {
        if(GameOver)
            return;
        
        if (Stopwatch.Elapsed.Milliseconds < 100)
            return;
        
        Stopwatch.Restart();
        
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

class GameService:IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly int _maxX;
    private readonly int _maxY;
    private Game _game;
    public GameService(IGameRepository gameRepository,int maxX, int maxY)
    {
        _gameRepository = gameRepository;
        _maxX = maxX;
        _maxY = maxY;
    }
    
    private Game CreateGame(int maxX, int maxY)
    {
        if(maxX < 4) 
            throw new ArgumentException( "maxX < 4");
        if(maxY < 4) 
            throw new ArgumentException("maxY < 4");
        
        var frame = new Frame(0,0,maxX,maxY);
        var food = new Food(frame);
        var list = new LinkedList<Point>();
        list.AddLast(new Point(1, 1));
        list.AddLast(new Point(2, 2));
        list.AddLast(new Point(3, 3));
        
        var snake = new Snake(list);
        return new Game(snake,frame,food);
    }

    public int GetCurrentScore()
    {
        return _game.Snake.Body.Count - 3;
    }

    public int MaxScore()
    {
        return _gameRepository.All().Max(g => g.Snake.Body.Count) - 3;
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

struct PointModel
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
        if(direction.HasValue)
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

    public PointModel[,] Draw()
    {
        var game = _service.Draw();
       List<Point> points = new List<Point>();

       for (int i = game.Frame.MinX; i <= game.Frame.MaxX; i++)
       {
           for (int j = game.Frame.MinY; j <= game.Frame.MaxY; j++)
           {
               if (i == game.Frame.MinX )
               {
                   
               }
           }
       }
       
    }

    private Direction? Parse(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
                return Direction.Top;
            case ConsoleKey.DownArrow:
                return Direction.Top;
            case ConsoleKey.LeftArrow:
                return Direction.Top;
            case ConsoleKey.RightArrow:
                return Direction.Top;
            default: return null;
        }
    }

}



namespace SnakeGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
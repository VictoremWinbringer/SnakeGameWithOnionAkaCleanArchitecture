using System;
using System.Runtime.Serialization;
using System.Threading;


enum DomainExceptionCode
{
    SnakeFromFoodToFar,
    GameFrameToSmallForSnake
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
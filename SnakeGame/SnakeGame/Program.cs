using System;
using System.Threading;
using SnakeGame.ApplicationCore.ApplicationServices;
using SnakeGame.ApplicationCore.DomainServices;
using SnakeGame.Infrastructure;
using SnakeGame.Presentation;


namespace SnakeGame
{
    class Program
    {
        static void Main()
        {
            using (var repository = new GameRepository())
            {
                var gameFactory = new GameFactory(3, 30);
                var pointGenerator = new RandomService(new Random());
                var gameService = new GameService(repository, gameFactory, pointGenerator);
                var gameController = new ConsoleGameController(gameService);
                while (true)
                {
                    Console.Clear();
                    Console.CursorVisible = false;
                    Console.WriteLine($"MaxScore:{gameController.MaxScore}");
                    Console.WriteLine($"CurrentScore:{gameController.Score}");
                    var points = gameController.Draw();
                    foreach (var pointModel in points)
                    {
                        Console.SetCursorPosition(pointModel.X, pointModel.Y + 2);
                        Console.Write(pointModel.Sym);
                    }
                    if (Console.KeyAvailable)
                        gameController.Input(Console.ReadKey().Key);
                    gameController.Logic();
                    Thread.Sleep(100);
                }
            }
        }
    }
}
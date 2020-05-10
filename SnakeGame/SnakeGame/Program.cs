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
                var gameFactory = new GameFactory();
                var gameService = new GameService(repository, gameFactory, 30,3);
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
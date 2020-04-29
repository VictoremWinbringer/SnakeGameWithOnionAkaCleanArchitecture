using System;
using System.Threading;
using SnakeGame.ApplicationServices;
using SnakeGame.DomainServices;
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
                var controller = new ConsoleGameController(new GameService(repository,new GameFactory(3,30)));
                Console.CursorVisible = false;
                while (true)
                {
                    Console.Clear();

                    Console.WriteLine($"MaxScore:{controller.MaxScore}");
                    Console.WriteLine($"CurrentScore:{controller.Score}");

                    var points = controller.Draw();

                    foreach (var pointModel in points)
                    {
                        Console.SetCursorPosition(pointModel.X, pointModel.Y + 2);
                        Console.Write(pointModel.Sym);
                    }

                    if (Console.KeyAvailable)
                        controller.Input(Console.ReadKey().Key);

                    controller.Logic();

                    Thread.Sleep(100);
                }
            }
        }
    }
}
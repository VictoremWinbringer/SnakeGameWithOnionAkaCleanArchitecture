using System;
using System.Collections.Generic;
using SnakeGame.ApplicationServices;
using SnakeGame.Entities.ValueObjects;

namespace SnakeGame.Presentation
{
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

        public int Score => _service.CurrentScore;

        public int MaxScore => _service.MaxScore;

        public List<PointModel> Draw()
        {
            var game = _service.Get;
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

            foreach (var point in game.Snake.Body.Value)
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
}

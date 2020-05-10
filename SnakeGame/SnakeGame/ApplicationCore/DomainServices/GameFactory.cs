using System;
using System.Collections.Generic;
using SnakeGame.ApplicationCore.Entities;
using SnakeGame.ApplicationCore.Entities.ValueObjects;

namespace SnakeGame.ApplicationCore.DomainServices
{
    class GameFactory : IGameFactory
    {
        public GameFactory()
        {
        }

        public Game Create(int offset, int height, int snakeLength)
        {
            var frame = new Frame(offset, offset, height, height);
            var food = new Food(new Point());
            food.MoveRandomIn(frame);
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
}

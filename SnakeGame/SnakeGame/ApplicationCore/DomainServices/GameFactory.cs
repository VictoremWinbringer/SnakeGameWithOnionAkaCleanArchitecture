using System.Collections.Generic;
using SnakeGame.ApplicationCore.Entities;
using SnakeGame.ApplicationCore.Entities.ValueObjects;

namespace SnakeGame.ApplicationCore.DomainServices
{
    class GameFactory:IGameFactory
    {
        private readonly int snakeLength;
        private readonly int height;

        public GameFactory(int snakeLength, int height)
        {
            this.snakeLength = snakeLength;
            this.height = height;
        }
        public Game Create()
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
            return new Game(snake, frame, food);
        }
    }
}

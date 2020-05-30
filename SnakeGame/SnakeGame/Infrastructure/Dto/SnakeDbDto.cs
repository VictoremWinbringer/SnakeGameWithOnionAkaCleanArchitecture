using System;
using System.Collections.Generic;
using System.Linq;
using SnakeGame.ApplicationCore.Entities;
using SnakeGame.ApplicationCore.Entities.ValueObjects;

namespace SnakeGame.Infrastructure.Dto
{
    class SnakeDbDto
    {
        public List<PointDbDto> Body { get; set; }
        public Direction Direction { get; set; }

        public Snake To()
        {
            return new Snake(new SnakeBody(new LinkedList<Point>(Body.Select(p => p.To()))), Direction);
        }

        public static SnakeDbDto From(Snake snake)
        {
            return new SnakeDbDto
            {
                Body = snake.Body.Value.Select(PointDbDto.From).ToList(),
                Direction = snake.Direction
            };
        }
    }
}

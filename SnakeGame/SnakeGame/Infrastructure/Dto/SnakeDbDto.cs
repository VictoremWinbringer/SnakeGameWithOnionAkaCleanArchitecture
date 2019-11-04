using System;
using System.Collections.Generic;
using System.Linq;

class SnakeDbDto
{
    public Guid Id { get; set; }
    public List<PointDbDto> Body { get; set; }
    public Direction Direction { get; set; }

    public Snake To()
    {
        return new Snake(new SnakeId(Id), new SnakeBody(new LinkedList<Point>(Body.Select(p => p.To()))), Direction);
    }

    public static SnakeDbDto From(Snake snake)
    {
        return new SnakeDbDto
        {
            Id = snake.Id.Value,
            Body = snake.Body.Value.Select(PointDbDto.From).ToList(),
            Direction = snake.Direction
        };
    }
}

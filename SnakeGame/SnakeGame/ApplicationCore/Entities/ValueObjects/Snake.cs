﻿using System;

namespace SnakeGame.ApplicationCore.Entities.ValueObjects
{
    class Snake
    {
        public SnakeBody Body { get; }

        public Direction Direction { get; private set; }

        public Snake(SnakeBody body, Direction direction)
        {
            Body = body ?? throw new ApplicationException("Body is null!");
            Direction = direction;
        }

        public Snake(SnakeBody body) : this(body, Direction.Right)
        {
        }

        public void Turn(Direction direction) => Direction = direction;

        public void Eat(Food food)
        {
            if (!CanEat(food))
            {
                var ex = new ApplicationException("Snake to far from food");
                ex.Data[nameof(food.Body)] = food.Body;
                ex.Data[nameof(Body.Last)] = Body.Last;
                throw ex;
            }

            Body.AddLast(food.Body);
        }

        public bool CanEat(Food food) => food.Body.Overlaps(Body.Head);

        public void Move()
        {
            var head = Body.Head;
            Body.RemoveLast();
            var point = head.Moved(Direction);
            Body.SetHead(point);
        }

        public bool IsBitingTail => Body.BitesSelf;

        public bool IsHeadIn(Frame frame) => Body.Head.IsIn(frame);
    }
}

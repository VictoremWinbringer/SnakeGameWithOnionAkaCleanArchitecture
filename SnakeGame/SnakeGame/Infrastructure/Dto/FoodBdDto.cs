using System;
using SnakeGame.ApplicationCore.Entities;
using SnakeGame.ApplicationCore.Entities.ValueObjects;

namespace SnakeGame.Infrastructure.Dto
{
    class FoodBdDto
    {
        public PointDbDto Body { get; set; }

        public Food To()
        {
            return new Food(Body.To(), new Random());
        }

        public static FoodBdDto From(Food food)
        {
            return new FoodBdDto
            {
                Body = PointDbDto.From(food.Body)
            };
        }
    }
}

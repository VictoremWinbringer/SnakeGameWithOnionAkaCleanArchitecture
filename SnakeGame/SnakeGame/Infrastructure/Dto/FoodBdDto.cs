using System;
using SnakeGame.Entities;

namespace SnakeGame.Infrastructure.Dto
{
    class FoodBdDto
    {
        public Guid Id { get; set; }
        public PointDbDto Body { get; set; }

        public Food To()
        {
            return new Food(Id, Body.To());
        }

        public static FoodBdDto From(Food food)
        {
            return new FoodBdDto
            {
                Id = food.Id,
                Body = PointDbDto.From(food.Body)
            };
        }
    }
}

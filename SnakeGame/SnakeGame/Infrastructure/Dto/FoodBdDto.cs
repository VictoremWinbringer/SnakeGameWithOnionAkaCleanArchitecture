using System;

class FoodBdDto
{
    public Guid Id { get; set; }
    public PointDbDto Body { get; set; }

    public Food To()
    {
        return new Food(new FoodId(Id), Body.To());
    }

    public static FoodBdDto From(Food food)
    {
        return new FoodBdDto
        {
            Id = food.Id.Value,
            Body = PointDbDto.From(food.Body)
        };
    }
}

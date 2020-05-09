using System;
using SnakeGame.ApplicationCore.Entities;

namespace SnakeGame.Infrastructure.Dto
{
    class GameDbDto
    {
        public Guid Id { get; set; }
        public SnakeDbDto Snake { get; set; }
        public FrameDbDto Frame { get; set; }
        public FoodBdDto Food { get; set; }
        public int Score { get; set; }
        public Game To()
        {
            return new Game(Id, Snake.To(), Frame.To(), Food.To(), Score);
        }

        public static GameDbDto From(Game game)
        {
            return new GameDbDto
            {
                Id = game.Id,
                Snake = SnakeDbDto.From(game.Snake),
                Food = FoodBdDto.From(game.Food),
                Frame = FrameDbDto.From(game.Frame),
                Score = game.Score
            };
        }
    }
}

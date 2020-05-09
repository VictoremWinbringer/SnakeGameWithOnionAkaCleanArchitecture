using SnakeGame.ApplicationCore.Entities;
using SnakeGame.ApplicationCore.Entities.ValueObjects;

namespace SnakeGame.ApplicationCore.ApplicationServices
{
    interface IGameService
    {
        int CurrentScore { get; }
        int MaxScore { get; }
        Game Get { get; }
        void Input(Direction direction);
        void Logic();
    }
}

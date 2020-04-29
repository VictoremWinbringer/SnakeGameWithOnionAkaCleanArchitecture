﻿using SnakeGame.Entities;
using SnakeGame.Entities.ValueObjects;

namespace SnakeGame.ApplicationServices
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
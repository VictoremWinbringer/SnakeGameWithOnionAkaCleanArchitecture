﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SnakeGame.ApplicationCore.ApplicationServices.Infrastructure.Interfaces;
using SnakeGame.ApplicationCore.DomainServices;
using SnakeGame.ApplicationCore.Entities;
using SnakeGame.ApplicationCore.Entities.ValueObjects;
using SnakeGame.Infrastructure;

namespace SnakeGame.ApplicationCore.ApplicationServices
{
    class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGameFactory _factory;
        private readonly int _gameHeight;
        private readonly int _snakeLength;
        private Game _game;

        public GameService(IGameRepository gameRepository, IGameFactory factory, int gameHeight, int snakeLength)
        {
            _gameRepository = gameRepository ?? throw new ApplicationException(nameof(gameRepository));
            _factory = factory ?? throw new ApplicationException(nameof(factory));
            _gameHeight = gameHeight;
            _snakeLength = snakeLength;
            _game = _factory.Create(0, gameHeight, snakeLength);
        }

        public int CurrentScore => _game.Score;

        public int MaxScore
        {
            get
            {
                var all = _gameRepository.All();
                return all.Count > 0 ? all.Max(g => g.Score) : 0;
            }
        }

        public Game Get => _game;

        public void Input(Direction direction)
        {
            _game.Input(direction);
        }

        public void Logic()
        {
            _game.Logic();
            if (_game.GameOver)
            {
                _gameRepository.Add(_game);
                _game = _factory.Create(0, _gameHeight, _snakeLength);
            }
        }
    }
}

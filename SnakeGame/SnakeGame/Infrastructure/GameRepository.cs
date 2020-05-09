using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using SnakeGame.ApplicationCore.Entities;
using SnakeGame.Infrastructure.Dto;

namespace SnakeGame.Infrastructure
{
    class GameRepository : IGameRepository, IDisposable
    {
        private readonly LiteDatabase _db;

        public GameRepository()
        {
            _db = new LiteDatabase("Filename=./game.db;Mode=Exclusive");
        }

        public List<Game> All()
        {
            return _db.GetCollection<GameDbDto>().FindAll()
                .Select(g => g.To()).ToList();
        }

        public void Add(Game game)
        {
            _db.GetCollection<GameDbDto>().Insert(GameDbDto.From(game));
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}

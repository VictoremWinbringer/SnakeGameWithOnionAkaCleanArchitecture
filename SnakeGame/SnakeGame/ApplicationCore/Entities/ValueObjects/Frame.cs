using System;

namespace SnakeGame.ApplicationCore.Entities.ValueObjects
{
    class Frame
    {
        public int MinX { get; }
        public int MinY { get; }
        public int MaxX { get; }
        public int MaxY { get; }

        public Frame(int minX, int minY, int maxX, int maxY)
        {
            if (maxX <= minX)
                throw new ApplicationException(nameof(maxX));
            if (maxY <= minY)
                throw new ApplicationException(nameof(maxX));
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }
    }
}

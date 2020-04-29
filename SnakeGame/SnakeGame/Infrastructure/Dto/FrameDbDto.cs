using SnakeGame.Entities.ValueObjects;

namespace SnakeGame.Infrastructure.Dto
{
    class FrameDbDto
    {
        public PointDbDto Min { get; set; }
        public PointDbDto Max { get; set; }

        public Frame To()
        {
            return new Frame(Min.X, Min.Y, Max.X, Max.Y);
        }

        public static FrameDbDto From(Frame frame)
        {
            return new FrameDbDto
            {
                Max = new PointDbDto { X = frame.MaxX, Y = frame.MaxY },
                Min = new PointDbDto { X = frame.MinX, Y = frame.MinY }
            };
        }
    }
}

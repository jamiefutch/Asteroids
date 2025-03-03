using System.Diagnostics.CodeAnalysis;

namespace Asteroids
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]

    public class Asteroid
    {
        public PointF Position { get; private set; }
        public bool IsAlive { get; private set; }
        private float rotation;
        private float speed;
        private static Random rand = new Random();

        public Asteroid(Size clientSize)
        {
            Position = new PointF(rand.Next(clientSize.Width), rand.Next(clientSize.Height));
            rotation = (float)(rand.NextDouble() * Math.PI * 2);
            speed = (float)(rand.NextDouble() * 3 + 1);
            IsAlive = true;
        }

        public void Update(Size clientSize)
        {
            Position = new PointF(
                (Position.X + (float)Math.Cos(rotation) * speed + clientSize.Width) % clientSize.Width,
                (Position.Y + (float)Math.Sin(rotation) * speed + clientSize.Height) % clientSize.Height
            );
        }

        public void Draw(Graphics g)
        {
            PointF[] asteroidPoints = new PointF[]
            {
                new PointF(Position.X + 10, Position.Y),
                new PointF(Position.X + 5, Position.Y + 10),
                new PointF(Position.X - 5, Position.Y + 10),
                new PointF(Position.X - 10, Position.Y),
                new PointF(Position.X - 5, Position.Y - 10),
                new PointF(Position.X + 5, Position.Y - 10)
            };
            g.DrawPolygon(Pens.White, asteroidPoints);

        }
    }
}
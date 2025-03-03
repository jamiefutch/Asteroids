using System.Diagnostics.CodeAnalysis;

namespace Asteroids
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]

    public class Bullet
    {
        public PointF Position { get; private set; }
        public bool IsAlive { get; private set; }
        private float rotation;
        private float speed;
        private int lifetime;

        public Bullet(PointF startPosition, float rotation)
        {
            Position = startPosition;
            this.rotation = rotation;
            speed = 5;
            lifetime = 60; // 1 second at 60 FPS
            IsAlive = true;
        }

        public void Update(Size clientSize)
        {
            Position = new PointF(
                (Position.X + (float)Math.Cos(rotation) * speed + clientSize.Width) % clientSize.Width,
                (Position.Y + (float)Math.Sin(rotation) * speed + clientSize.Height) % clientSize.Height
            );
            lifetime--;
            if (lifetime <= 0)
            {
                IsAlive = false;
            }
        }

        public void Draw(Graphics g)
        {
            g.DrawEllipse(Pens.White, Position.X - 2, Position.Y - 2, 4, 4);
        }
    }
}
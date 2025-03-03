using System.Diagnostics.CodeAnalysis;

namespace Asteroids
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]

    public class Spaceship
    {
        public PointF Position { get; private set; }
        public float Rotation { get; private set; }
        private float speed;
        private bool isThrusting;

        public Spaceship(PointF startPosition)
        {
            Position = startPosition;
            Rotation = 0;
            speed = 0;
            isThrusting = false;
        }

        public void Update(Size clientSize)
        {
            if (isThrusting)
            {
                speed += 0.1f;
            }
            else
            {
                speed *= 0.99f;
            }

            Position = new PointF(
                (Position.X + (float)Math.Cos(Rotation) * speed + clientSize.Width) % clientSize.Width,
                (Position.Y + (float)Math.Sin(Rotation) * speed + clientSize.Height) % clientSize.Height
            );
        }

        public void Draw(Graphics g)
        {
            PointF[] shipPoints = new PointF[]
            {
                new PointF(Position.X + (float)Math.Cos(Rotation) * 10, Position.Y + (float)Math.Sin(Rotation) * 10),
                new PointF(Position.X + (float)Math.Cos(Rotation + 2.5) * 10, Position.Y + (float)Math.Sin(Rotation + 2.5) * 10),
                new PointF(Position.X + (float)Math.Cos(Rotation - 2.5) * 10, Position.Y + (float)Math.Sin(Rotation - 2.5) * 10)
            };
            g.DrawPolygon(Pens.White, shipPoints);
        }

        public void OnKeyDown(Keys key)
        {
            if (key == Keys.Up)
            {
                isThrusting = true;
            }
            if (key == Keys.Left)
            {
                Rotation -= 0.1f;
            }
            if (key == Keys.Right)
            {
                Rotation += 0.1f;
            }
        }

        public void OnKeyUp(Keys key)
        {
            if (key == Keys.Up)
            {
                isThrusting = false;
            }
        }
    }
}
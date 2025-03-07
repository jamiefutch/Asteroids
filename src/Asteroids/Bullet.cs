namespace Asteroids
{
    public class Bullet
    {
        public PointF Position { get; private set; }
        public bool IsActive { get; private set; }
        
        private float angle;
        private const float Speed = 7.0f;
        private int lifetime;
        private const int MaxLifetime = 60; // 1 second at 60 FPS
        
        public Bullet(PointF position, float shipAngle)
        {
            // Start bullet at the nose of the ship
            Position = new PointF(
                position.X + (float)Math.Cos(shipAngle) * 20,
                position.Y + (float)Math.Sin(shipAngle) * 20
            );
            angle = shipAngle;
            IsActive = true;
            lifetime = MaxLifetime;
        }

        public void Update(Size playArea)
        {
            if (!IsActive) return;
            
            // Move bullet
            Position = new PointF(
                (Position.X + (float)Math.Cos(angle) * Speed + playArea.Width) % playArea.Width,
                (Position.Y + (float)Math.Sin(angle) * Speed + playArea.Height) % playArea.Height
            );
            
            // Reduce lifetime
            lifetime--;
            if (lifetime <= 0)
            {
                IsActive = false;
            }
        }

        public void Draw(Graphics g)
        {
            if (!IsActive) return;
            
            g.FillEllipse(
                Brushes.White,
                Position.X - 2,
                Position.Y - 2,
                4,
                4
            );
        }
    }
}
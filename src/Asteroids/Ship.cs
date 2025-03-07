namespace Asteroids
{
    public class Ship
    {
        public PointF Position { get; private set; }
        public float Angle { get; private set; }
        public PointF Velocity { get; private set; }
        
        private bool isThrusting;
        private bool isRotatingLeft;
        private bool isRotatingRight;
        private const float ThrustPower = 0.15f;
        private const float RotationSpeed = 0.1f;
        private const float Drag = 0.99f;
        
        public Ship(PointF startPosition)
        {
            Position = startPosition;
            Angle = -(float)Math.PI / 2; // Start pointing up
            Velocity = new PointF(0, 0);
            isThrusting = false;
            isRotatingLeft = false;
            isRotatingRight = false;
        }

        public void Update(Size playArea)
        {
            // Update rotation
            if (isRotatingLeft)
                Angle -= RotationSpeed;
            if (isRotatingRight)
                Angle += RotationSpeed;
                
            // Apply thrust
            if (isThrusting)
            {
                Velocity = new PointF(
                    Velocity.X + (float)Math.Cos(Angle) * ThrustPower,
                    Velocity.Y + (float)Math.Sin(Angle) * ThrustPower
                );
            }
            
            // Apply drag
            Velocity = new PointF(Velocity.X * Drag, Velocity.Y * Drag);
            
            // Update position
            Position = new PointF(
                (Position.X + Velocity.X + playArea.Width) % playArea.Width,
                (Position.Y + Velocity.Y + playArea.Height) % playArea.Height
            );
        }

        public void Draw(Graphics g)
        {
            // Calculate triangle points for ship
            float size = 15;
            PointF[] shipPoints = new PointF[3];
            shipPoints[0] = new PointF(
                Position.X + (float)Math.Cos(Angle) * size,
                Position.Y + (float)Math.Sin(Angle) * size
            );
            shipPoints[1] = new PointF(
                Position.X + (float)Math.Cos(Angle + 2.6f) * (size * 0.6f),
                Position.Y + (float)Math.Sin(Angle + 2.6f) * (size * 0.6f)
            );
            shipPoints[2] = new PointF(
                Position.X + (float)Math.Cos(Angle - 2.6f) * (size * 0.6f),
                Position.Y + (float)Math.Sin(Angle - 2.6f) * (size * 0.6f)
            );
            
            g.DrawPolygon(Pens.White, shipPoints);
            
            // Draw thrust flame when thrusting
            if (isThrusting)
            {
                PointF[] thrustPoints = new PointF[3];
                thrustPoints[0] = shipPoints[1];
                thrustPoints[1] = shipPoints[2];
                thrustPoints[2] = new PointF(
                    Position.X + (float)Math.Cos(Angle + Math.PI) * (size * 0.7f),
                    Position.Y + (float)Math.Sin(Angle + Math.PI) * (size * 0.7f)
                );
                
                g.DrawPolygon(Pens.Orange, thrustPoints);
            }
        }

        public void StartThrusting() => isThrusting = true;
        public void StopThrusting() => isThrusting = false;
        public void StartRotatingLeft() => isRotatingLeft = true;
        public void StopRotatingLeft() => isRotatingLeft = false;
        public void StartRotatingRight() => isRotatingRight = true;
        public void StopRotatingRight() => isRotatingRight = false;
        
        public bool CollidesWith(Asteroid asteroid)
        {
            float distance = (float)Math.Sqrt(
                Math.Pow(Position.X - asteroid.Position.X, 2) + 
                Math.Pow(Position.Y - asteroid.Position.Y, 2)
            );
            return distance < asteroid.Radius + 10;
        }
    }
}
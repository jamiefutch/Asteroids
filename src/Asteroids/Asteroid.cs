namespace Asteroids
{
    public class Asteroid
    {
        public PointF Position { get; private set; }
        public int Size { get; private set; } // 0=small, 1=medium, 2=large
        public float Radius { get; private set; }
        
        private float speed;
        private float angle;
        private PointF[] points;
        
        public Asteroid(PointF position, int size, Random random, float speed = 0, float angle = 0)
        {
            Position = position;
            Size = size;
            
            // Set size-dependent properties
            switch (Size)
            {
                case 0: // Small
                    Radius = 10;
                    break;
                case 1: // Medium
                    Radius = 20;
                    break;
                case 2: // Large
                    Radius = 40;
                    break;
            }
            
            // If speed/angle weren't specified, randomize them
            if (speed == 0)
                this.speed = (float)(random.NextDouble() * 2 + 0.5);
            else
                this.speed = speed;
                
            if (angle == 0)
                this.angle = (float)(random.NextDouble() * Math.PI * 2);
            else
                this.angle = angle;
                
            // Generate irregular polygon points for the asteroid
            GeneratePoints(random);
        }

        private void GeneratePoints(Random random)
        {
            int numPoints = 8 + Size * 2; // More points for larger asteroids
            points = new PointF[numPoints];
            
            for (int i = 0; i < numPoints; i++)
            {
                float angle = (float)(i * 2 * Math.PI / numPoints);
                float variance = (float)(random.NextDouble() * 0.4 + 0.8); // 0.8 to 1.2
                float radius = Radius * variance;
                
                points[i] = new PointF(
                    (float)Math.Cos(angle) * radius,
                    (float)Math.Sin(angle) * radius
                );
            }
        }

        public void Update(Size playArea)
        {
            // Move asteroid
            float dx = (float)Math.Cos(angle) * speed;
            float dy = (float)Math.Sin(angle) * speed;
            
            Position = new PointF(
                (Position.X + dx + playArea.Width) % playArea.Width,
                (Position.Y + dy + playArea.Height) % playArea.Height
            );
        }

        public void Draw(Graphics g)
        {
            PointF[] drawPoints = new PointF[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                drawPoints[i] = new PointF(
                    Position.X + points[i].X,
                    Position.Y + points[i].Y
                );
            }
            
            g.DrawPolygon(Pens.White, drawPoints);
        }
        
        public bool CollidesWith(Bullet bullet)
        {
            float distance = (float)Math.Sqrt(
                Math.Pow(Position.X - bullet.Position.X, 2) + 
                Math.Pow(Position.Y - bullet.Position.Y, 2)
            );
            return distance < Radius;
        }
        
        public bool CollidesWith(Ship ship)
        {
            float distance = (float)Math.Sqrt(
                Math.Pow(Position.X - ship.Position.X, 2) + 
                Math.Pow(Position.Y - ship.Position.Y, 2)
            );
            return distance < Radius + 10; // Ship radius approximation
        }
    }
}
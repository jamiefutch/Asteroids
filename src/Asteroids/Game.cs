namespace Asteroids
{
    public class Game
    {
        public Ship Ship { get; private set; }
        public List<Asteroid> Asteroids { get; private set; }
        public List<Bullet> Bullets { get; private set; }
        public Size PlayArea { get; private set; }
        public bool IsGameOver { get; private set; }
        public int Score { get; private set; }
        public int Level { get; private set; }
        private Random random;

        public Game(Size playArea)
        {
            PlayArea = playArea;
            random = new Random();
            Reset();
        }

        public void Reset()
        {
            Ship = new Ship(new PointF(PlayArea.Width / 2, PlayArea.Height / 2));
            Asteroids = new List<Asteroid>();
            Bullets = new List<Bullet>();
            Score = 0;
            Level = 1;
            IsGameOver = false;
            SpawnAsteroids(Level * 2 + 3);
        }

        public void Update()
        {
            if (IsGameOver) return;

            Ship.Update(PlayArea);
            
            foreach (var bullet in Bullets.ToArray())
            {
                bullet.Update(PlayArea);
                if (!bullet.IsActive)
                {
                    Bullets.Remove(bullet);
                }
            }
            
            foreach (var asteroid in Asteroids.ToArray())
            {
                asteroid.Update(PlayArea);
                
                // Check ship collision
                if (asteroid.CollidesWith(Ship))
                {
                    IsGameOver = true;
                    return;
                }
                
                // Check bullet collision
                foreach (var bullet in Bullets.ToArray())
                {
                    if (bullet.IsActive && asteroid.CollidesWith(bullet))
                    {
                        Score += (3 - asteroid.Size + 1) * 100;
                        Bullets.Remove(bullet);
                        
                        if (asteroid.Size > 0)
                        {
                            SpawnSplitAsteroids(asteroid);
                        }
                        
                        Asteroids.Remove(asteroid);
                        break;
                    }
                }
            }

            // Level progression
            if (Asteroids.Count == 0)
            {
                Level++;
                SpawnAsteroids(Level * 2 + 3);
            }
        }

        public void Draw(Graphics g)
        {
            Ship.Draw(g);
            
            foreach (var asteroid in Asteroids)
            {
                asteroid.Draw(g);
            }
            
            foreach (var bullet in Bullets)
            {
                bullet.Draw(g);
            }
        }

        private void SpawnAsteroids(int count)
        {
            for (int i = 0; i < count; i++)
            {
                float x, y;
                // Make sure asteroids don't spawn on the ship
                do
                {
                    x = random.Next(PlayArea.Width);
                    y = random.Next(PlayArea.Height);
                } while (Math.Sqrt(Math.Pow(x - Ship.Position.X, 2) + Math.Pow(y - Ship.Position.Y, 2)) < 100);
                
                Asteroids.Add(new Asteroid(new PointF(x, y), 2, random));
            }
        }

        private void SpawnSplitAsteroids(Asteroid parent)
        {
            for (int i = 0; i < 2; i++)
            {
                float angle = (float)(random.NextDouble() * Math.PI * 2);
                float speed = (float)(random.NextDouble() * 2 + 1);
                
                Asteroids.Add(new Asteroid(
                    parent.Position,
                    parent.Size - 1,
                    random,
                    speed,
                    angle
                ));
            }
        }

        public void Shoot()
        {
            Bullets.Add(new Bullet(Ship.Position, Ship.Angle));
        }

        public void HandleKeyDown(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.Left:
                    Ship.StartRotatingLeft();
                    break;
                case Keys.Right:
                    Ship.StartRotatingRight();
                    break;
                case Keys.Up:
                    Ship.StartThrusting();
                    break;
                case Keys.Space:
                    Shoot();
                    break;
            }
        }

        public void HandleKeyUp(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.Left:
                    Ship.StopRotatingLeft();
                    break;
                case Keys.Right:
                    Ship.StopRotatingRight();
                    break;
                case Keys.Up:
                    Ship.StopThrusting();
                    break;
            }
        }
        
        // ML-specific methods
        public float[] GetGameState()
        {
            // Create a state vector for ML model input
            // This will contain the ship position, rotation, velocity, 
            // and information about nearby asteroids
            
            List<float> state = new List<float>();
            
            // Ship state
            state.Add(Ship.Position.X / PlayArea.Width);  // Normalize to 0-1
            state.Add(Ship.Position.Y / PlayArea.Height); // Normalize to 0-1
            state.Add((float)(Math.Sin(Ship.Angle)));
            state.Add((float)(Math.Cos(Ship.Angle)));
            state.Add(Ship.Velocity.X / 10f); // Normalize velocity
            state.Add(Ship.Velocity.Y / 10f);
            
            // Nearest asteroids (up to 5)
            var sortedAsteroids = new List<Asteroid>(Asteroids);
            sortedAsteroids.Sort((a, b) => 
                DistanceBetween(a.Position, Ship.Position).CompareTo(
                    DistanceBetween(b.Position, Ship.Position)));
            
            for (int i = 0; i < 5; i++)
            {
                if (i < sortedAsteroids.Count)
                {
                    var asteroid = sortedAsteroids[i];
                    float relX = (asteroid.Position.X - Ship.Position.X) / PlayArea.Width;
                    float relY = (asteroid.Position.Y - Ship.Position.Y) / PlayArea.Height;
                    state.Add(relX);
                    state.Add(relY);
                    state.Add(asteroid.Size / 2f); // Size 0-2, normalize to 0-1
                }
                else
                {
                    // Pad with no asteroids
                    state.Add(0);
                    state.Add(0);
                    state.Add(0);
                }
            }
            
            return state.ToArray();
        }
        
        public void PerformAction(int actionId)
        {
            // Actions: 0 = no-op, 1 = thrust, 2 = rotate left, 3 = rotate right, 4 = shoot
            switch (actionId)
            {
                case 0:
                    Ship.StopThrusting();
                    Ship.StopRotatingLeft();
                    Ship.StopRotatingRight();
                    break;
                case 1:
                    Ship.StartThrusting();
                    break;
                case 2:
                    Ship.StartRotatingLeft();
                    break;
                case 3:
                    Ship.StartRotatingRight();
                    break;
                case 4:
                    Shoot();
                    break;
            }
        }
        
        private float DistanceBetween(PointF p1, PointF p2)
        {
            return (float)Math.Sqrt(
                Math.Pow(p2.X - p1.X, 2) + 
                Math.Pow(p2.Y - p1.Y, 2));
        }
    }
}
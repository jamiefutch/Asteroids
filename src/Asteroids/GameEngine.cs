using System.Diagnostics.CodeAnalysis;

namespace Asteroids
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
    public class GameEngine
    {
        private Size clientSize;
        private Spaceship spaceship;
        private List<Asteroid> asteroids;
        private List<Bullet> bullets;

        public GameEngine(Size clientSize)
        {
            this.clientSize = clientSize;
            spaceship = new Spaceship(new PointF(clientSize.Width / 2, clientSize.Height / 2));
            asteroids = new List<Asteroid>();
            bullets = new List<Bullet>();
            // Add initial asteroids
            for (int i = 0; i < 5; i++)
            {
                asteroids.Add(new Asteroid(clientSize));
            }
        }

        public void Update()
        {
            spaceship.Update(clientSize);
            foreach (var bullet in bullets)
            {
                bullet.Update(clientSize);
            }
            foreach (var asteroid in asteroids)
            {
                asteroid.Update(clientSize);
            }
            // Check for collisions and remove hit asteroids
            bullets.RemoveAll(bullet => !bullet.IsAlive);
            asteroids.RemoveAll(asteroid => !asteroid.IsAlive);
        }

        public void Draw(Graphics g)
        {
            spaceship.Draw(g);
            foreach (var bullet in bullets)
            {
                bullet.Draw(g);
            }
            foreach (var asteroid in asteroids)
            {
                asteroid.Draw(g);
            }
        }

        public void OnKeyDown(Keys key)
        {
            spaceship.OnKeyDown(key);
            if (key == Keys.Space)
            {
                bullets.Add(new Bullet(spaceship.Position, spaceship.Rotation));
            }
        }

        public void OnKeyUp(Keys key)
        {
            spaceship.OnKeyUp(key);
        }

    }
}
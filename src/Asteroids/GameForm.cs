using Timer = System.Windows.Forms.Timer;

namespace Asteroids
{
    public class GameForm : Form
    {
        private GameEngine gameEngine;

        public GameForm()
        {
            this.DoubleBuffered = true;
            this.ClientSize = new Size(800, 600);
            this.Text = "Asteroids Clone";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.Black; // Set background color to black

            this.MaximizeBox = false;
            gameEngine = new GameEngine(this.ClientSize);
            this.Paint += new PaintEventHandler(this.OnPaint);
            this.KeyDown += new KeyEventHandler(this.OnKeyDown);
            this.KeyUp += new KeyEventHandler(this.OnKeyUp);
            Timer timer = new Timer();
            timer.Interval = 16; // ~60 FPS
            timer.Tick += new EventHandler(this.OnTick);
            timer.Start();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            gameEngine.Draw(e.Graphics);
        }

        private void OnTick(object sender, EventArgs e)
        {
            gameEngine.Update();
            this.Invalidate();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            gameEngine.OnKeyDown(e.KeyCode);
        }

        private void InitializeComponent()
        {

        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            gameEngine.OnKeyUp(e.KeyCode);
        }
    }
}
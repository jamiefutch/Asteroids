using Timer = System.Windows.Forms.Timer;

namespace Asteroids
{
    public class GameForm : Form
    {
        private Game game;
        private Timer gameTimer;
        private bool isTraining = false;
        private AIPlayer aiPlayer;

        public GameForm()
        {
            this.DoubleBuffered = true;
            this.ClientSize = new Size(800, 600);
            this.BackColor = Color.Black;
            this.Text = "Asteroids ML";
            
            game = new Game(this.ClientSize);
            aiPlayer = new AIPlayer(game);
            
            this.Paint += GameForm_Paint;
            this.KeyDown += GameForm_KeyDown;
            this.KeyUp += GameForm_KeyUp;

            gameTimer = new Timer();
            gameTimer.Interval = 16; // ~60 FPS
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            // Setup menu
            var mainMenu = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("Game");
            var trainAI = new ToolStripMenuItem("Train AI");
            var playWithAI = new ToolStripMenuItem("Play with AI");
            var playManually = new ToolStripMenuItem("Play Manually");
            
            trainAI.Click += (s, e) => StartTrainingMode();
            playWithAI.Click += (s, e) => StartAIPlayMode();
            playManually.Click += (s, e) => StartManualPlayMode();
            
            fileMenu.DropDownItems.Add(trainAI);
            fileMenu.DropDownItems.Add(playWithAI);
            fileMenu.DropDownItems.Add(playManually);
            mainMenu.Items.Add(fileMenu);
            
            this.MainMenuStrip = mainMenu;
            this.Controls.Add(mainMenu);
        }

        private void StartTrainingMode()
        {
            isTraining = true;
            game.Reset();
            aiPlayer.StartTraining();
        }

        private void StartAIPlayMode()
        {
            isTraining = false;
            game.Reset();
            aiPlayer.StartPlaying();
        }

        private void StartManualPlayMode()
        {
            isTraining = false;
            game.Reset();
            aiPlayer.StopAI();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (isTraining)
            {
                aiPlayer.Update();
            }
            else
            {
                game.Update();
            }
            this.Invalidate();
        }

        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            game.Draw(e.Graphics);
            
            // Display training info when in training mode
            if (isTraining)
            {
                e.Graphics.DrawString($"Training: Episode {aiPlayer.CurrentEpisode}", 
                    new Font("Arial", 12), Brushes.White, 10, 30);
                e.Graphics.DrawString($"Score: {game.Score}", 
                    new Font("Arial", 12), Brushes.White, 10, 50);
            }
            else
            {
                e.Graphics.DrawString($"Score: {game.Score}", 
                    new Font("Arial", 12), Brushes.White, 10, 30);
            }
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isTraining) game.HandleKeyDown(e.KeyCode);
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (!isTraining) game.HandleKeyUp(e.KeyCode);
        }
    }
}
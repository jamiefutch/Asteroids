using Microsoft.ML;
using Microsoft.ML.Data;

namespace Asteroids
{
    public class AIPlayer
    {
        private Game game;
        private MLContext mlContext;
        private ITransformer model;
        private PredictionEngine<GameState, ActionPrediction> predictionEngine;
        private List<TrainingExample> trainingData;
        private Random random;
        private bool isTraining;
        private bool isAIPlaying;
        private float explorationRate;
        private int gameCount;
        private const int MaxTrainingExamples = 10000;
        private const int TrainingEpisodes = 1000;
        private const string ModelPath = "asteroid_model.zip";
        
        public int CurrentEpisode { get; private set; }

        public AIPlayer(Game game)
        {
            this.game = game;
            mlContext = new MLContext(seed: 42);
            random = new Random(42);
            trainingData = new List<TrainingExample>();
            explorationRate = 1.0f;
            CurrentEpisode = 0;
            
            // Try to load existing model
            if (File.Exists(ModelPath))
            {
                try
                {
                    model = mlContext.Model.Load(ModelPath, out _);
                    Console.WriteLine("Model loaded successfully!");
                    
                    // Initialize prediction engine when model is loaded
                    predictionEngine = mlContext.Model.CreatePredictionEngine<GameState, ActionPrediction>(model);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading model: {ex.Message}");
                    model = null;
                }
            }
        }

        public void StartTraining()
        {
            isTraining = true;
            isAIPlaying = false;
            gameCount = 0;
            CurrentEpisode = 0;
            explorationRate = 1.0f;
            trainingData.Clear();
        }

        public void StartPlaying()
        {
            isTraining = false;
            isAIPlaying = true;
            
            if (model == null)
            {
                Console.WriteLine("No trained model available.");
                return;
            }
            
            // This is now redundant if already initialized in constructor or after training,
            // but keeping it for safety
            if (predictionEngine == null)
            {
                predictionEngine = mlContext.Model.CreatePredictionEngine<GameState, ActionPrediction>(model);
            }
        }

        public void StopAI()
        {
            isTraining = false;
            isAIPlaying = false;
        }

        public void Update()
        {
            if (isTraining)
            {
                UpdateTraining();
            }
            else if (isAIPlaying)
            {
                UpdateAIPlay();
            }
        }

        private void UpdateTraining()
        {
            // Check if game is over or level is completed
            if (game.IsGameOver || game.Asteroids.Count == 0)
            {
                // Get final game state and record
                float[] finalState = game.GetGameState();
                float reward = game.IsGameOver ? -10 : 10; // Negative reward for dying, positive for clearing level
                
                // Store example with weighted reward
                foreach (var example in trainingData)
                {
                    example.Reward = reward;
                }
                
                // Start a new game
                game.Reset();
                gameCount++;
                
                // Adjust exploration rate
                explorationRate = Math.Max(0.1f, explorationRate * 0.99f);
                
                // Save model and reset training data periodically
                if (gameCount % 100 == 0)
                {
                    CurrentEpisode++;
                    TrainAndSaveModel();
                    trainingData.Clear();
                }
                
                if (gameCount >= TrainingEpisodes)
                {
                    isTraining = false;
                    return;
                }
            }
            
            // Get current game state
            float[] state = game.GetGameState();
            
            // Choose action (exploration vs exploitation)
            int action;
            if (random.NextDouble() < explorationRate || model == null || predictionEngine == null)
            {
                // Explore: choose random action
                action = random.Next(5);
            }
            else
            {
                // Exploit: use model to predict best action
                var gameState = new GameState { Features = state };
                var prediction = predictionEngine.Predict(gameState);
                action = (int)prediction.PredictedAction;
            }
            
            // Take action
            game.PerformAction(action);
            
            // Calculate immediate reward
            float immediateReward = CalculateReward();
            
            // Store training example
            if (trainingData.Count < MaxTrainingExamples)
            {
                trainingData.Add(new TrainingExample
                {
                    State = state,
                    Action = action,
                    Reward = immediateReward
                });
            }
            
            // Update the game
            game.Update();
        }

        private void UpdateAIPlay()
        {
            if (game.IsGameOver)
            {
                game.Reset();
                return;
            }
            
            // Get current game state
            float[] state = game.GetGameState();
            
            // Use model to predict action
            var gameState = new GameState { Features = state };
            var prediction = predictionEngine.Predict(gameState);
            int action = (int)prediction.PredictedAction;
            
            // Take action
            game.PerformAction(action);
            
            // Update the game
            game.Update();
        }

        private float CalculateReward()
        {
            float reward = 0;
            
            // Reward for shooting asteroids
            // This will be handled by TrainAndSaveModel since we don't know immediately when points are scored
            
            // Small penalty for shooting (to discourage constant shooting)
            foreach (var bullet in game.Bullets)
            {
                reward -= 0.01f;
            }
            
            // Check if ship is in danger (close to asteroids)
            foreach (var asteroid in game.Asteroids)
            {
                float distance = (float)Math.Sqrt(
                    Math.Pow(game.Ship.Position.X - asteroid.Position.X, 2) +
                    Math.Pow(game.Ship.Position.Y - asteroid.Position.Y, 2)
                );
                
                // Penalize being close to asteroids
                if (distance < 50 + asteroid.Radius)
                {
                    reward -= (50 + asteroid.Radius - distance) * 0.01f;
                }
            }
            
            return reward;
        }

        private void TrainAndSaveModel()
        {
            if (trainingData.Count == 0)
                return;
                
            Console.WriteLine($"Training model with {trainingData.Count} examples");
            
            // Create training data view
            var data = mlContext.Data.LoadFromEnumerable(
                trainingData.Select(e => new GameState
                {
                    Features = e.State,
                    Label = (uint)e.Action,
                    Weight = e.Reward
                })
            );
            
            // Define pipeline
            var pipeline = mlContext.Transforms.Concatenate("Features", "Features")
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.MulticlassClassification.Trainers.SdcaNonCalibrated(
                    labelColumnName: "Label",
                    featureColumnName: "Features",
                    exampleWeightColumnName: "Weight"));
            
            // Train model
            model = pipeline.Fit(data);
            
            // Create prediction engine for future predictions - initialize here
            predictionEngine = mlContext.Model.CreatePredictionEngine<GameState, ActionPrediction>(model);
            
            // Save model
            mlContext.Model.Save(model, data.Schema, ModelPath);
            
            Console.WriteLine("Model trained and saved successfully");
        }
    }

    // Classes for ML.NET
    public class GameState
    {
        [VectorType(21)]
        public float[] Features { get; set; }
        
        public float Weight { get; set; }
        
        public uint Label { get; set; }
    }

    public class ActionPrediction
    {
        [ColumnName("PredictedLabel")]
        public uint PredictedAction { get; set; }
    }

    // Training data storage
    public class TrainingExample
    {
        public float[] State { get; set; }
        public int Action { get; set; }
        public float Reward { get; set; }
    }
}
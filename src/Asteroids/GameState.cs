using Microsoft.ML;
using Microsoft.ML.Data;

 namespace Asteroids
{
    //gs
    public class GameState
    {
        [VectorType(21)] public float[] Features { get; set; } = new float[21];
        public float Label { get; set; }
        public float Weight { get; set; }
    }

    public class GameStatePrediction
    {
        [ColumnName("PredictedLabel")]
        public float PredictedLabel { get; set; }
        
        [ColumnName("Score")]
        public float[] Score { get; set; }
    }

    //public class ActionPrediction
    //{
    //    [ColumnName("PredictedLabel")]
    //    public uint PredictedLabel { get; set; }
    //}

    public class ModelTrainer
    {
        private static readonly string ModelPath = "model.zip";
        private readonly MLContext mlContext;
        private PredictionEngine<GameState, GameStatePrediction> predictionEngine;

        public ModelTrainer()
        {
            mlContext = new MLContext();
        }

        //private List<GameState> GetSampleGameStates()
        //{
        //    var random = new Random(0);
        //    var gameStates = new List<GameState>();
            
        //    // Generate 100 sample GameState objects with random features
        //    for (int i = 0; i < 100; i++)
        //    {
        //        var features = Enumerable.Range(0, 21)
        //            .Select(x => (float)random.NextDouble())
        //            .ToArray();
                
        //        var gameState = new GameState
        //        {
        //            Features = features,
        //            // Assign a class from 0 to 4 (5 classes total)
        //            Label = (uint)random.Next(0, 5),
        //            Weight = 1.0f
        //        };
                
        //        gameStates.Add(gameState);
        //    }
            
        //    return gameStates;
        //}

        //private List<GameState> GetSampleGameStates2(List<GameState> trainGameStates)
        //{

        
        //    //var random = new Random(0);
        //    var gameStates = new List<GameState>();
            
        //    // Generate 100 sample GameState objects with random features
        //    for (int i = 0; i < trainGameStates.Count ; i++)
        //    {
                
        //        if(trainGameStates[i].Weight != -10)
        //        {
        //            Console.WriteLine($"{trainGameStates[i].Weight}");
        //        }
        //        var gameState = new GameState
        //        {
        //            Features = trainGameStates[i].Features,
        //            // Assign a class from 0 to 4 (5 classes total)
        //            Label = trainGameStates[i].Label,
        //            Weight = trainGameStates[i].Weight
        //        };
                
        //        gameStates.Add(gameState);
        //    }
            
        //    return gameStates;
        //}

        public void TrainAndSaveModel(List<GameState> trainingData, string modelFilename)
        {
            if (trainingData.Count == 0)
                return;

            Console.WriteLine($@"Training model with {trainingData.Count} examples");

            //var gameStateList = GetSampleGameStates2(trainingData); 

            var data = mlContext.Data.LoadFromEnumerable<GameState>(trainingData);

            var dataSplit = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

            // 2. Define your pipeline for multiclass classification
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "KeyLabel", inputColumnName: "Label")
                .Append(mlContext.Transforms.Concatenate("Features", "Features")) // Features are already packed
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(
                    labelColumnName: "KeyLabel",
                    featureColumnName: "Features",
                    exampleWeightColumnName: "Weight")) // Using the Weight property
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            // 3. Train the model
            Console.WriteLine("Training the model...");
            //var model = pipeline.Fit(dataSplit.TrainSet);
            var model = pipeline.Fit(data);
            mlContext.Model.Save(model, dataSplit.TestSet.Schema, Path.Combine(Environment.CurrentDirectory, modelFilename));

            
            // do this later
            //predictionEngine = mlContext.Model.CreatePredictionEngine<GameState, GameStatePrediction>(model);

            

            Console.WriteLine(@"Model trained and saved successfully");
        }
    }

}

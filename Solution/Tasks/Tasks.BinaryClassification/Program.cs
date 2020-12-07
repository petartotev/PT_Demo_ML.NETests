 namespace DemoMLNetSentimentAnalysisConsoleApp
{
    using Microsoft.ML;
    using Microsoft.ML.Data;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using static Microsoft.ML.DataOperationsCatalog;

    public class Program
    {
        // Added above the Main method - to create a field that holds the recently downloaded dataset file path:
        static readonly string dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "FlagmanBgCommentsData.txt");

        static void Main()
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;

            // Create context.
            // MLContext is the starting point for all ML.NET operations. Conceptually similar to DBContext in Entity Framework Core.
            // Initializing MLContext creates a new ML.NET environment that can be shared across the model creation workflow objects.            
            MLContext mlContext = new MLContext(1234); // 1234 is seed.

            // Load data.
            TrainTestData splitDataView = LoadData(mlContext);

            // Train model.
            ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);

            // Evaluate model.
            Evaluate(mlContext, model, splitDataView.TestSet);

            // Use model.
            UseModelWithSingleItem(mlContext, model);
            UseModelWithBatchItems(mlContext, model);
        }

        public static TrainTestData LoadData(MLContext mlContext)
        {
            // Data in ML.NET is represented as an IDataView interface - a flexible and efficient way of describing tabular data (numeric / text). 
            // Data can be loaded from a text file or in real time (for example, SQL database or log files) to an IDataView object.
            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(dataPath, hasHeader: false);

            TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2); //default is 0.1 (10% of all lines will be used for testing).
            return splitDataView;
        }

        public static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        {
            // Extract and transform the data. Add a learning algorithm.
            // This is appended to the estimator and accepts the featurized SentimentText (Features) and the Label input parameters to learn from the historic data.
            var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
            .Append(mlContext.BinaryClassification.Trainers.FastTree(labelColumnName: "Label", featureColumnName: "Features"));
            
            Console.WriteLine("========== Create and Train the Model ==========");
            // The Fit() method trains your model by transforming the dataset and applying the training.
            var model = estimator.Fit(splitTrainSet);
            Console.WriteLine("========== End of training ==========\n");

            return model;
        }

        public static void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        {
            Console.WriteLine("========== Evaluating Model accuracy with Test data ==========");

            // Transform the splitTestSet data by adding the following code to Evaluate():
            IDataView predictions = model.Transform(splitTestSet);

            // Evaluate the model by adding the following as the next line of code in the Evaluate() method:
            // Once you have the prediction set, Evaluate() assesses the model, which compares the predicted values with the actual Labels in the test dataset 
            // It returns a CalibratedBinaryClassificationMetrics object on how the model is performing.
            CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");

            Console.WriteLine("Model quality metrics evaluation:");
            Console.WriteLine($"- Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"- Auc: {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"- F1Score: {metrics.F1Score:P2}");
            Console.WriteLine("========== End of model evaluation ==========");
        }

        private static void UseModelWithSingleItem(MLContext mlContext, ITransformer model)
        {
            PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);

            SentimentData sampleStatement = new SentimentData
            {
                SentimentText = "Това беше ужасна пържола и повече няма да ви стъпя в ресторанта!"
            };

            var resultPrediction = predictionFunction.Predict(sampleStatement);

            Console.WriteLine("\n========== Prediction Test of model with a single sample and test dataset ==========");
            Console.WriteLine($"\nSentiment: {resultPrediction.SentimentText} | Prediction: {(Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative")} | Probability: {resultPrediction.Score} ");
            Console.WriteLine("========== End of Predictions ==========\n");
        }

        public static void UseModelWithBatchItems(MLContext mlContext, ITransformer model)
        {
            IEnumerable<SentimentData> sentiments = new[]
            {
                new SentimentData { SentimentText = "Гларусите са враг за бургаския гражданин. Аман от тази летяща напаст!" },
                new SentimentData { SentimentText = "Бих застрелял всеки гларус - имам си пушка. Да мрат, яко да мрат гадните птици - вредители. Нападат баници, нападат пици, детето гладно, жената и то." },
                new SentimentData { SentimentText = "Генерал Мутафчийски със строгите мерки, които наложи, спаси Мама България и много хиляди човешки животи!" },
                new SentimentData { SentimentText = "Морската градина стана прекрасна, с всички тези зелени площи, поддържани градини, пейки и детски площадки! Чудесно място за цялото семейство!" },
                new SentimentData { SentimentText = "Аз съм оптимист за светлото, успешно бъдеще на България! Само след 10 години тук ще бъде земен рай - барове, алкохол, купон и красиви момичета!" },
                new SentimentData { SentimentText = "COVID 19 не е за подценяване! Носете си маските, съобразявайте се с дистанцията, мислете с главите си! Вижте какво става по света - хиляди умрели!" },
                new SentimentData { SentimentText = "Абе вече няколко месеца се говори за тоя Ковид пък аз още лично не познавам нито 1 заболял!" },
                new SentimentData { SentimentText = "COVID 19 COVID-19 КОВИД" },
                new SentimentData { SentimentText = "Бойко Борисов" },
                new SentimentData { SentimentText = "Валери Симеонов" },
                new SentimentData { SentimentText = "Генерал Мутафчийски" },
                new SentimentData { SentimentText = "Владимир Путин" },
                new SentimentData { SentimentText = "Доналд Тръмп" },
                new SentimentData { SentimentText = "Григор Димитров" },
                new SentimentData { SentimentText = "Tova e malko shliokavica za tova niama da se opravi tazi darjava!!!" },
                new SentimentData { SentimentText = "Niamam kirilica na telefona si." }
            };

            IDataView batchComments = mlContext.Data.LoadFromEnumerable(sentiments);

            IDataView predictions = model.Transform(batchComments);

            // Use model to predict whether comment data is Positive (1) or Negative (0).
            IEnumerable<SentimentPrediction> predictedResults = mlContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);

            Console.WriteLine("\n========== Prediction Test of loaded model with multiple samples ==========");
            foreach (SentimentPrediction prediction in predictedResults)
            {
                Console.WriteLine($"Sentiment: {prediction.SentimentText} | Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative")} | Probability: {prediction.Score} ");
            }
            Console.WriteLine("========== End of predictions ==========");
        }
    }
}
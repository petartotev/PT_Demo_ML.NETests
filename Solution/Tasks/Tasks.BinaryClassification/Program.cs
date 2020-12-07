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
        // Add the following code to the line right above the Main method, to create a field to hold the recently downloaded dataset file path:
        static readonly string _dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "FlagmanBgCommentsData.txt");

        static void Main(string[] args)
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;

            // The MLContext class is a starting point for all ML.NET operations.
            // Initializing mlContext creates a new ML.NET environment that can be shared across the model creation workflow objects.
            // It's similar, conceptually, to DBContext in Entity Framework (Core).
            MLContext mlContext = new MLContext(1234);

            TrainTestData splitDataView = LoadData(mlContext);

            ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);

            Evaluate(mlContext, model, splitDataView.TestSet);

            UseModelWithSingleItem(mlContext, model);

            UseModelWithBatchItems(mlContext, model);
        }

        public static TrainTestData LoadData(MLContext mlContext)
        {
            // Data in ML.NET is represented as an IDataView class. 
            // IDataView is a flexible, efficient way of describing tabular data (numeric and text). 
            // Data can be loaded from a text file or in real time (for example, SQL database or log files) to an IDataView object.

            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, hasHeader: false);

            TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2); //default is 0.1

            return splitDataView;
        }

        public static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        {
            var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText)) // Extract and transform the data
                .Append(mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(labelColumnName: "Label", featureColumnName: "Features")); // Add a learning algorithm
            // LbfgsLogisticRegression
            // The SdcaLogisticRegressionBinaryTrainer is your classification training algorithm. 
            // This is appended to the estimator and accepts the featurized SentimentText (Features) and the Label input parameters to learn from the historic data.

            Console.WriteLine("=============== Create and Train the Model ===============");
            var model = estimator.Fit(splitTrainSet); // The Fit() method trains your model by transforming the dataset and applying the training.
            Console.WriteLine("=============== End of training ===============");
            Console.WriteLine();

            return model; //Return the model trained to use for evaluation
        }

        public static void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        {
            Console.WriteLine("=============== Evaluating Model accuracy with Test data===============");

            IDataView predictions = model.Transform(splitTestSet);
            // Transform the splitTestSet data by adding the following code to Evaluate():

            CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
            // Evaluate the model by adding the following as the next line of code in the Evaluate() method:

            Console.WriteLine();
            Console.WriteLine("Model quality metrics evaluation");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            Console.WriteLine("=============== End of model evaluation ===============");
            // Once you have the prediction set (predictions), the Evaluate() method assesses the model, which compares the predicted values 
            // with the actual Labels in the test dataset and returns a CalibratedBinaryClassificationMetrics object on how the model is performing.
        }

        private static void UseModelWithSingleItem(MLContext mlContext, ITransformer model)
        {
            PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);

            SentimentData sampleStatement = new SentimentData
            {
                SentimentText = "Това беше ужасна пържола и повече няма да ви стъпя в ресторанта!"
            };

            var resultPrediction = predictionFunction.Predict(sampleStatement);

            Console.WriteLine();
            Console.WriteLine("=============== Prediction Test of model with a single sample and test dataset ===============");

            Console.WriteLine();
            Console.WriteLine($"Sentiment: {resultPrediction.SentimentText} | Prediction: {(Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative")} | Probability: {resultPrediction.Probability} ");

            Console.WriteLine("=============== End of Predictions ===============");
            Console.WriteLine();
        }

        public static void UseModelWithBatchItems(MLContext mlContext, ITransformer model)
        {
            IEnumerable<SentimentData> sentiments = new[]
            {
                new SentimentData // 1
                {
                    SentimentText = "Гларусите са враг за бургаския гражданин. Аман от тази летяща напаст!"
                },
                new SentimentData // 2
                {
                    SentimentText = "Бих застрелял всеки гларус - имам си пушка. Да мрат, яко да мрат гадните птици - вредители. Нападат баници, нападат пици, детето гладно, жената и то."
                },
                new SentimentData // 3
                {
                    SentimentText = "Генерал Мутафчийски със строгите мерки, които наложи, спаси Мама България и много хиляди човешки животи!"
                },
                new SentimentData // 4
                {
                    SentimentText = "Морската градина стана прекрасна, с всички тези зелени площи, поддържани градини, пейки и детски площадки! Чудесно място за цялото семейство!"
                },
                new SentimentData // 5
                {
                    SentimentText = "Аз съм оптимист за светлото, успешно бъдеще на България! Само след 10 години тук ще бъде земен рай - барове, алкохол, купон и красиви момичета!"
                },
                new SentimentData // 6
                {
                    SentimentText = "COVID 19 не е за подценяване! Носете си маските, съобразявайте се с дистанцията, мислете с главите си! Вижте какво става по света - хиляди умрели!"
                },
                new SentimentData // 7
                {
                    SentimentText = "Абе вече няколко месеца се говори за тоя Ковид пък аз още лично не познавам нито 1 заболял!"
                },
                new SentimentData // 8
                {
                    SentimentText = "COVID 19 COVID-19 КОВИД"
                },
                new SentimentData // 9
                {
                    SentimentText = "Бойко Борисов"
                },
                new SentimentData // 10
                {
                    SentimentText = "Валери Симеонов"
                },
                new SentimentData // 11
                {
                    SentimentText = "Генерал Мутафчийски"
                },
                new SentimentData // 12
                {
                    SentimentText = "Владимир Путин"
                },
                new SentimentData // 13
                {
                    SentimentText = "Доналд Тръмп"
                },
                new SentimentData // 14
                {
                    SentimentText = "Григор Димитров"
                },
                new SentimentData // 15
                {
                    SentimentText = "Tova e malko shliokavica za tova niama da se opravi tazi darjava!!!"
                },
                new SentimentData // 16
                {
                    SentimentText = "Niamam kirilica na telefona si."
                }
            };

            IDataView batchComments = mlContext.Data.LoadFromEnumerable(sentiments);

            IDataView predictions = model.Transform(batchComments);

            // Use model to predict whether comment data is Positive (1) or Negative (0).
            IEnumerable<SentimentPrediction> predictedResults = mlContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);

            Console.WriteLine();

            Console.WriteLine("=============== Prediction Test of loaded model with multiple samples ===============");

            foreach (SentimentPrediction prediction in predictedResults)
            {
                Console.WriteLine($"Sentiment: {prediction.SentimentText} | Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative")} | Probability: {prediction.Probability} ");
            }
            Console.WriteLine("=============== End of predictions ===============");
        }
    }
}

// https://docs.microsoft.com/en-us/dotnet/machine-learning/tutorials/sentiment-analysis

// CREATE A CONSOLE APPLICATION
// Create Data folder.

// Install NuGet package Microsoft.ML.
// Install NuGet package Microsoft.ML.FastTree.

// Download UCI Sentiment Labeled Sentences dataset ZIP file => unzip.
// Copy the yelp_labelled.txt file into Data directory.
// yelp_labelled.txt => Properties => Advanced => Copy to Output Directory -> Copy if newer
// Create classes, define paths...
// Add additional using statements.
// Add the following code to the line right above the Main method, to create a field to hold the recently downloaded dataset file path (_dataPath = )
// Create new class SentimentData.cs
// Add 2 classes inside the file - SentimentData (Input) and SentimentPrediction (Output)

// Data in ML.NET is represented as an IDataView class. IDataView is a flexible, efficient way of describing tabular data (numeric and text). 
// Data can be loaded from a text file or in real time (for example, SQL database or log files) to an IDataView object.

// Program.cs => MLContext mlContext = new MLContext();
// Program.cs => TrainTestData splitDataView = LoadData(mlContext);
// Program.cs => add new method LoadData()

// BUILD AND TRAIN THE MODEL

// STEPS

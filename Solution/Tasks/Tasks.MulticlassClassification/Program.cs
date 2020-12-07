namespace DemoMLNetMultiClassificationConsoleApp
{
    using Microsoft.ML;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var fileTrain = "../../../Data/storytel-train-data.csv";
            var fileModel = "../../../Models/StorytelCategoryModel.zip";

            var fileTest = "../../../Data/storytel-test-data.txt";
            var testData = File.ReadAllText(fileTest).Split("~.~").ToList();

            var context = new MLContext(seed: 1234);

            TrainModel(fileTrain, fileModel, context);

            UseModel(fileModel, testData, context);
        }

        static void TrainModel(string fileTrain, string fileModel, MLContext context)
        {
            Console.WriteLine($"Loading the data file: {fileTrain}...");
            var dataView = context.Data.LoadFromTextFile<BookData>(fileTrain, ',', false, true, true);

            Console.WriteLine("Map raw input data columns to ML.NET data...");
            var dataProcessPipeline = context.Transforms.Conversion.MapValueToKey("Label", nameof(BookData.Category))
                .Append(context.Transforms.Text.FeaturizeText("Features", nameof(BookData.Summary)));

            Console.WriteLine("Create and configure the selected training algorithm...");
            // Algorithms tested: SDCA = Stochastic Dual Coordinate Ascent, LightGbm (GBM = Gradient Boosting Machine)
            var trainer = context.MulticlassClassification.Trainers.LbfgsMaximumEntropy();

            var trainingPipeline = dataProcessPipeline
                .Append(trainer)
                .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            Console.WriteLine("Train the model fitting to the DataSet...");
            var trainedModel = trainingPipeline.Fit(dataView);

            Console.WriteLine($"Save the model to a file ({fileModel})...");
            context.Model.Save(trainedModel, dataView.Schema, fileModel);
        }

        static void UseModel(string fileModel, List<string> testData, MLContext context)
        {
            var model = context.Model.Load(fileModel, out _);
            var predictionEngine = context.Model.CreatePredictionEngine<BookData, BookPrediction>(model);

            foreach (var test in testData)
            {
                var prediction = predictionEngine.Predict(new BookData { Summary = test });
                Console.WriteLine(new string('-', 50));
                Console.WriteLine($"Content: {test}");
                Console.WriteLine($"Prediction: {prediction.Category}");
                Console.WriteLine($"Score: {prediction.Score.Max()}");
            }
        }
    }
}

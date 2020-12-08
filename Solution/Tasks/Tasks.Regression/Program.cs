namespace DemoMLNetRegressionConsoleApp
{
    using DemoMLNet.Tasks.Regression;
    using Microsoft.ML;
    using Microsoft.ML.Trainers.LightGbm;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public static class Program
    {
        public static void Main()
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;

            var modelFile = "../../../Model/CarsSaleModel.zip";

            if (!File.Exists(modelFile))
            {
                TrainModel("../../../Data/CarsCars.csv", modelFile);
            }

            var testData = new List<CarsData>
            {
                new CarsData
                {
                    Model = "Passat",
                    Year = 2019,
                    Transmission = "Manual",
                    Mileage = 5000,
                    FuelType = "Petrol",
                    Tax = 30,
                    Mpg = 60.1f,
                    EngineSize = 1.6f
                },
                new CarsData
                {
                    Model = "Passat",
                    Year = 2017,
                    Transmission = "Manual",
                    Mileage = 5000,
                    FuelType = "Petrol",
                    Tax = 30,
                    Mpg = 60.1f,
                    EngineSize = 2.0f
                },
                new CarsData
                {
                    Model = "Passat",
                    Year = 2017,
                    Transmission = "Manual",
                    Mileage = 5000,
                    FuelType = "Petrol",
                    Tax = 30,
                    Mpg = 60.1f,
                    EngineSize = 1.6f
                },
            };

            TestModel(modelFile, testData);
        }

        private static void TrainModel(string dataFile, string modelFile)
        {
            var context = new MLContext();

            var trainingDataView = context.Data.LoadFromTextFile<CarsData>(
                dataFile,
                hasHeader: true,
                separatorChar: ',',
                allowQuoting: true);

            var dataProcessPipeline = context.Transforms.Categorical
                .OneHotEncoding(
                    new[]
                    {
                        new InputOutputColumnPair(nameof(CarsData.Model), nameof(CarsData.Model)),
                        new InputOutputColumnPair(nameof(CarsData.FuelType), nameof(CarsData.FuelType)),
                        new InputOutputColumnPair(nameof(CarsData.Transmission), nameof(CarsData.Transmission)),
                    }).Append(
                    context.Transforms.Concatenate(
                        outputColumnName: "Features",
                        nameof(CarsData.Year),
                        nameof(CarsData.Mileage),
                        nameof(CarsData.Tax),
                        nameof(CarsData.Mpg),
                        nameof(CarsData.EngineSize)));

            var trainer = context.Regression.Trainers.LightGbm(
                new LightGbmRegressionTrainer.Options // LightGbm comes from 'Gradient Boosting Machine'.
                {
                    NumberOfIterations = 4000,
                    LearningRate = 0.1006953f,
                    NumberOfLeaves = 55,
                    MinimumExampleCountPerLeaf = 20,
                    UseCategoricalSplit = true,
                    HandleMissingValue = false,
                    MinimumExampleCountPerGroup = 200,
                    MaximumCategoricalSplitPointCount = 16,
                    CategoricalSmoothing = 10,
                    L2CategoricalRegularization = 1,
                    Booster = new GradientBooster.Options { L2Regularization = 0.5, L1Regularization = 0 },
                    LabelColumnName = nameof(CarsData.Price),
                    FeatureColumnName = "Features",
                });
            var trainingPipeline = dataProcessPipeline.Append(trainer);

            ITransformer model = trainingPipeline.Fit(trainingDataView);
            context.Model.Save(model, trainingDataView.Schema, modelFile);
        }

        private static void TestModel(string modelFile, IEnumerable<CarsData> testModelData)
        {
            var context = new MLContext(666);

            var model = context.Model.Load(modelFile, out _);

            var predictionEngine = context.Model.CreatePredictionEngine<CarsData, CarsResult>(model);

            foreach (var testData in testModelData)
            {
                var prediction = predictionEngine.Predict(testData);
                Console.WriteLine($"\nModel: {testData.Model}");
                Console.WriteLine($"Year: {testData.Year}");
                Console.WriteLine($"Transmission: {testData.Transmission}");
                Console.WriteLine($"Mileage: {testData.Mileage}");
                Console.WriteLine($"FuelType: {testData.FuelType}");
                Console.WriteLine($"Tax: {testData.Tax}");
                Console.WriteLine($"Mpg: {testData.Mpg}");
                Console.WriteLine($"EngineSize: {testData.EngineSize}");
                PrintSeparationLine(27);
                Console.WriteLine($"Price Prediction: {prediction.Score}");
            }
        }

        private static void PrintSeparationLine(int count)
        {
            Console.WriteLine(new string('_', count));
        }
    }
}

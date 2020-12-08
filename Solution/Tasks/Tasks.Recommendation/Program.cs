namespace DemoMLNet.Tasks.Recommendation
{
    using Microsoft.ML;
    using Microsoft.ML.Trainers;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Program
    {
        public static void Main()
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;

            var context = new MLContext();

            var modelFile = "../../../Model/UsersBooksModel.zip";

            TrainModel(context, "../../../Data/ratings.csv", modelFile);

            var testModelData = new List<UserBook>
                             {
                                 new UserBook { UserId = 16795, BookId = "0446610038" }, // 1st to Die: A Novel
                                 new UserBook { UserId = 16795, BookId = "0316666009" }, // 1st to Die: A Novel
                                 new UserBook { UserId = 16795, BookId = "0151660387" }, // 1984
                                 new UserBook { UserId = 16795, BookId = "0451524934" }, // 1984
                                 new UserBook { UserId = 16795, BookId = "0452262933" }, // 1984
                                 new UserBook { UserId = 16795, BookId = "0451519841" }, // 1984
                                 new UserBook { UserId = 16795, BookId = "038039586X"}, // Watership Down
                                 new UserBook { UserId = 16795, BookId = "0380002930"}, // Watership Down
                                 new UserBook { UserId = 16795, BookId = "0446611212"}, // Violets Are Blue
                                 new UserBook { UserId = 16795, BookId = "1586211978"}, // Violets Are Blue
                                 new UserBook { UserId = 16795, BookId = "0316693235"}, // Violets Are Blue
                                 new UserBook { UserId = 16795, BookId = "0843950293"}, // Violets Are Blue
                                 new UserBook { UserId = 16795, BookId = "0345417623"}, // Timeline
                                 new UserBook { UserId = 16795, BookId = "061333633X"}, // Timeline
                                 new UserBook { UserId = 16795, BookId = "0679444815"}, // Timeline
                                 new UserBook { UserId = 16795, BookId = "0374386137"}, // A Wrinkle In Time
                                 new UserBook { UserId = 16795, BookId = "0440227151"}, // A Wrinkle In Time
                                 new UserBook { UserId = 16795, BookId = "0440800544"}, // A Wrinkle In Time
                                 new UserBook { UserId = 16795, BookId = "0440998050"}, // A Wrinkle In Time
                                 new UserBook { UserId = 16795, BookId = "0440498058"}, // A Wrinkle In Time
                                 new UserBook { UserId = 16795, BookId = "0446310786"}, // To Kill a Mockingbird
                                 new UserBook { UserId = 16795, BookId = "0446310492"}, // To Kill a Mockingbird
                                 new UserBook { UserId = 16795, BookId = "0060935464"}, // To Kill a Mockingbird
                             };

            TestModel(context, modelFile, testModelData);
        }

        private static void TrainModel(MLContext context, string inputFile, string modelFile)
        {
            // Load data
            IDataView trainingDataView = context.Data.LoadFromTextFile<UserBook>(
                inputFile,
                hasHeader: true,
                separatorChar: ',');

            // Build & train model
            IEstimator<ITransformer> estimator = context.Transforms.Conversion
                .MapValueToKey(outputColumnName: "userIdEncoded", inputColumnName: nameof(UserBook.UserId)).Append(
                    context.Transforms.Conversion.MapValueToKey(outputColumnName: "bookIdEncoded", inputColumnName: nameof(UserBook.BookId)));

            var options = new MatrixFactorizationTrainer.Options
            {
                LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass,
                MatrixColumnIndexColumnName = "userIdEncoded",
                MatrixRowIndexColumnName = "bookIdEncoded",
                LabelColumnName = nameof(UserBook.Label),
                Alpha = 0.1,
                Lambda = 0.5,
                NumberOfIterations = 1000,
            };

            var trainerEstimator = estimator.Append(context.Recommendation().Trainers.MatrixFactorization(options));
            ITransformer model = trainerEstimator.Fit(trainingDataView);

            // Save model
            context.Model.Save(model, trainingDataView.Schema, modelFile);
        }

        private static void TestModel(MLContext context, string modelFile, IEnumerable<UserBook> testModelData)
        {
            var model = context.Model.Load(modelFile, out _);

            var predictionEngine = context.Model.CreatePredictionEngine<UserBook, UserBookScore>(model);

            foreach (var testInput in testModelData)
            {
                var prediction = predictionEngine.Predict(testInput);
                Console.WriteLine($"User: {testInput.UserId}, Course: {testInput.BookId}, Score: {prediction.Score}");
            }
        }
    }
}

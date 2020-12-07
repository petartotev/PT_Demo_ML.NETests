namespace DemoMLNetObjectDetectionConsoleApp
{
    using DemoMLNetObjectDetectionConsoleApp.DataStructures;
    using DemoMLNetObjectDetectionConsoleApp.YoloParser;
    using Microsoft.ML;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class Program
    {
        public static void Main()
        {
            var assetsRelativePath = @"../../../assets";
            string assetsPath = ProgramService.GetAbsolutePath(assetsRelativePath);
            var modelFilePath = Path.Combine(assetsPath, "Model", "TinyYolo2_model.onnx");
            var imagesFolder = Path.Combine(assetsPath, "images");
            var outputFolder = Path.Combine(assetsPath, "images", "output");

            MLContext mlContext = new MLContext();

            try
            {
                IEnumerable<ImageNetData> images = ImageNetData.ReadFromFile(imagesFolder);
                IDataView imageDataView = mlContext.Data.LoadFromEnumerable(images);

                var modelScorer = new OnnxModelScorer(imagesFolder, modelFilePath, mlContext);

                // Use model to score data
                IEnumerable<float[]> probabilities = modelScorer.Score(imageDataView);

                YoloOutputParser parser = new YoloOutputParser();

                var boundingBoxes =
                    probabilities
                    .Select(probability => parser.ParseOutputs(probability))
                    .Select(boxes => parser.FilterBoundingBoxes(boxes, 5, .5F));

                for (var i = 0; i < images.Count(); i++)
                {
                    string imageFileName = images.ElementAt(i).Label;
                    IList<YoloBoundingBox> detectedObjects = boundingBoxes.ElementAt(i);

                    ProgramService.DrawBoundingBox(imagesFolder, outputFolder, imageFileName, detectedObjects);

                    ProgramService.LogDetectedObjects(imageFileName, detectedObjects);

                    Console.WriteLine("========= End of Process..Hit any Key ========");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadLine();
        }
    }
}
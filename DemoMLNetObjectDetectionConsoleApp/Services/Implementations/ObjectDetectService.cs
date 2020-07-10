namespace DemoMLNetObjectDetectionConsoleApp.Services.Implementations
{  
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using DemoMLNetObjectDetectionConsoleApp.DataStructures;
    using DemoMLNetObjectDetectionConsoleApp.YoloParser;

    using Microsoft.ML;

    public class ObjectDetectService : IObjectDetectService
    {
        public async void CopyImageFromLocalToWebDirectory(string guidName)
        {         
            var sourceFileName = @"C:\Users\USER\source\repos\DemoMLNet\DemoMLNetObjectDetectionConsoleApp\assets\images\output\" + guidName + ".jpg";
            var destinationFileName = @"C:\Users\USER\source\repos\DemoMLNet\DemoMLNetObjectDetectionWebApp\wwwroot\" + guidName + ".jpg";

            await Task.Run(() => File.Copy(sourceFileName, destinationFileName, true));
        }

        public List<string> GenerateAnalysis(string guidName)
        {
            List<string> stringsToReturn = new List<string>();

            var assetsRelativePath = @"../../../../DemoMLNetObjectDetectionConsoleApp/assets";
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
                    if (images.ElementAt(i).Label == guidName + ".jpg")
                    {
                        string imageFileName = images.ElementAt(i).Label;
                        IList<YoloBoundingBox> detectedObjects = boundingBoxes.ElementAt(i);

                        ProgramService.DrawBoundingBox(imagesFolder, outputFolder, imageFileName, detectedObjects);

                        stringsToReturn = ProgramService.LogDetectedObjects(imageFileName, detectedObjects);
                    }
                    //Console.WriteLine("========= End of Process..Hit any Key ========");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return stringsToReturn;
        }
    }
}

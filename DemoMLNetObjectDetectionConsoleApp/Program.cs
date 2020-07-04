using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Microsoft.ML;
using DemoMLNetObjectDetectionConsoleApp.DataStructures;
using DemoMLNetObjectDetectionConsoleApp.YoloParser;

namespace DemoMLNetObjectDetectionConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

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

//CREATE A CONSOLE APPLICATION

//Create a console application;

//Install NuGet Package => Microsoft.ML (1.5.0);
//Install NuGet Package => Microsoft.ML.ImageAnalytics (1.5.0);
//Install NuGet Package => Microsoft.ML.OnnxTransformer (1.5.0);
//Install NuGet Package => Microsoft.ML.OnnxRuntime (1.3.0);

// PREPARE YOUR DATA AND PRE-TRAINED MODEL

//1. Download The project assets directory zip file => unzip;
//2. Copy the assets directory into your project directory (they contain the image files).
//3.1. Download the Tiny YOLOv2 model (from the ONNX Model Zoo)
//3.2. Go to CMD.EXE => tar -xvzf tiny_yolov2.tar.gz
//4. Copy the extracted model.onnx file from the directory to Project => assets\Model directory and rename it TinyYolo2_model.onnx

//5. Go to every single file in assets folder in Solution Explorer (MSVS2019), right-click => Properties => Advanced => Copy to Output Directory: Do not copy => Copy if newer

// CREATE CLASSES AND DEFINE PATHS

//Program.cs => set USINGs
//Program.cs => Add method: public static string GetAbsolutePath(string relativePath) {}
//Program.cs => Main() => create fields to store the location of assets (assetsRelativePath, assetsPath, modelFilePath, imagesFolder, outputFolder

//Add a new directory Add => New folder => DataStructures

//Create your input data class in DataStructures directory => Add => New Item => ImageNetData.cs
//Set USINGs
//Replace class (copy-paste)

//Create your prediction class in DataStructures directory => Add => New Item => ImageNetPrediction.cs
//Set USINGs
//Replace class (copy-paste)

// INITIALIZE VARIABLES IN MAIN

//Initialize with a new instance of MLContext in the Main() of Program.cs below the outputFolder field.
//MLContext mlContext = new MLContext();



// CREATE A PARSER TO POST-PROCESS MODEL OUTPUTS!

//Add => New Folder => YoloParser

//CREATE BOUNDING BOXES AND DIMENSIONS

//Right click on YoloParser directory => Add => New Item => DimensionsBase.cs
//Delete all USINGs!
//Replace class (copy-paste)

//Right click on YoloParser directory => Add => New Item => YoloBoundingBox.cs
//Set USINGs
//Add public class BoundingBoxDimensions : DimensionsBase { }
//Replace class (copy-paste)

//CREATE THE PARSER

//Add => New Item => YoloOutputParser.cs
//Set USINGs
//Add a nested class CellDimensions : DimensionsBase { }
//Add constants and fields
//Create a list of anchors below channelStride
//Add a list of labels below the anchors
//Add colors
//Add the code for all the helper methods below your list of classColors
//Create the method ParseOutputs() { }
//Inside it create a list to store bounding boxes (var boxes)
//Add for1st => for2nd => for3rd
//Inside for3rd add var channel
//Inside for3rd add BoundingBoxDimensions boundingBoxDimensions
//Inside for3rd add float confidence
//Inside for3rd add CellDimensions mappedBoundingBox
//Inside for3rd add if (confidence < threshold) => continue;
//Inside for3rd add float[] predictedClasses
//Inside for3rd add var (topResultIndex, topResultScore), var topScore
//Inside for3rd add if (topScore < threshold) => continue;
//Inside for3rd if the current bounding box exceeds the threshold => create new BoundingBox object and add it to the boxes list: boxes.Add(new YoloBoundingBox() { });
//Below the for1st add return boxes;

//FILTER OVERLAPPING BOXES

//Add method public IList<YoloBoundingBox> FilterBoundingBoxes() after ParseOutputs()

//USE THE MODEL FOR SCORING

//Right-click on project => Add => New Item => OnnxModelScorer.cs
//Set USINGs
//Add variables imagesFolder, modelLocation, mlContext, _boundingBoxes
//Add a constructor
//Add struct ImageNetSettings
//Add struct TinyYoloModelSettings
//Add method private ITransformer LoadModel(string modelLocation)
//Add method private IEnumerable<float[]> PredictDataUsingModel(IDataView testData, ITransformer model)
//Add method public IEnumerable<float[]> Score(IDataView data)

//DETECT OBJECTS
//Program.cs =>
//Set USINGs (using DemoMLNetObjectDetectionConsoleApp.DataStructures; using DemoMLNetObjectDetectionConsoleApp.YoloParser;)
//Main() => Add try-catch statement
//Inside try block => 

//VISUALIZE PREDICTIONS
//Program.cs => private static void DrawBoundingBox()
//Load the image and get the height and width dimensions
//Create a for-each loop to iterate over each of the bounding boxes detected by the model
//Inside the for-each loop get the dimensions of the bounding box (416 x 416)
//Scale the bounding box to match the actual size of the image
//Define a template for text that will appear above each bounding box
//In order to draw on the image, convert it to a Graphics object.
//Inside the using code block, tune the graphic's Graphics object settings.
//Set the font and color options for the text and bounding box.
//Create and fill a rectangle above the bounding box to contain the text using the FillRectangle method. This will help contrast the text and improve readability.
//Then, Draw the text and bounding box on the image using the DrawString() and DrawRectangle() methods.
//Outside of the for-each loop, add code to save the images in the outputDirectory.
//For additional feedback that the application is making predictions as expected at runtime, add a method called LogDetectedObjects()
//Back to Main() try block => add a for-loop to iterate over each of the scored images.
//get the name of the image file and the bounding boxes associated with it - string imageFileName
//DrawingBoundingBox();
//LogDetectedObjects();
//After the try-catch statement, add additional logic to indicate the process is done running. (Console.WriteLine(), Console.ReadLine());
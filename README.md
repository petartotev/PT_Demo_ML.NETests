# PT_Demo_ML.NETests

![Horse Object Detected](DemoMLNetObjectDetectionConsoleApp/assets/images/output/horse_output.jpg)

## Information:
This repository contains a Solution with a series of Demo Projects that experiment with the ML.NET Library. 

### Binary Classification / (Sentiment Analysis)

#### A pre-trained model predicts if a certain comment (text) that you give as an input will receive a positive or negative value.

##### Algorithm used: LbfgsLogisticRegression

1. The **DataGathererFlagmanBg** project is created to gather information from https://www.flagman.bg/ - a local media in Burgas, Bulgaria.

Foreach article it takes:
- all the positive comments with (upvote : downvote) ratio of 30 : 1 or more
- all the negative comments with (upvote : downvote) ratio of 1 : 15 or less.

Foreach comment that matches these conditions it File.Appends() a new line in the format: 

$"{commentText}\t{commentBinaryValue}"

2. The text file is split into two - 80% of it would be used for training the model and 20% - for testing it.

3. Then the .txt with all the comments (content, value) inside is used to train the ML model with the algorithm that comes from the Microsoft.ML library.

4. Now the model could be evaluated with those 20% separated in step 2.

5. Finally, you can give the pre-trained model some text and see what it predicts (positive (1) or negative (0)) and how sure it is about it (probability). 

### Multiclass Classification 

#### A pre-trained model predicts the probable category of a book/movie/joke - based on a summary (text) about it that you give as an input.

##### Algorithm used: SdcaMaximumEntropy

### Object Detection

#### Finds all the familiar objects in an image (person, dog, horse, bike, car etc.)

##### Algorithm used: OnnxModelScorer ( TinyYolo2_model.onnx )

## Credits:
Using those was a piece of cake thanks to:
- https://dotnet.microsoft.com/learn/ml-dotnet - the free tutorials about Machine Learning with ML.NET
- https://www.youtube.com/watch?v=dluB5VE1m1k&feature=emb_logo - a lecture of Nikolay Kostov (@NikolayIT) presented in Software University in 2020.

# PT_Demos_ML.NETests

![Horse Object Detected](DemoMLNetObjectDetectionConsoleApp/assets/images/output/dd3832f7-44cd-4466-b5fa-70bbe58ebd77.jpg)

Information:
The repository contains a Solution with a series of Projects that experiment with the ML.NET Library. 

- **Binary Classification** (Sentiment Analysis)

1. A DataGathererFlagmanBg project is created to gather information through the existing articles in the website of a local media in Burgas, Bulgaria.

For each article it takes:
- all the positive comments with (upvote : downvote) ratio of 30 : 1 or more
- all the negative comments with (upvote : downvote) ratio of 1 : 15 or less.

Foreach comment that matches the instructions above - it File.Appends a new line with the following format:
$"{commentText}\t{commentBinaryValue}

2. The text file is split into two - 80% of it would be used for training the model and 20% - for testing it.

3. The text file with all the comments (content, value) is then used to train a ML model, based on the following algorithm:
mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression

4. Now the model could be evaluated with those 20% separated in step 2.

5. Finally, you can give the pre-trained model some text and see what it predicts (positive (1) or negative (0)) and how sure it is about it (probability). 

- **Multiclass Classification** - guesses the category/genre of a book/movie/joke - based on the decision of a model that is pre-trained with a file in a format (category,summary/n...).

- **Object Detection** - finds all the familiar objects in an image (person, dog, horse, bike, car etc.)

Credits:
Using those was a piece of cake thanks to:
- the Microsoft Tutorials about ML.NET
- https://www.youtube.com/watch?v=dluB5VE1m1k&feature=emb_logo - a lecture of Nikolay Kostov (@NikolayIT) presented in Software University in 2020.

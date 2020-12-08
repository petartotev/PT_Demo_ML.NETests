# PT_Demo_ML.NETests

## General Information

This repository contains a solution with a number of .NET Core projects that experiment with the ML.NET Library.

_This is an educational project. That's why there are a lot of comments with explanations on what different sections of code do. They help me understand the steps of machine learning 'gather info' > 'build model' > 'train model' > 'test model' > 'evaluate model' > 'use model' better._

![Horse Object Detected](Resources/Screenshots/PT_Demo_ML.NETests_cover.jpg)

## Technologies

- AngleSharp
- Microsoft\.ML\.NET
- Microsoft\.ML\.LightGBM
- Microsoft\.ML\.Recommender

## Contents

The 'PT_Demo_ML.NETests' solution consists of 2 directories:

- Data
  - DemoMLNet.Data.Gatherer
- Tasks
  - BinaryClassification
    - DemoMLNet.Tasks.BinaryClassification
  - MulticlassClassification
    - DemoMLNet.Tasks.MulticlassClassification
  - ObjectDetection
    - DemoMLNet.Tasks.ObjectDetection\.Console
    - DemoMLNet.Tasks.ObjectDetection\.Web
  - Recommendation
    - DemoMLNet.Tasks.Recommendation
  - Regression
    - DemoMLNet.Tasks.Regression

---

### Data

---

### DemoMLNet.Data.Gatherer (Data Gatherer / web crawler)

DemoMLNet.Data.Gatherer is a .NET Core 3.1 Console Application.  
It is implemented to play the role of a **web crawler**.  
Once an instance of DataGatherer is instantiated it creates new HttpClient and AngleSharp HtmlParser objects.
They are used within a for-loop that goes through a number of pages with URLs that end with an article/category number.  
For example: https://www.flagman.bg/article/{articleId}  
The result of this crawling is a string. It is cleaned from all its html elements and is saved as a .txt file.  
The .txt files gathered would be used to train the ML models - 80% for training and 20% for evaluating the accuracy of the algorithm used.

---

### Tasks

---

### BinaryClassification

### DemoMLNet.Tasks.BinaryClassification / (Sentiment Analysis)

#### **Algorithms (Trainers) used:**

mlContext.BinaryClassification.Trainers

- .FastTree
  - Accuracy: 77.06%
  - Auc: 73.42%
  - F1Score: 85.96%
- .LbfgsLogisticRegression
  - Accuracy: 76.50%
  - Auc: 72.56%
  - F1Score: 85.77%
- .SdcaLogisticRegression
  - Accuracy: 75.27%
  - Auc: 71.86%
  - F1Score: 84.03%

DemoMLNet.Tasks.BinaryClassification is a .NET Core 3.1 Console Application.  
A pre-trained model predicts if a certain comment that you write as an input will receive a positive (1) or negative (0) reaction from the users that read it.

The target of the sentiment analysis is 'Flagman' - a local online media in Burgas, Bulgaria.  
Link: https://www.flagman.bg/

- all the positive comments with (upvote : downvote) ratio of 30 : 1 or more
- all the negative comments with (upvote : downvote) ratio of 1 : 15 or less.

Foreach comment that matches these conditions it File.Appends() a new line in the format:

$"{commentText}\t{commentBinaryValue}"

2. The text file is split into two - 80% of it would be used for training the model and 20% - for testing it.

3. Then the .txt with all the comments (content, value) inside is used to train the ML model with the algorithm that comes from the Microsoft.ML library.

4. Now the model could be evaluated with those 20% separated in step 2.

5. Finally, you can give the pre-trained model some text and see what it predicts (positive (1) or negative (0)) and how sure it is about it (probability).

---

### Multiclass Classification

### DemoMLNet.Tasks.MulticlassClassification

#### **Algorithms (Trainers) used:**

mlContext.MulticlassClassification.Trainers

- .SdcaMaximumEntropy
  - Accuracy: 77.06%
  - Auc: 73.42%
  - F1Score: 85.96%
- .LightGbm
  - Accuracy:
  - Auc:
  - F1Score:

DemoMLNet.Tasks.MulticlassClassification is a .NET Core 3.1 Console Application.  
A pre-trained model predicts the category of a book/movie/joke - based on a summary (text) about it that you give as an input.

The target of the multiclass classification is Storytel.bg - a Bulgarian platform for audio books etc.  
Link: https://www.storytel.com/bg/bg/

---

### Object Detection

### DemoMLNet.Tasks.ObjectDetection\.Console

DemoMLNet.Tasks.ObjectDetection\.Console is a .NET Core 3.1 Console Application.

### DemoMLNet.Tasks.ObjectDetection\.Web

DemoMLNet.Tasks.ObjectDetection\.Web is a .NET Core 3.1 Web (MVC) Application.  
Its only purpose is to create a better environment for the dev to upload new images and evaluate the results that the ONNX prediction model provides.

![Person Object Detected](Resources/Screenshots/PT_Demo_ML.NETests_webapp.jpg)

#### **Algorithm used: OnnxModelScorer ( TinyYolo2_model.onnx )**

Finds all the familiar objects in an image (person, dog, horse, bike, car etc.)

---

### Recommendation

### DemoMLNet.Tasks.Recommendation

The source used for this demo came from kaggle.com.  
Link: https://www.kaggle.com/arashnic/book-recommendation-dataset  
Book Recommendation Dataset includes 3 tables:

- Books.csv
- Ratings.csv
- Users.csv - ingored

These datasets are simplified to a single table that includes UsersIds to BooksIds that are recommended with rating between 8 and 10 (highly liked).

This information is used to evaluate if a random User would like a Book, based on network of the recommended books within the UserId-BookId table.

## Credits

Implementing my first ML.NET project was a piece of cake thanks to:

- https://dotnet.microsoft.com/learn/ml-dotnet - a free tutorials by Microsoft about Machine Learning with ML.NET
- https://www.youtube.com/watch?v=dluB5VE1m1k&feature=emb_logo - a great lecture by Nikolay Kostov (@NikolayIT) presented in Software University (SoftUni) in 2020.

\~THE END\~

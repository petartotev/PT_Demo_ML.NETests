# PT_Demo_ML.NETests

## General Information

This repository contains a solution with a number of .NET Core projects that experiment with the ML.NET library.

_This is an educational project. That's why there are a lot of comments with explanations on what different sections of code do. They help me understand easier the steps of machine learning:  
'gather info' > 'build model' > 'train model' > 'test model' > 'evaluate model' > 'use model'._

![Horse Object Detected](Resources/Screenshots/PT_Demo_ML.NETests_cover.jpg)

## Technologies

- AngleSharp
- Microsoft\.ML\.NET
- Microsoft\.ML\.LightGBM
- Microsoft\.ML\.Recommender

## Contents

'PT_Demo_ML.NETests' solution consists of 6 projects in 2 directories:

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

## Data

---

## DemoMLNet.Data.Gatherer (Web Crawler)

DemoMLNet.Data.Gatherer is a .NET Core 3.1 Console Application.  
It is implemented to play the role of a web crawler.  
Once an instance of DataGatherer is instantiated it creates new HttpClient and AngleSharp HtmlParser objects.  
They are used within a for-loop that goes through a number of pages with URLs that end with an article/category number.  
For example: https://www.flagman.bg/article/{articleId}  
The result of this crawling is a string. It is cleaned from all its html tag elements and is saved as a .txt file.  
The .txt files gathered would be used to train a ML model (usually 80% for training and 20% for evaluating the accuracy of the algorithm used).

---

## Tasks

---

## BinaryClassification

### DemoMLNet.Tasks.BinaryClassification (Sentiment Analysis)

#### **Algorithms** (MLContext.BinaryClassification.Trainers):

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

#### **Data Source:**

Link: https://www.flagman.bg/  
For this project the target of the sentiment analysis is 'Flagman' - a local online media in Burgas, Bulgaria.

#### **Summary:**

DemoMLNet.Tasks.BinaryClassification is a .NET Core 3.1 Console Application.  
A pre-trained model predicts if a certain comment that you write as input would be more likely to receive a positive (1) or a negative (0) reaction from the users that read it.

---

## Multiclass Classification

### DemoMLNet.Tasks.MulticlassClassification

#### **Algorithms** (MLContext.MulticlassClassification.Trainers):

- .SdcaMaximumEntropy
  - Accuracy: 77.06%
  - Auc: 73.42%
  - F1Score: 85.96%
- .LightGbm
  - Accuracy:
  - Auc:
  - F1Score:

#### **Data Source:**

Link: https://www.storytel.com/bg/bg/  
The target of the multiclass classification is Storytel.bg - a Bulgarian platform for audio books etc.

#### **Summary:**

DemoMLNet.Tasks.MulticlassClassification is a .NET Core 3.1 Console Application.  
A pre-trained model predicts the category of a book/movie/joke - based on a summary (text) about it that one defines as an input.

---

## Object Detection

### DemoMLNet.Tasks.ObjectDetection\.Console

#### **Algorithm:**

OnnxModelScorer (TinyYolo2_model.onnx)

#### **Summary:**

DemoMLNet.Tasks.ObjectDetection\.Console is a .NET Core 3.1 Console Application.  
It uses a pre-trained ONNX model that analyzes a picture and evaluates if there are certain objects that are familiar for the model (person, dog, horse, bike, car etc.).

### DemoMLNet.Tasks.ObjectDetection\.Web

#### **Summary:**

DemoMLNet.Tasks.ObjectDetection\.Web is a .NET Core 3.1 Web (MVC) Application.  
Its only purpose is to create a better environment for the dev to upload new images and evaluate the results that the ONNX prediction model provides.

![Person Object Detected](Resources/Screenshots/PT_Demo_ML.NETests_webapp.jpg)

---

## Recommendation

### DemoMLNet.Tasks.Recommendation

#### **Algorithm:**

MLContext.Recommendation().Trainers.MatrixFactorization()

#### **Data Source:**

Link: https://www.kaggle.com/arashnic/book-recommendation-dataset  
The source used for this demo came from kaggle.com.  
The Dataset includes 3 tables:

- Books.csv
- Ratings.csv
- Users.csv - ingored
  Those are gathered in a single table with only 2 columns (UsersIds, BooksIds).  
  In addition, only those that are recommended with a rating between 8 and 10 (highly liked) are taken.

#### **Summary:**

Once gathered, the model evaluates if a User would like a Book - based on a network of recommended books within the table.

---

## Regression

### DemoMLNet.Tasks.Regression

#### **Algorithm:**

MLContext.Regression.Trainers.LightGbm

#### **Data Source:**

Link: https://www.kaggle.com/adityadesai13/used-car-dataset-ford-and-mercedes  
The source used for this demo came from kaggle.com.  
The Dataset includes a number of tables that I gathered into 1 common table with the following columns:  
model,year,price,transmission,mileage,fuelType,tax,mpg,engineSize

#### **Summary:**

Once trained, the model can test input and evaluate its hypothetical price, based on the other properties of the input (model, mileage, year, transmission).

---

## Credits

---

Implementing my first ML.NET project was a piece of cake thanks to:

- https://dotnet.microsoft.com/learn/ml-dotnet - a free set of tutorials by Microsoft about Machine Learning with ML\.NET
- https://www.youtube.com/watch?v=dluB5VE1m1k&feature=emb_logo - a great lecture by Nikolay Kostov (@NikolayIT) presented in SoftUni in 2020.

\~THE END\~

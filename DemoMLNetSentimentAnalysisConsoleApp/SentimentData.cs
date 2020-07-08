using Microsoft.ML.Data;

namespace DemoMLNetSentimentAnalysisConsoleApp
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public string SentimentText;

        [LoadColumn(1), ColumnName("Label")]
        public bool Sentiment;
    }    
}

// The input dataset class, SentimentData, has a string for user comments (SentimentText) and a bool (Sentiment) value of either 1 (positive) or 0 (negative) for sentiment. 
// Both fields have LoadColumn attributes attached to them, which describes the data file order of each field. 
// In addition, the Sentiment property has a ColumnName attribute to designate it as the Label field. 
// The following example file doesn't have a header row, and looks like this:
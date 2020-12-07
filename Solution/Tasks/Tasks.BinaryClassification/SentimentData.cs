namespace DemoMLNetSentimentAnalysisConsoleApp
{
    using Microsoft.ML.Data;

    /// <summary>
    /// INPUT dataset class.
    /// The class has a string for user comments (SentimentText) and a bool (Sentiment) value of either 1 (positive) or 0 (negative) for sentiment.
    /// Both fields have LoadColumn attributes attached to them, which describes the data file order of each field. 
    /// In addition, the Sentiment property has a ColumnName attribute to designate it as the Label field. 
    /// </summary>
    public class SentimentData
    {
        // User Comment
        [LoadColumn(0)]
        public string SentimentText;

        // Vote Value (0 or 1)
        [LoadColumn(1), ColumnName("Label")]
        public bool Sentiment;
    }    
}
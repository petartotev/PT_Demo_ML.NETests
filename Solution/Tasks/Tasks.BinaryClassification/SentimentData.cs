namespace DemoMLNetSentimentAnalysisConsoleApp
{
    using Microsoft.ML.Data;

    /// <summary>
    /// INPUT dataset class.
    /// The class has a string for user comments (SentimentText) and a bool (Sentiment) value (1 (positive) or 0 (negative)).
    /// Both fields have LoadColumn attributes attached to them, which describes the data file order of each field. 
    /// </summary>
    public class SentimentData
    {        
        [LoadColumn(0)]
        public string SentimentText; // Content.
                
        [LoadColumn(1), ColumnName("Label")] // The attribute designates it as the Label field.
        public bool Sentiment; // Vote Value (0 or 1).
    }    
}
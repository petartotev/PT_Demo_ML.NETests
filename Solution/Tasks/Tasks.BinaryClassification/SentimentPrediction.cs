namespace DemoMLNetSentimentAnalysisConsoleApp
{
    using Microsoft.ML.Data;

    /// <summary>
    /// OUTPUT dataset class.
    /// SentimentPrediction is the prediction class used after model training.
    /// It inherits from SentimentData so that the input SentimentText can be displayed along with the output prediction. 
    /// The Prediction boolean is the value that the model predicts when supplied with new input SentimentText.
    /// The output class SentimentPrediction contains 2 other properties calculated by the model: 
    /// - Prediction - the raw score calculated by the model
    /// - Probability - the score calibrated to the likelihood of the text having positive sentiment
    /// </summary>
    public class SentimentPrediction : SentimentData
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; } // The most important property for this tutorial.

        public float Score { get; set; }
    }
}
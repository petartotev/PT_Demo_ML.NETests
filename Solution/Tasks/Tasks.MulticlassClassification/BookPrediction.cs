namespace DemoMLNetMultiClassificationConsoleApp
{
    using Microsoft.ML.Data;

    public class BookPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Category { get; set; }

        public float[] Score { get; set; }
    }
}

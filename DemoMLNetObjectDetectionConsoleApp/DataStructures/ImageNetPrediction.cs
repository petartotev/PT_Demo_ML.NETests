using Microsoft.ML.Data;

namespace DemoMLNetObjectDetectionConsoleApp.DataStructures
{
    public class ImageNetPrediction
    {
        [ColumnName("grid")]
        public float[] PredictedLabels;
    }
}

using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoMLNetMultiClassificationConsoleApp
{
    public class BookPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Category { get; set; }

        public float[] Score { get; set; }
    }
}

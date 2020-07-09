using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoMLNetMultiClassificationConsoleApp
{
    public class BookData
    {
        [LoadColumn(1)]
        public string Category { get; set; }

        [LoadColumn(2)]
        public string Summary { get; set; }
    }
}

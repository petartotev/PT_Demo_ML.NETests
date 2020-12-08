namespace DemoMLNet.Tasks.Regression
{
    using Microsoft.ML.Data;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class CarsData
    {
        [LoadColumn(0)]
        public string Model { get; set; }

        [LoadColumn(1)]
        public float Year { get; set; }

        [LoadColumn(2)]
        public float Price { get; set; }

        [LoadColumn(3)]
        public string Transmission { get; set; }

        [LoadColumn(4)]
        public float Mileage { get; set; }

        [LoadColumn(5)]
        public string FuelType { get; set; }

        [LoadColumn(6)]
        public float Tax { get; set; }

        [LoadColumn(7)]
        public float Mpg { get; set; }

        [LoadColumn(8)]
        public float EngineSize { get; set; }
    }
}

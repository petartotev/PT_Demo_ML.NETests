namespace DemoMLNet.Tasks.Recommendation
{
    using Microsoft.ML.Data;

    public class UserBook
    {
        [LoadColumn(0)]
        public int UserId { get; set; }

        [LoadColumn(1)]
        public string BookId { get; set; } // ISBN

        [LoadColumn(2)]
        public float Label { get; set; }
    }
}

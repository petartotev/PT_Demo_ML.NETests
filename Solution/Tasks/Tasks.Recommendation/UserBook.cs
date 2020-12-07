using Microsoft.ML.Data;

namespace DemoMLNet.Tasks.Recommendation
{
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

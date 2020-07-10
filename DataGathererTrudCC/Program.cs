namespace DataGathererTrudCC
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using AngleSharp.Html.Parser;

    public class Program
    {
        static void Main(string[] args)
        {
            var comments = new DataGathererTrud().GatherData().GetAwaiter().GetResult();
        }
    }
}

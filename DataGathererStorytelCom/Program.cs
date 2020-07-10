namespace DataGathererStorytelCom
{
    using System;
    using System.Text;

    public class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;

            var comments = new DataGatherer().Gather().GetAwaiter().GetResult();
        }
    }
}

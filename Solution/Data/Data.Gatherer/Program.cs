using System;
using System.Text;
using static DemoMLNet.Data.Gatherer.DataSources.StaticDataSources;

namespace DemoMLNet.Data.Gatherer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;

            DataGatherer dataGatherer = new DataGatherer();

            // Will return 0 once the Gather methods are executed.
            int commentsFlagmanBg = dataGatherer.GatherDataFromFlagmanBg(FlagmanBg.Path).GetAwaiter().GetResult();
            int commentsStorytelBg = dataGatherer.GatherDataFromStorytelBg(StorytelBg.Path).GetAwaiter().GetResult();
            int commentsTrudBg = dataGatherer.GatherDataFromTrudBg(TrudBg.Path).GetAwaiter().GetResult();
        }
    }
}

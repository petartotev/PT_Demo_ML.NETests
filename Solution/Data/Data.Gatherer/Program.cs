namespace DemoMLNet.Data.Gatherer
{
    using System;
    using System.Text;
    using static DemoMLNet.Data.Gatherer.DataSources.StaticDataSources;

    public class Program
    {
        static void Main()
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;

            DataGatherer dataGatherer = new DataGatherer();

            // Will return 0 once a Gather method is executed.
            int commentsFlagmanBg = dataGatherer.GatherDataFromFlagmanBg(FlagmanBg.Path).GetAwaiter().GetResult();
            int commentsStorytelBg = dataGatherer.GatherDataFromStorytelBg(StorytelBg.Path).GetAwaiter().GetResult();
            int commentsTrudBg = dataGatherer.GatherDataFromTrudBg(TrudBg.Path).GetAwaiter().GetResult();
        }
    }
}


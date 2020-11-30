using System;
using System.Text;

namespace DemoMLNet.Data.Gatherer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;

            DataGatherer dataGatherer = new DataGatherer();
                        
            // Will return 0 once the Gather methods are executed.
            string pathToSaveFileFlagmanBg = @"C:\myFolder\FlagmanBgCommentsData.txt";
            int commentsFlagmanBg = dataGatherer.GatherDataFromFlagmanBg(pathToSaveFileFlagmanBg).GetAwaiter().GetResult();

            string pathToSaveFileStorytelBg = @"C:\myFolder\DataGathererStorytelCom.txt";
            int commentsStorytelBg = dataGatherer.GatherDataFromStorytelBg(pathToSaveFileStorytelBg).GetAwaiter().GetResult();

            string pathToSaveFileTrudBg = @"C:\myFolder\TrudCCDataGatherer.txt";
            int commentsTrudBg = dataGatherer.GatherDataFromTrudBg(pathToSaveFileTrudBg).GetAwaiter().GetResult();
        }
    }
}

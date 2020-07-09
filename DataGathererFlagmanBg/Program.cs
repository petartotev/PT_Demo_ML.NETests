using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace DataGathererFlagmanBg
{
    public class Program
    {
        static void Main()
        {
            var comments = new DataGatherer().GatherData().GetAwaiter().GetResult();
        }
    }
}

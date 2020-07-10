namespace DataGathererStorytelCom
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    // AngleSharp is a .NET library that gives you the ability to parse angle bracket based hyper-texts like HTML, SVG, and MathML.
    using AngleSharp.Html.Parser;

    public class DataGatherer
    {
        public async Task<int> Gather()
        {       
            var client = new HttpClient();

            var parser = new HtmlParser();

            for (int idBookAudio = 2000000; idBookAudio >= 1; idBookAudio--)
            {
                Console.Write($"{idBookAudio} ->");
                Console.Write(" . ");

                var url = $"https://www.storytel.com/bg/bg/books/{idBookAudio}";

                string htmlContent = null;

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        var response = await client.GetAsync(url);
                        htmlContent = await response.Content.ReadAsStringAsync();
                        break;
                    }
                    catch
                    {
                        Console.Write("catch!");
                        Thread.Sleep(500);
                    }
                }
                
                if (string.IsNullOrWhiteSpace(htmlContent))
                {
                    continue;
                }

                var document = await parser.ParseDocumentAsync(htmlContent);

                // CATEGORY

                if (document.QuerySelectorAll("[itemprop=\"category\"]").Count() <= 0)
                {
                    continue;
                }

                var category = document.QuerySelectorAll("[itemprop=\"category\"]").First().InnerHtml.ToString().Trim();
                category = RemoveAllHtmlElementsFromString(category);

                // SUMMARY

                if (document.QuerySelectorAll("[itemprop=\"description\"]").Count() <= 0)
                {
                    continue;
                }

                var summary = document.QuerySelectorAll("[itemprop=\"description\"]").First().InnerHtml.ToString().Trim();
                summary = RemoveAllHtmlElementsFromString(summary);

                // IF (NO CATEGORY OR NO SUMMARY) => CONTINUE!

                if (string.IsNullOrEmpty(summary) || string.IsNullOrEmpty(category))
                {
                    continue;
                }

                // WRITE TO FILE

                string myLineToWriteToFile = $"{idBookAudio},{category},\"{summary}\"";

                File.AppendAllText(@"C:\myFolder\DataGathererStorytelCom.txt", myLineToWriteToFile + Environment.NewLine);
            }

            return 0;
        }

        public static string RemoveAllHtmlElementsFromString(string inputString)
        {
            string outputString = inputString
                .Replace("<br>", "")
                .Replace("<strong>", "")
                .Replace("</strong>", "")
                .Replace("<em>", "")
                .Replace("</em>", "")
                .Replace("<span>", "")
                .Replace("</span>", "")
                .Replace("&nbsp;", "");

            //while (true)
            //{
            //    if (!outputString.Contains("<a"))
            //    {
            //        break;
            //    }

            //    int startIndexAhref = outputString.IndexOf("<a");
            //    int firstIndexAhrefEnd = outputString.IndexOf("a>");
            //    int count = firstIndexAhrefEnd - startIndexAhref + 2;

            //    outputString = outputString.Remove(startIndexAhref, count);
            //}

            //while (true)
            //{
            //    if (!outputString.Contains("<span "))
            //    {
            //        break;
            //    }

            //    int startIndexSpanStart = outputString.IndexOf("<span ");
            //    int endIndexOfSpan = outputString.IndexOf("\">");
            //    int countSpan = endIndexOfSpan - startIndexSpanStart + 2;

            //    outputString = outputString.Remove(startIndexSpanStart, countSpan);
            //}

            outputString = outputString.Replace("\"", "");

            return outputString;
        }
    }
}


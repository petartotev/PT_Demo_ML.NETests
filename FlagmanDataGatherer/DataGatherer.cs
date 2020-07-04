namespace DataGathererFlagmanBg
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
        public async Task<int> GatherData()
        {
            // Creates a new parser that comes from the AngleSharp library - with the default options and context.
            var parser = new HtmlParser();

            var client = new HttpClient();

            // https://www.flagman.bg/ is a local online media in the district of Burgas, Bulgaria.
            // It has articles about politics, society, local municipalities, sport and investigation.
            // The reason flagman.bg was chosen for the demo is that the urls of the articles there are easily put into a for-loop.
            // Each article has its unique id that comes in the query string - for example https://www.flagman.bg/article/218139.
            for (var articleId = 218000; articleId >= 1; articleId--)
            {
                Console.Write($"{articleId} => ");
                Console.Write('^');

                var url = $"https://www.flagman.bg/article/{articleId}";

                string htmlContent = null;

                for (var i = 0; i < 10; i++)
                {
                    try
                    {
                        var response = await client.GetAsync(url);
                        htmlContent = await response.Content.ReadAsStringAsync();
                        break;
                    }
                    catch
                    {
                        Console.Write('!');
                        Thread.Sleep(500);
                    }
                }

                if (string.IsNullOrWhiteSpace(htmlContent))
                {
                    break;
                }

                var document = await parser.ParseDocumentAsync(htmlContent);

                // Get the Comments section.
                var htmlComments = document.GetElementsByClassName("commentsBox");

                if (htmlComments.Length == 0)
                {
                    continue;
                }

                // Foreach comment in the Comments section...
                foreach (var htmlComment in htmlComments)
                {
                    int commentBinaryValue;

                    // Get the comment content.
                    var commentText = htmlComment
                        .GetElementsByClassName("commentText")
                        .FirstOrDefault()
                        .InnerHtml
                        .ToString()
                        .Replace("\n", string.Empty)
                        .Replace("\t", string.Empty)
                        .Replace("<br>", " ")
                        .Trim();

                    if (commentText == null)
                    {
                        continue;
                    }

                    // Get the Up Vote Count as string and clean it.
                    string commentUpvoteCountString = htmlComment
                        .GetElementsByClassName("head").FirstOrDefault()
                        .GetElementsByClassName("rating text-right").FirstOrDefault()
                        .GetElementsByClassName("text-center ml-2").FirstOrDefault()
                        .GetElementsByClassName("commentsUp").FirstOrDefault()
                        .InnerHtml
                        .ToString()
                        .Replace("\n", string.Empty)
                        .Replace("\t", string.Empty)
                        .Replace("<br>", " ")
                        .Trim();

                    // int.Parse the Up Vote Count string.
                    int commentUpvoteCount = int.Parse(commentUpvoteCountString);

                    // Get the Down Vote Count as string and clean it.
                    string commentDownvoteCountString = htmlComment
                        .GetElementsByClassName("head").FirstOrDefault()
                        .GetElementsByClassName("rating text-right").FirstOrDefault()
                        .GetElementsByClassName("text-center").FirstOrDefault()
                        .GetElementsByClassName("commentsDown").FirstOrDefault()
                        .InnerHtml
                        .ToString().
                        Replace("\n", string.Empty)
                        .Replace("\t", "")
                        .Replace("<br>", " ")
                        .Trim();

                    // int.Parse the Down Vote Count string.
                    int commentDownvoteCount = int.Parse(commentDownvoteCountString);

                    // Deep doubts about how to define a "positive" / "negative" comment criteria.
                    // After a few attempts I finally tried with:
                    // A positive comment is the one that has 30 more positive comments compared to the ones that are negative.
                    // A negative comment is the one that has 15 more negative comments compared to the ones that are positive.
                    if ((commentUpvoteCount - commentDownvoteCount >= 30) || (commentDownvoteCount - commentUpvoteCount >= 15))
                    {
                        if (commentUpvoteCount - commentDownvoteCount >= 30)
                        {
                            // positive
                            commentBinaryValue = 1;
                        }
                        else
                        {
                            // negative
                            commentBinaryValue = 0;
                        }

                        string myLineToWriteToFile = $"{commentText}\t{commentBinaryValue}";

                        File.AppendAllText(@"C:\myFolder\FlagmanBgCommentsData.txt", myLineToWriteToFile + Environment.NewLine);
                    }
                }
            }

            return 0;
        }
    }
}

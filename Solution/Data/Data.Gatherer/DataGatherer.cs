namespace DemoMLNet.Data.Gatherer
{
    // AngleSharp is a .NET library which gives the ability to parse angle bracket based hyper-texts like HTML, SVG etc.
    using AngleSharp.Dom;
    using AngleSharp.Html.Parser;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using static DemoMLNet.Data.Gatherer.DataSources.StaticDataSources;

    public class DataGatherer
    {
        private HtmlParser parser; //using AngleSharp.Html.Parser.HtmlParser
        private HttpClient client;

        public DataGatherer()
        {
            parser = new HtmlParser();
            client = new HttpClient();
        }

        /// <summary>
        /// https://www.flagman.bg/ is a local online media in the district of Burgas, Bulgaria.
        /// It has articles on politics, society, local municipalities, sport and investigation.
        /// Flagman.bg was a choice for the demo as the urls of the articles are easy to put into a for-loop crawler.
        /// Each article has unique id included in the query string - https://www.flagman.bg/article/218139.
        /// </summary>
        /// <param name="pathToFile">Path to the file in which a new 'comment, vote binary ratio' line is appended for each article crawled.</param>
        /// <returns>0 if the method is executed successfully.</returns>
        public async Task<int> GatherDataFromFlagmanBg(string pathToFile, int articleId = 219000)
        {
            for (var id = articleId; id >= 1; id--)
            {
                PrintActionSeparator(id);

                string htmlContent = null;

                for (var i = 0; i < 10; i++)
                {
                    try
                    {
                        var response = await client.GetAsync(FlagmanBg.Url + id);
                        htmlContent = await response.Content.ReadAsStringAsync();
                        break;
                    }
                    catch
                    {
                        PrintException();
                    }
                }

                if (string.IsNullOrWhiteSpace(htmlContent))
                {
                    break;
                }

                // AngleSharp parses the string response to understandable Html.
                var document = await parser.ParseDocumentAsync(htmlContent);

                // Get the Comments section.
                var htmlComments = document.GetElementsByClassName("commentsBox");

                if (htmlComments == null || htmlComments.Length == 0)
                {
                    continue;
                }

                // Foreach comment in the Comments section...
                foreach (var htmlComment in htmlComments)
                {      
                    // Get the comment content.
                    string commentText = htmlComment
                        .GetElementsByClassName("commentText")
                        .FirstOrDefault()
                        .InnerHtml
                        .ToString()
                        .Replace("\n", string.Empty)
                        .Replace("\t", string.Empty)
                        .Replace("<br>", " ")
                        .Trim();

                    if (string.IsNullOrEmpty(commentText))
                    {
                        continue;
                    }

                    int commentUpvoteCount = GetFlagmanBgCommentVoteAsIntegerByHtmlClassName(htmlComment, "commentsUp");
                    int commentDownvoteCount = GetFlagmanBgCommentVoteAsIntegerByHtmlClassName(htmlComment, "commentsDown");

                    // What is really a "positive" / "negative" comment? Let's assume that:
                    // A positive comment is the one that has 30 more positive comments compared to the ones that are negative.
                    // A negative comment is the one that has 15 more negative comments compared to the ones that are positive.

                    // Get the binary value of the comment - positive (1) or negative (0).
                    int commentBinaryValue;

                    if ((commentUpvoteCount - commentDownvoteCount >= 30) || (commentDownvoteCount - commentUpvoteCount >= 15))
                    {
                        if (commentUpvoteCount - commentDownvoteCount >= 30)
                        {                            
                            commentBinaryValue = 1; // positive
                        }
                        else
                        {                            
                            commentBinaryValue = 0; // negative
                        }

                        string line = $"{commentText}\t{commentBinaryValue}";
                        AppendLineToFile(line, pathToFile);
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// https://www.storytel.com/ is a Bulgarian platform for audio books.
        /// This platform was very suitable for the purposes of this demo, as each book belongs to a certain category.
        /// </summary>
        /// <param name="pathToFile">>Path to the file in which a new 'book, category, summary' line is appended for each article crawled.</param>
        /// <returns>0 if the method is executed successfully.</returns>
        public async Task<int> GatherDataFromStorytelBg(string pathToFile, int audioBookId = 2000000)
        {
            for (int id = audioBookId; id >= 1; id--)
            {
                PrintActionSeparator(id);

                string htmlContent = null;

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        var response = await client.GetAsync(StorytelBg.Url + id);
                        htmlContent = await response.Content.ReadAsStringAsync();
                        break;
                    }
                    catch
                    {
                        PrintException();
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
                category = CleanStringFromHtmlElements(category);

                // SUMMARY
                if (document.QuerySelectorAll("[itemprop=\"description\"]").Count() <= 0)
                {
                    continue;
                }

                var summary = document.QuerySelectorAll("[itemprop=\"description\"]").First().InnerHtml.ToString().Trim();
                summary = CleanStringFromHtmlElements(summary);

                // IF (NO CATEGORY OR NO SUMMARY) => CONTINUE!
                if (string.IsNullOrEmpty(summary) || string.IsNullOrEmpty(category))
                {
                    continue;
                }

                // WRITE TO FILE
                string line = $"{id},{category},\"{summary}\"";
                AppendLineToFile(line, pathToFile);
            }

            return 0;
        }

        /// <summary>
        /// https://trud.bg/ is a national media and publishing house in Bulgaria.
        /// Its books are organized into categories, so it is again easy to get (category <> summary) information.
        /// </summary>
        /// <param name="pathToFile">Path to the file in which a new 'book, category, summary' line is appended for each article crawled.</param>
        /// <returns>0 if the method is executed successfully.</returns>
        public async Task<int> GatherDataFromTrudBg(string pathToFile, int bookId = 11066)
        {
            for (int id = bookId; id >= 1; id--)
            {
                PrintActionSeparator(id);

                string htmlContent = null;

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        var response = await client.GetAsync(TrudBg.Url + id);
                        htmlContent = await response.Content.ReadAsStringAsync();
                        break;
                    }
                    catch
                    {
                        PrintException();
                    }
                }

                if (string.IsNullOrWhiteSpace(htmlContent))
                {
                    continue;
                }

                var document = await parser.ParseDocumentAsync(htmlContent);


                // BOOK DESCRIPTION
                var divDescription = document.GetElementsByClassName("book-description").FirstOrDefault();

                if (divDescription == null)
                {
                    continue;
                }

                var allParagraphsInDivDescription = divDescription.GetElementsByTagName("P");

                StringBuilder descriptionStringBuilder = new StringBuilder();

                foreach (var paragraph in allParagraphsInDivDescription)
                {
                    descriptionStringBuilder.Append(paragraph.InnerHtml + " ");
                }

                string description = descriptionStringBuilder.ToString().TrimEnd();

                string descriptionClean = CleanStringFromHtmlElements(description);

                // BOOK GENRE
                var divBookGenre = document.GetElementsByClassName("book-genre").FirstOrDefault();

                if (divBookGenre == null)
                {
                    continue;
                }

                string genre = string.Empty;

                if (divBookGenre.GetElementsByTagName("A").Count() >= 1)
                {
                    genre = divBookGenre.GetElementsByTagName("A")[0].InnerHtml.Split(',', StringSplitOptions.RemoveEmptyEntries)[0];
                }

                if (string.IsNullOrEmpty(genre))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(genre) && !string.IsNullOrEmpty(descriptionClean) && !descriptionClean.Contains("<xml>"))
                {
                    if (TrudBg.ListGenresToSearch.Contains(genre))
                    {
                        string line = $"{id},{genre},\"{descriptionClean}\"";
                        AppendLineToFile(line, pathToFile);
                    }
                }
            }

            return 0;
        }

        private static string CleanStringFromHtmlElements(string inputString)
        {
            // Clean input string from all html tags.
            string outputString = inputString
                .Replace("<br>", "")
                .Replace("</br>", "")
                .Replace("<hr>", "")
                .Replace("</hr>", "")
                .Replace("<strong>", "")
                .Replace("</strong>", "")
                .Replace("<em>", "")
                .Replace("</em>", "")
                .Replace("<span>", "")
                .Replace("</span>", "")
                .Replace("&nbsp;", "")
                .Replace("\"", "");

            // Clean input string from all <a> tags.
            while (true)
            {
                if (!outputString.Contains("<a"))
                {
                    break;
                }

                int startIndexAhref = outputString.IndexOf("<a");
                int firstIndexAhrefEnd = outputString.IndexOf("a>");
                int count = firstIndexAhrefEnd - startIndexAhref + 2;

                outputString = outputString.Remove(startIndexAhref, count);
            }

            // Clean input string from all <span> tags.
            while (true)
            {
                if (!outputString.Contains("<span "))
                {
                    break;
                }

                int startIndexSpanStart = outputString.IndexOf("<span ");
                int endIndexOfSpan = outputString.IndexOf("\">");
                int countSpan = endIndexOfSpan - startIndexSpanStart + 2;

                outputString = outputString.Remove(startIndexSpanStart, countSpan);
            }

            outputString = CleanStringFromHtmlElements(outputString);
            return outputString;
        }

        private static void AppendLineToFile(string line, string path)
        {
            File.AppendAllText($@"{path}", line + Environment.NewLine);
        }

        private static int GetFlagmanBgCommentVoteAsIntegerByHtmlClassName(IElement htmlComment, string className)
        {
            // Get the Vote Count as string and clean it.
            string commentUpvoteCountString = htmlComment
                .GetElementsByClassName("head")
                .FirstOrDefault()
                .GetElementsByClassName("rating text-right")
                .FirstOrDefault()
                .GetElementsByClassName("text-center ml-2")
                .FirstOrDefault()
                .GetElementsByClassName($"{className}")
                .FirstOrDefault()
                .InnerHtml
                .ToString()
                .Replace("\n", string.Empty)
                .Replace("\t", string.Empty)
                .Replace("<br>", " ")
                .Trim();

            // int.Parse(Vote Count string) and return it.
            return int.Parse(commentUpvoteCountString);
        }

        private static void PrintActionSeparator(int index)
        {
            Console.Write($"{index}");
            Console.Write(" | ");
        }

        private static void PrintException()
        {
            Console.Write("> catch!");
            Thread.Sleep(500);
        }
    }
}

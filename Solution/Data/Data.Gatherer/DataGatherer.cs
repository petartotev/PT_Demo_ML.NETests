// AngleSharp is a .NET library which gives the ability to parse angle bracket based hyper-texts like HTML, SVG etc.
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static DemoMLNet.Data.Gatherer.DataSources.StaticDataSources;

namespace DemoMLNet.Data.Gatherer
{
    public class DataGatherer
    {
        private HtmlParser parser; //using AngleSharp.Html.Parser.HtmlParser
        private HttpClient client;

        public DataGatherer()
        {
            parser = new HtmlParser();
            client = new HttpClient();
        }

        public async Task<int> GatherDataFromFlagmanBg(string pathToSaveFile)
        {
            // Creates a parser coming from the AngleSharp library (default options and context).
            // https://www.flagman.bg/ is a local online media in the district of Burgas, Bulgaria.
            // It has articles about politics, society, local municipalities, sport and investigation.
            // The reason flagman.bg was chosen for the demo is that the urls of the articles there are easily put into a for-loop.
            // Each article has its unique id that comes in the query string - for example https://www.flagman.bg/article/218139.
            for (var articleId = 219000; articleId >= 1; articleId--)
            {
                Console.Write($"{articleId} => ");
                Console.Write('^');


                string htmlContent = null;

                for (var i = 0; i < 10; i++)
                {
                    try
                    {
                        var response = await client.GetAsync(FlagmanBg.Url + articleId);
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

                        File.AppendAllText($@"{pathToSaveFile}", myLineToWriteToFile + Environment.NewLine);
                    }
                }
            }

            return 0;
        }

        public async Task<int> GatherDataFromStorytelBg(string pathToSaveFile)
        {
            for (int idBookAudio = 2000000; idBookAudio >= 1; idBookAudio--)
            {
                Console.Write($"{idBookAudio} ->");
                Console.Write(" . ");

                string htmlContent = null;

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        var response = await client.GetAsync(StorytelBg.Url + idBookAudio);
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

                string myLineToWriteToFile = $"{idBookAudio},{category},\"{summary}\"";

                File.AppendAllText($@"{pathToSaveFile}", myLineToWriteToFile + Environment.NewLine);
            }

            return 0;
        }

        public async Task<int> GatherDataFromTrudBg(string pathToSaveFile)
        {
            List<string> listGenresToSearch = new List<string>()
            {
                //"Афоризми",
                //"Българска литература",
                "Български романи",
                //"Драма",
                //"Други",
                //"Езотерика и астрология",
                "Енциклопедии",
                //"За 3. и 4. клас",
                //"За 7. и 8. клас",
                //"За 9. и 10. клас",
                //"За 11. и 12. клас",
                //"За кандидат-шофьори",
                //"За студенти",
                "Истории",
                "Исторически романи",
                "Криминални романи",
                "Кулинарни книги",
                "Медицина",
                "Мемоари и обществено-политическа литература",
                //"Наръчници",
                //"НАМАЛЕНИ КНИГИ",
                //"Научна литература",
                //"Повест",
                "Поезия",
                "Приказки",
                "Приключенски романи",
                //"Проза",
                "Разкази",
                //"Религия и етнология",
                "Речници",
                //"Сексът от древността до днес",
                //"Справочна литература",
                //"Трилър",
                "Фантастика и фентъзи",
                //"Фолклор и митология",
                //"Художествена литература",
                //"Хумор",
                //"Чуждестранна литература",
                //"Чуждестранни романи",
            };

            for (int bookId = 11066; bookId >= 1; bookId--)
            {
                Console.Write($"{bookId} => ");
                Console.Write('^');

                string htmlContent = null;

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        var response = await client.GetAsync(TrudBg.Url + bookId);

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
                    if (listGenresToSearch.Contains(genre))
                    {
                        string myLineToWriteToFile = $"{bookId},{genre},\"{descriptionClean}\"";

                        File.AppendAllText($@"{pathToSaveFile}", myLineToWriteToFile + Environment.NewLine);
                    }
                }
            }

            return 0;
        }

        private static string CleanStringFromHtmlElements(string inputString)
        {
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
    }
}

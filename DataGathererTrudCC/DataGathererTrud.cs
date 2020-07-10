namespace DataGathererTrudCC
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    // AngleSharp is a .NET library that gives you the ability to parse angle bracket based hyper-texts like HTML, SVG, and MathML.
    using AngleSharp.Html.Parser;

    public class DataGathererTrud
    {
        public async Task<int> GatherData()
        {
            List<string> listOfGenres = new List<string>()
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

            var parser = new HtmlParser();

            var client = new HttpClient();

            for (int bookId = 11066; bookId >= 1; bookId--)
            {
                Console.Write($"{bookId} => ");
                Console.Write('^');

                var url = $"https://trud.cc/?cid=9&pid={bookId}";

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

                string descriptionClean = RemoveAllHtmlElementsFromString(description);

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
                    if (listOfGenres.Contains(genre))
                    {
                        string myLineToWriteToFile = $"{bookId},{genre},\"{descriptionClean}\"";

                        File.AppendAllText(@"C:\myFolder\TrudCCDataGatherer.txt", myLineToWriteToFile + Environment.NewLine);
                    }
                }                
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

            outputString = outputString.Replace("\"", "");

            return outputString;
        }
    }
}

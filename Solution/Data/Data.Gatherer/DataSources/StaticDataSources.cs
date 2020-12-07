using PetarTotev.Net.DSA.MyCollections;

namespace DemoMLNet.Data.Gatherer.DataSources
{
    public static class StaticDataSources
    {
        public static class FlagmanBg
        {
            public const string Path = @"C:\myFolder\FlagmanBgCommentsData.txt";
            public const string Url = "https://www.flagman.bg/article/";
        }

        public static class StorytelBg
        {
            public const string Path = @"C:\myFolder\DataGathererStorytelCom.txt";
            public const string Url = "https://www.storytel.com/bg/bg/books/";
        }

        public static class TrudBg
        {
            public const string Path = @"C:\myFolder\TrudCCDataGatherer.txt";
            public const string Url = "https://trud.cc/?cid=9&pid=";

            public static readonly MyList<string> ListGenresToSearch = new MyList<string>()
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
        }
    }
}

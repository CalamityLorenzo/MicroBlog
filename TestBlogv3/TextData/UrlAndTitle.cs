using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestBlogv3.TextData
{
    public static class roodData
    {
        public static string GetSynopsis(int id)
        {
            using(var reader = new StreamReader($"TextData/Synop_{id}.txt"))
            {
                return reader.ReadToEnd();
            }

        }

        public static string GetBodyText(int id)
        {
            using (var reader = new StreamReader($"TextData/Post_{id}.txt"))
            {
                return reader.ReadToEnd();
            }

        }

        public static List<(string url, string title)> UrlTitle =>
            new List<(string, string)>{
                ("things-that-can-be-a-title","Things that can be a title"),
                ("what-a-way-to-show","What a way to show"),
                ("newsgrounds-found-pickles","Newsgrounds found pickles"),
                ("where-do-i-find?","Where do I find?"),
                ("could-you-directthis-is-not-your-office","Could you direct....this is not your office"),
                ("text-fonts-text-sizes","text fonts. Text sizes"),
                ("impertive-genative-dative-nominative","Impertive genative dative nominative"),
                ("accused-of-impertinantance-i-throw-down-my-thesaurus-in-a-rage","Accused of impertinantance I throw down my thesaurus in a rage"),
                ("bright-psark-on-the-meadow","Bright psark on the meadow"),
                ("william-packs-a-punch","William packs a punch"),
            };
        public static List<List<string>> Tags => new List<List<string>>
            {
                new List<string>{"lion","cole","eyetooth","bluff","smut"},
                new List<string>{"elevation","new","rebel","smut","apology","fate"},
                new List<string>{"trust","prophetic","ashes"},
                new List<string>{"among","elongation","plaid"},
                new List<string>{"trust","among","plaid"},
                new List<string>{"new","eyetooth","ashes"},
                new List<string>{"fate","among","smut"},
                new List<string>{"loser","beneath","esoteric","barbershop"},
                new List<string>{"loser","trust","lion"},
                new List<string>{"babershop","plaid","trust"},
        };
        public static List<List<string>> Categories => new List<List<string>>
        {
              new List<string>{"People"},
              new List<string>{"Places", "People"},
              new List<string>{"Architecture", "People"},
              new List<string>{"Places" , "Geography"},
              new List<string>{"Programming", "Philosophy", "Design"},
              new List<string>{"Philosophy", "Programming", "Design"},
              new List<string>{"Design", "Places"},
              new List<string>{"Functional" , "Imperative",},
              new List<string>{"Architecture", "Functional", "Design"},
              new List<string>{ "Places", "Philosophy", "Functional"},
        };

        public static List<DateTime> Published => new List<DateTime>
        {
            DateTime.Parse("27/10/2018"),
            DateTime.Parse("14/05/2018"),
            DateTime.Parse("30/06/2018"),
            DateTime.Parse("01/01/2017"),
            DateTime.Parse("19/01/2018"),
            DateTime.Parse("11/03/2018"),
            DateTime.Parse("09/07/2018"),
            DateTime.Parse("14/02/2018"),
            DateTime.Parse("15/11/2018"),
            DateTime.Parse("08/12/2018"),
        };

        public static List<bool> Available => new List<bool>
        {
            true,
            true,
            true,
            true,
            true,
            false,
            true,
            true,
            false,
            true,
            true,

        };
    }
}

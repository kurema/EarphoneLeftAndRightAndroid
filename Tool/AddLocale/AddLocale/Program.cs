using System;
using System.Linq;
using System.Globalization;

namespace AddLocale
{
    class Program
    {
        static void Main(string[] args)
        {
            var languages = CultureInfo.GetCultures(CultureTypes.AllCultures).GroupBy(a => a.TwoLetterISOLanguageName).Select(a => a.First()).ToArray();

            while (true)
            {
                Console.WriteLine("言語を入力してください。");
                var lang = Console.ReadLine().Trim();
                var hits = languages.Where(a => a.DisplayName.StartsWith(lang)).ToArray();
                if (hits.Length != 1)
                {
                    Console.WriteLine("言語が見当たりません。TwoLetterISOLanguageNameを入力してください");
                    var lang2l = Console.ReadLine().Trim();
                    hits = languages.Where(a => a.TwoLetterISOLanguageName == lang2l).ToArray();
                    if (hits.Length != 1)
                    {
                        Console.WriteLine("言語が見当たりません。");
                        continue;
                    }
                }
                var code = hits[0];
                Console.WriteLine($"{code.DisplayName}が指定されました。");

                var lineLeft = Console.ReadLine().Trim();
                var lineRight = Console.ReadLine().Trim();

                Console.WriteLine($"左は{code.DisplayName}で\"{lineLeft}\"");
                Console.WriteLine($"右は{code}で\"{lineRight}\"");

            }
        }
    }
}

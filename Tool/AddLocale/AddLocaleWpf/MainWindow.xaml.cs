using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Globalization;

namespace AddLocaleWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        CultureInfo[] languages;
        CultureInfo culture;

        public MainWindow()
        {
            InitializeComponent();

            languages = CultureInfo.GetCultures(CultureTypes.AllCultures).GroupBy(a => a.TwoLetterISOLanguageName).Select(a => a.First()).ToArray();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox tb) return;
            CultureInfo result;
            var hits=languages.Where(a => a.DisplayName.StartsWith(tb.Text.Trim())).ToArray();
            if (hits.Length == 1) result = hits[0];
            else
            {
                hits = languages.Where(a => a.TwoLetterISOLanguageName == tb.Text.Trim()).ToArray();
                if (hits.Length == 1) result = hits[0];
                else
                {
                    textBlockHitLang.Text = "該当言語なし";
                    culture = null;
                    return;
                }
            }
            textBlockHitLang.Text = string.Format(textBlockHitLang.Tag.ToString(),result.DisplayName);
            culture = result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            void AppendLog(string text)
            {
                textBoxLog.Text = textBoxLog.Text + $"{text}\n";
                textBoxLog.ScrollToEnd();
            }

            if (culture is null)
            {
                AppendLog("言語が指定されていません。");
                return;

            }
            var lines = textBlockTrans.Text.Replace("\r\n", "\n").Split(new[] { '\n', '\r' });
            if (lines.Length != 2)
            {
                AppendLog("翻訳結果が2行ではありません。");
                return;
            }
            var lr = lines[1].Split("|");
            if (lr.Length != 2)
            {
                AppendLog("|で区切られていません。");
                return;
            }

            string appname = lines[0].Trim();
            string left = lr[0].Trim();
            string right = lr[1].Trim();

            string dir = $"values-{culture.TwoLetterISOLanguageName}";
            System.IO.Directory.CreateDirectory(dir);

            using (var writer = new System.IO.StreamWriter(System.IO.Path.Combine(dir, "strings.xml"), false, Encoding.UTF8))
            {
                var text = $@"<?xml version=""1.0"" encoding=""utf-8"" ?> 
<resources>
  <string name=""app_name"">{appname}</string>
  <string name=""tile_play_name"">{left}/{right}</string>
  <string name=""word.left"">{left}</string>
  <string name=""word.right"">{right}</string>
</resources>";
                writer.WriteLine(text);

                textBlockTrans.Clear();
                textBoxLang.Clear();
            }

            //Stereo test
            //Left|Right

            AppendLog($"{dir}に書き込みました。");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EarphoneLeftAndRight.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DictionaryPage : ContentPage
    {
        public DictionaryPage()
        {
            InitializeComponent();

        }

        private string _Html;
        public string Html { get => _Html;
            set
            {
                _Html = value;
                var labels = Helper.Helpers.XhtmlToFormattedString(value);
                foreach (var item in labels) layoutMain.Children.Add(item);
            }
        }
        public string DictionaryName
        {
            get => labelDictionaryName.Text;
            set => labelDictionaryName.Text = value;
        }
    }
}
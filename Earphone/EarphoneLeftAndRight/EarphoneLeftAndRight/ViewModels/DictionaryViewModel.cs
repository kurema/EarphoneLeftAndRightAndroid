using System.Windows.Input;

namespace EarphoneLeftAndRight.ViewModels
{
    public class DictionaryViewModel : BaseViewModel
    {

        private string _Id;
        public string Id { get => _Id; set => SetProperty(ref _Id, value); }


        private string _Html;
        public string Html { get => _Html; set => SetProperty(ref _Html, value); }


        private string _HtmlLocale;
        public string HtmlLocale { get => _HtmlLocale; set => SetProperty(ref _HtmlLocale, value); }


        private string _DictionaryTitle;
        public string DictionaryTitle { get => _DictionaryTitle; set => SetProperty(ref _DictionaryTitle, value); }


        private string _WebDictionaryLink;

        public DictionaryViewModel()
        {
            OpenWebDictionaryCommand = new Xamarin.Forms.Command(async () =>
            {
                if (!string.IsNullOrWhiteSpace(WebDictionaryLink))
                {
                    try
                    {
                        await Xamarin.Essentials.Browser.OpenAsync(WebDictionaryLink);
                    }
                    catch { }
                }
            });
        }

        public string WebDictionaryLink { get => _WebDictionaryLink; set => SetProperty(ref _WebDictionaryLink, value); }

        public ICommand OpenWebDictionaryCommand { get; }

    }
}

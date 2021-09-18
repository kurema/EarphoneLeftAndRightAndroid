namespace EarphoneLeftAndRight.ViewModels
{
    public class DictionaryViewModel:BaseViewModel
    {

        private string _Id;
        public string Id { get => _Id; set => SetProperty(ref _Id, value); }


        private string _Html;
        public string Html { get => _Html; set => SetProperty(ref _Html, value); }


        private string _DictionaryTitle;
        public string DictionaryTitle { get => _DictionaryTitle; set => SetProperty(ref _DictionaryTitle, value); }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.Linq;

namespace EarphoneLeftAndRight.ViewModels
{
    public class PlayViewModel : BaseViewModel
    {

        public PlayViewModel()
        {

            Title = Resx.AppResources.Play_Title;
            OpenWebCommand = new Command(async a => await Browser.OpenAsync(a.ToString()));
            SpeakCommand = new Command(Speak);
            OpenDictionaryCommand = new Command(async a =>
            {
                await Shell.Current.GoToAsync($"///{nameof(Views.DictionaryTabbed)}?{nameof(ViewModels.DictionaryTabbedViewModel.SelectedItemId)}={a?.ToString() ?? ""}");
            });
            OpenSearchCommand = new Command(async a =>
            {
                await Browser.OpenAsync(string.Format(SearchServiceSelected.Uri, System.Web.HttpUtility.UrlEncode(SearchWord)));
            });
            PlayBeepCommand = new Command(async a =>
            {
                try
                {
                    var nums = a.ToString()?.Split(',').Select(a => double.Parse(a)).ToArray();
                    await Storages.AudioStorage.RegisterWave(nums[0], nums[1], nums[2], nums[3]);
                    await Task.Run(() => { try { Storages.AudioStorage.AudioTest.Play(); } catch { } });
                }
                catch (Exception e)
                {
                }
            });
            PlayBeepShiftCommand = new Command(async a =>
            {
                try
                {
                    var nums = a.ToString()?.Split(',').Select(a => double.Parse(a)).ToArray();
                    await Storages.AudioStorage.RegisterSignWaveStereoShift(nums[0], nums[1], nums[2] == 0 ? Storages.AudioStorage.ShiftDirection.LeftToRight : Storages.AudioStorage.ShiftDirection.RightToLeft);
                    await Task.Run(() => { try { Storages.AudioStorage.AudioTest.Play(); } catch { } });
                }
                catch { }
            });
        }

        private async static void Speak(object a)
        {
            var tts = DependencyService.Get<Dependency.ITextToSpeech>();
            switch (a.ToString())
            {
                case "Left":
                    await tts.Clear();
                    await tts.SpeakLeft();
                    break;
                case "Right":
                    await tts.Clear();
                    await tts.SpeakRight();
                    break;
                case "Both":
                    await tts.Clear();
                    await tts.SpeakLeft();
                    await tts.SpeakRight();
                    break;
            }
        }

        public ICommand OpenWebCommand { get; }
        public ICommand SpeakCommand { get; }
        public ICommand OpenDictionaryCommand { get; }
        public ICommand OpenSearchCommand { get; }

        public ICommand PlayBeepCommand { get; }
        public ICommand PlayBeepShiftCommand { get; }

        private string[] _SearchWords;
        public string[] SearchWords => _SearchWords ??= new string[] {
            Resx.AppResources.Play_StereoTest_Title,
            Resx.LocalResources.LeftRight,
            Resx.LocalResources.Left,
            Resx.LocalResources.Right,
            Resx.AppResources.More_OpenDictionary_Left,
            Resx.AppResources.More_OpenDictionary_Right,
            Resx.AppResources.More_OpenCompass_Title,
            Resx.AppResources.Profile_Accounts_Github_ID,
        };

        private string _SearchWord = null;
        public string SearchWord { get => _SearchWord ?? SearchWords[0]; set => SetProperty(ref _SearchWord, value); }


        private SearchService _SearchServiceSelected = null;
        public SearchService SearchServiceSelected { get => _SearchServiceSelected ?? SearchServices[0]; set => SetProperty(ref _SearchServiceSelected, value); }

        private SearchService[] _SearchServices;
        public SearchService[] SearchServices => _SearchServices ??= new SearchService[]
        {
            new SearchService("Google","https://www.google.com/search?q={0}"),
            new SearchService("DuckDuckGo","https://duckduckgo.com/?q={0}"),
            new SearchService("YouTube","https://www.youtube.com/results?search_query={0}"),
            new SearchService("GitHub","https://github.com/search?q={0}"),
            new SearchService("Amazon","https://www.amazon.com/s?k={0}&tag=kuremastereotest-22"),
            new SearchService("Twitter","https://twitter.com/search?q={0}"),
            new SearchService("Google Play","https://play.google.com/store/search?q={0}"),
        };

        public class SearchService
        {
            public SearchService(string title, string uri)
            {
                Title = title ?? throw new ArgumentNullException(nameof(title));
                Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            }

            public string Title { get; }
            public string Uri { get; }
        }
    }
}

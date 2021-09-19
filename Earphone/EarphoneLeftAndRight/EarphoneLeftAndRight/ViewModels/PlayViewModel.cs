using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;

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

    }
}

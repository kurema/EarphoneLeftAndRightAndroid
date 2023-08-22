//using EarphoneLeftAndRight.Services;
using EarphoneLeftAndRight.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EarphoneLeftAndRight
{
    public partial class App : Application
    {
        private const string appActionIdOpen = "open_";

        public App()
        {
            InitializeComponent();

            //DependencyService.Register<MockDataStore>();
            Xamarin.Essentials.AppActions.OnAppAction += AppActions_OnAppAction;
            MainPage = new AppShell();
        }

        private void AppActions_OnAppAction(object sender, Xamarin.Essentials.AppActionEventArgs e)
        {
            //https://docs.microsoft.com/ja-jp/xamarin/essentials/app-actions?tabs=android

            if (Application.Current != this && Application.Current is App app)
            {
                Xamarin.Essentials.AppActions.OnAppAction -= app.AppActions_OnAppAction;
                return;
            }

            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
            {
                var tts = DependencyService.Get<Dependency.ITextToSpeech>();
                tts.Load();
                switch (e.AppAction.Id)
                {
                    case "play_voice_lr":
                        await Shell.Current.GoToAsync($"///{nameof(Views.PlayPage)}");
                        await tts.Clear();
                        await tts.SpeakLeftAsync();
                        await tts.SpeakRightAsync();
                        break;
                    case "play_voice_left":
                        await Shell.Current.GoToAsync($"///{nameof(Views.PlayPage)}");
                        await tts.Clear();
                        await tts.SpeakLeftAsync();
                        break;
                    case "play_voice_right":
                        await Shell.Current.GoToAsync($"///{nameof(Views.PlayPage)}");
                        await tts.Clear();
                        await tts.SpeakRightAsync();
                        break;
                    case appActionIdOpen + nameof(Views.DictionaryTabbed):
                        await Shell.Current.GoToAsync($"///{nameof(Views.DictionaryTabbed)}");
                        break;
                    case appActionIdOpen + nameof(Views.PlayPage):
                        await Shell.Current.GoToAsync($"///{nameof(Views.PlayPage)}");
                        break;
                    case appActionIdOpen + nameof(Views.ConfigPage):
                        await Shell.Current.GoToAsync($"///{nameof(Views.ConfigPage)}");
                        break;
                    case appActionIdOpen + nameof(Views.BeepTabbed):
                        await Shell.Current.GoToAsync($"///{nameof(Views.BeepTabbed)}");
                        break;
                }
            });
        }

        protected override async void OnStart()
        {
            try
            {
                string? icon = Device.RuntimePlatform switch
                {
                    Device.Android => "@mipmap/icon",
                    _ => null,
                };

                await Xamarin.Essentials.AppActions.SetAsync(
                    new Xamarin.Essentials.AppAction("play_voice_lr", Resx.LocalResources.LeftRight, null, icon)
                    //, new Xamarin.Essentials.AppAction("play_voice_left", Resx.LocalResources.Left, null, icon)
                    //, new Xamarin.Essentials.AppAction("play_voice_right", Resx.LocalResources.Right, null, icon)
                    , new Xamarin.Essentials.AppAction(appActionIdOpen + nameof(Views.PlayPage), Resx.AppResources.Play_Title, null, icon)
                    , new Xamarin.Essentials.AppAction(appActionIdOpen + nameof(Views.BeepTabbed), Resx.AppResources.Sound_Title, null, icon)
                    , new Xamarin.Essentials.AppAction(appActionIdOpen + nameof(Views.DictionaryTabbed), Resx.AppResources.Dict_Header, null, icon)
                    , new Xamarin.Essentials.AppAction(appActionIdOpen + nameof(Views.ConfigPage), Resx.AppResources.Config_Title, null, icon)
                    );
            }
            catch (Xamarin.Essentials.FeatureNotSupportedException)
            {
                //ignore
            }
            catch
            {
                //ignore anyway
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

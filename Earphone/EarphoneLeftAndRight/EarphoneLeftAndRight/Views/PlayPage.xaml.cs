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
    public partial class PlayPage : ContentPage
    {
        private static Dependency.ITextToSpeech tts;


        public PlayPage()
        {
            InitializeComponent();

            tts = DependencyService.Get<Dependency.ITextToSpeech>();
        }

        private async void Button_ClickedLeft(object sender, EventArgs e)
        {
            await tts.Clear();
            await tts.SpeakLeft();
        }

        private async void Button_ClickedRight(object sender, EventArgs e)
        {
            await tts.Clear();
            await tts.SpeakRight();
        }

        private async void Button_ClickedBoth(object sender, EventArgs e)
        {
            await tts.Clear();
            await tts.SpeakLeft();
            await tts.SpeakRight();
        }
    }
}
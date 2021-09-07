using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(EarphoneLeftAndRight.Droid.TextToSpeechDependency))]
namespace EarphoneLeftAndRight.Droid
{

    public class TextToSpeechDependency : Dependency.ITextToSpeech
    {
        public Task Clear()
        {
            Manager.Tts.StopIfSpeaking();
            return Task.CompletedTask;
        }

        public async Task SpeakLeft()
        {
            await Manager.Tts.SpeakLeft();
        }

        public async Task SpeakRight()
        {
            await Manager.Tts.SpeakRight();
        }
    }
}
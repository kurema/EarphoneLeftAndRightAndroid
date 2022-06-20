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
        public bool IsSpeaking => Manager.Tts.Content?.IsSpeaking ?? false;

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

        public async Task<bool> SpeakWithPan(string text, float pan, CultureInfo language)
        {
            var jLoc = new Java.Util.Locale(language.TwoLetterISOLanguageName);
            var tts = Manager.Tts.Content;
            await Manager.Tts.WaitReadyAsync();
            var lang = tts.IsLanguageAvailable(jLoc) >= Android.Speech.Tts.LanguageAvailableResult.Available ? jLoc : null;
            lang = lang ?? (tts.IsLanguageAvailable(Java.Util.Locale.English) >= Android.Speech.Tts.LanguageAvailableResult.Available ? Java.Util.Locale.English : null);
            lang = lang ?? tts.DefaultVoice?.Locale;
            if (lang is null) return false;
            await Manager.Tts.SpeakWithPan(text, pan, jLoc);
            return true;
        }
    }
}
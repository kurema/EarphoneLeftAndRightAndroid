using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Speech.Tts;
using System.Threading.Tasks;
using Java.Interop;

namespace EarphoneLeftAndRight.Droid.Manager
{
    //TextToSpeechと名前被りを避けるためTts
    //Tts not TextToSpeech in order to avoid same name.
    public static class Tts
    {
        static TtsListner listner;

        private static TextToSpeech _Content;
        public static TextToSpeech Content => _Content ??= new TextToSpeech(Application.Context, listner = new TtsListner());

        public static void StopIfSpeaking()
        {
            if (Content.IsSpeaking) Content.Stop();
        }

        public static async Task SpeakLeft()
        {
            await SpeakWithPan("Left", -1.0f, Java.Util.Locale.English);
        }

        public static async Task SpeakRight()
        {
            await SpeakWithPan("Right", 1.0f, Java.Util.Locale.English);
        }


        public static async Task SpeakWithPan(string text, float paramPan, Java.Util.Locale locale)
        {
            await listner.SemaphoreWaitAsync();
            Content.SetLanguage(locale);
            var bundle = new Bundle();
            if (paramPan != 0.0f) bundle.PutFloat(TextToSpeech.Engine.KeyParamPan, paramPan);
            bundle.PutFloat(TextToSpeech.Engine.KeyParamVolume, 1.0f);
            Content.Speak(text, QueueMode.Add, bundle, Guid.NewGuid().ToString());
        }

        private class TtsListner : Activity, TextToSpeech.IOnInitListener
        {
            static System.Threading.SemaphoreSlim semaphoreSlim;

            public async Task SemaphoreWaitAsync()
            {
                await semaphoreSlim.WaitAsync();
                semaphoreSlim.Release();
            }

            public TtsListner()
            {
                semaphoreSlim = new System.Threading.SemaphoreSlim(0, 1);
            }

            public void OnInit([GeneratedEnum] OperationResult status)
            {
                semaphoreSlim.Release(1);
            }
        }
    }
}
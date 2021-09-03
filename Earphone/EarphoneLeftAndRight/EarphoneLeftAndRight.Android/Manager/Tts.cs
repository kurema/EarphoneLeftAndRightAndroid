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
    public static class Tts
    {
        static TtsListner listner;

        private static TextToSpeech _Content;
        public static TextToSpeech Content => _Content ??= new TextToSpeech(Application.Context, listner = new TtsListner());

        public static void StopIfSpeaking()
        {
            if (Content.IsSpeaking) Content.Stop();
        }

        public static async Task SpeakWithPan(string text, float paramPan,Java.Util.Locale locale)
        {
            await listner.SemaphoreWaitAsync();
            Content.SetLanguage(locale);
            var bundle = new Bundle();
            bundle.PutFloat(TextToSpeech.Engine.KeyParamPan, paramPan);
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
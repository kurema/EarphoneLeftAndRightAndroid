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

namespace EarphoneLeftAndRight.Droid.Manager
{
    public static class Tts
    {
        public static void SpeakWithPan(TextToSpeech tts,string text, float paramPan)
        {
            var bundle = new Bundle();
            bundle.PutFloat(TextToSpeech.Engine.KeyParamPan, paramPan);
            bundle.PutFloat(TextToSpeech.Engine.KeyParamVolume, 1.0f);
            tts.Speak(text, QueueMode.Add, bundle, Guid.NewGuid().ToString());
        }

        public static void SpeakLeft(TextToSpeech tts)
        {
        }
    }
}
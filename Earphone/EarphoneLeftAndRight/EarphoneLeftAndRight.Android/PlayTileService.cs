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

using Android.Service.QuickSettings;
using Java.Interop;

namespace EarphoneLeftAndRight.Droid
{
    [Service(Name = "com.github.kurema.earphoneleftandright.PlayService",
        Permission = Android.Manifest.Permission.BindQuickSettingsTile,
        Label = "@string/tile_play_name",
        Icon = "@drawable/ic_tile_play")]
    [IntentFilter(new[] { ActionQsTile })]
    public class PlayTileService : TileService
    {
        //https://devblogs.microsoft.com/xamarin/android-nougat-quick-setting-tiles/

        TextToSpeech TextToSpeech;

        public override void OnClick()
        {
            base.OnClick();

            var tts = TextToSpeech ??= new TextToSpeech(ApplicationContext, null);

            if (tts.IsSpeaking) { tts.Stop(); }

            tts.SetLanguage(Java.Util.Locale.English);
            tts.SetSpeechRate(1.0f);
            tts.SetPitch(1.0f);

            void SpeakWithPan(string text,float paramPan)
            {
                var bundle = new Bundle();
                bundle.PutFloat(TextToSpeech.Engine.KeyParamPan, paramPan);
                bundle.PutFloat(TextToSpeech.Engine.KeyParamVolume, 1.0f);

                tts.Speak(text, QueueMode.Add, bundle, Guid.NewGuid().ToString());
            }
            SpeakWithPan("Left", -1);
            SpeakWithPan("Right", 1);
        }
    }
}
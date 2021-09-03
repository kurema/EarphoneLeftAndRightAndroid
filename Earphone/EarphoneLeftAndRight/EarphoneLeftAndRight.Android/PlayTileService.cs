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

        public override async void OnClick()
        {
            base.OnClick();

            Manager.Tts.StopIfSpeaking();

            await Manager.Tts.SpeakWithPan("Left", -1, Java.Util.Locale.English);
            await Manager.Tts.SpeakWithPan("Right", 1, Java.Util.Locale.English);
        }
    }
}
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
        Icon = "@drawable/outline_earbuds_24", Exported = true)]
    [IntentFilter(new[] { ActionQsTile })]
    public class PlayTileService : TileService
    {
        //https://devblogs.microsoft.com/xamarin/android-nougat-quick-setting-tiles/

        public override async void OnClick()
        {
            base.OnClick();

            Manager.Tts.StopIfSpeaking();
            await Manager.Tts.SpeakLeft();
            await Manager.Tts.SpeakRight();
        }

        public override void OnStartListening()
        {
            base.OnStartListening();

            // Load tts engine when the user swipes down.
            // This shold improve the lag.
            //https://devblogs.microsoft.com/xamarin/android-nougat-quick-setting-tiles/
            _ = Manager.Tts.Content;
        }
    }
}
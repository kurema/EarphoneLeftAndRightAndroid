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
using Xamarin.Forms;
using Android.Net.Wifi.P2p;
using EarphoneLeftAndRight.Dependency;

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

		static IAudioTest _AudioTest;

		public static IAudioTest AudioTest
		{
			get
			{
				try
				{
					return Storages.AudioStorage.AudioTest;
				}
				catch
				{
					//ここはアプリの起動前に呼ばれる場合がある。
					//その場合、Xamarin.Forms.Init()を呼ぶ前なのでDependencyServiceを呼べない。
					//https://qiita.com/ta-yamaoka/items/b118fa1315f2b9dc87a9
					//普段はセマフォの関係でStorages.AudioStorage.AudioTestを使った方が良い。
					return _AudioTest ??= new AudioTestDependency();
				}
			}
		}


		public override async void OnClick()
		{
			base.OnClick();

			Storages.StatusBarManagerStorage.IsTileAdded = true;

			if (Storages.ConfigStorage.TileForceBeep.Value)
			{
				var audioTest = AudioTest;
				var sample = Storages.AudioStorage.HiDefSupported192kHz ? 192000 : (Storages.AudioStorage.HiDefSupported096kHz ? 96000 : 44100);
				double duration = 0.2;
				double freqL = 440;
				double freqR = 880;
				double easing = 0.05;
				//duration*freqLが整数なので実際にはポップノイズ対策処理は不必要。
				//ここではフェードアウトでノイズ対策をしている。独特な感じがする。
				await audioTest.Register((sample, actualSampleRate, channel) =>
				{
					double t = (double)(sample) / actualSampleRate;
					double[] amplifications;
					if (sample < duration * actualSampleRate * 2)
					{
						amplifications = new[] { Math.Max(Math.Min(1, 1 - (t - duration) / easing), 0), 0 };
						return (amplifications[channel] * Math.Sin(2.0 * Math.PI * freqL * t), false);
					}
					else
					{
						amplifications = new[] { 0, Math.Max(Math.Min(1, 1 - (t - duration * 3) / easing), 0) };
						return (amplifications[channel] * Math.Sin(2.0 * Math.PI * freqR * (t - duration * 2)), false);
					}

					//if (sample < actualSampleRate * Math.Floor(duration * freqL * 2) / freqL / 2)
					//{
					//	amplifications = new[] { 1, 0 };
					//	return (amplifications[channel] * Math.Sin(2.0 * Math.PI * freqL * t), false);
					//}
					//else if (sample < duration * actualSampleRate * 2)
					//{
					//	return (0, false);
					//}
					//else if (sample < actualSampleRate * (duration * 2 + Math.Floor(duration * freqR * 2) / freqR / 2))
					//{
					//	amplifications = new[] { 0, 1 };
					//	return (amplifications[channel] * Math.Sin(2.0 * Math.PI * freqR * (t - duration * 2)), false);
					//}
					//else
					//{
					//	var temp = Math.Sin(2.0 * Math.PI * freqR * (t - duration * 2));
					//	return (0, false);
					//}
				}, duration * 3 + easing, sample);
				await System.Threading.Tasks.Task.Run(() => { try { audioTest.Play(); } catch { } });
			}
			else
			{
				await PlayDummyAudioIfRequired();
				await Manager.Tts.SpeakLeftRight();
			}
		}

		public static async System.Threading.Tasks.Task PlayDummyAudioIfRequired()
		{
			try
			{
				if (!AudioTest.IsPlaying)
				{
					await AudioTest.Register((sample, actualSampleRate, channel) => (0.0, false), 5.0 / 44100);
					try { AudioTest.Play(); } catch { }
				}
			}
			catch { }
		}

		public override async void OnStartListening()
		{
			base.OnStartListening();

			// Load tts engine when the user swipes down.
			// This shold improve the lag.
			//https://devblogs.microsoft.com/xamarin/android-nougat-quick-setting-tiles/
			Storages.StatusBarManagerStorage.IsTileAdded = true;
			await Manager.Tts.Content.Initialize();
		}

		public override void OnTileAdded()
		{
			Storages.StatusBarManagerStorage.IsTileAdded = true;
			base.OnTileAdded();
		}

		public override void OnTileRemoved()
		{
			Storages.StatusBarManagerStorage.IsTileAdded = false;
			base.OnTileRemoved();
		}
	}
}
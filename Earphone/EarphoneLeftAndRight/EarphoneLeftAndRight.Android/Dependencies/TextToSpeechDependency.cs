using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EarphoneLeftAndRight.Dependency;
using EarphoneLeftAndRight.Droid.Manager;
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
			try { Manager.Tts.Content.Stop(); } catch { }
			return Task.CompletedTask;
		}

		public async Task SpeakLeftAsync()
		{
			try { await Manager.Tts.SpeakLeft(); } catch { }
		}

		public async Task SpeakRightAsync()
		{
			try { await Manager.Tts.SpeakRight(); } catch { }
		}

		public async Task SpeakLeftRightAsync()
		{
			try { await Manager.Tts.SpeakLeftRight(); } catch { }
		}

		public void Load() => Manager.Tts.Content?.Initialize();

		public async Task SpeakAsync(string text, TextToSpeechOptions optionOverride)
		{
			try { await Tts.SpeakAsync(text, optionOverride); } catch { }
		}

		public async Task SpeakAsync((string text, TextToSpeechOptions optionOverride)[] values)
		{
			try { await Tts.SpeakAsync(values); } catch { }
		}
	}
}
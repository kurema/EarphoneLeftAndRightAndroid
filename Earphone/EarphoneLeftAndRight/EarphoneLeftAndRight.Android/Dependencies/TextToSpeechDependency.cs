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
			Manager.Tts.Content.Stop();
			return Task.CompletedTask;
		}

		public async Task SpeakLeftAsync()
		{
			await Manager.Tts.SpeakLeft();
		}

		public async Task SpeakRightAsync()
		{
			await Manager.Tts.SpeakRight();
		}

		public async Task SpeakLeftRightAsync()
		{
			await Manager.Tts.SpeakLeftRight();
		}

		public void Load() => Manager.Tts.Content?.Initialize();

		public async Task SpeakAsync(string text, TextToSpeechOptions optionOverride)
		{
			await Tts.SpeakAsync(text, optionOverride);
		}

	}
}
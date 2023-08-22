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
using EarphoneLeftAndRight.Dependency;

namespace EarphoneLeftAndRight.Droid.Manager
{
	//TextToSpeechと名前被りを避けるためTts
	//Tts not TextToSpeech in order to avoid same name.
	public static class Tts
	{

		static TtsEx.TextToSpeechImplementation _content;

		public static TtsEx.TextToSpeechImplementation Content => _content ??= new TtsEx.TextToSpeechImplementation();


		static Dependency.TextToSpeechOptions GetCurrentOption()
		{
			return new Dependency.TextToSpeechOptions(null, Storages.ConfigStorage.VoicePitch.Value, Storages.ConfigStorage.VoiceVolume.Value, null);
		}


		public static async Task SpeakLeft()
		{
			var option = GetCurrentOption();
			option.Pan = -Storages.ConfigStorage.VoicePan.Value;
			if (Storages.ConfigStorage.VoiceForeceEnglish.Value)
			{
				var en = Java.Util.Locale.English;
				option.Locale = new Dependency.TextToSpeechLocale(en?.Language, en?.Country, en?.DisplayName, string.Empty);
				await Content.SpeakAsync(new (string Text, Dependency.TextToSpeechOptions Options)[] { ("Left", option) });
			}
			else
			{
				var (locale, text) = await Content.GetLocalizedTextAsync(Resource.String.word_left, Resource.String.word_left_voice);
				await Content.SpeakAsync(new (string Text, Dependency.TextToSpeechOptions Options)[] { });
				//await SpeakWithPan(local.Item2, +1.0f, local.Item2.ToUpperInvariant() == "RIGHT" && Content.IsLanguageAvailable(Java.Util.Locale.English) >= LanguageAvailableResult.Available ? Java.Util.Locale.English : local.Item1);
			}
		}

		public static async Task SpeakRight()
		{
			var local = await Content.GetLocalizedTextAsync(Resource.String.word_right, Resource.String.word_right_voice);

		}

		public static async Task SpeakLeftRight()
		{

		}

		public static Task<bool> Initialize() => Content.Initialize();

		public static async Task SpeakAsync(string text, TextToSpeechOptions optionOverride)
		{
			var option = GetCurrentOption();
			await Content.SpeakAsync(new (string Text, TextToSpeechOptions Options)[] { (text, new TextToSpeechOptions(optionOverride?.Locale ?? option?.Locale, optionOverride?.Pitch ?? option?.Pitch, optionOverride?.Volume ?? option?.Volume, optionOverride?.Pan ?? option?.Pan)) });
		}
	}
}
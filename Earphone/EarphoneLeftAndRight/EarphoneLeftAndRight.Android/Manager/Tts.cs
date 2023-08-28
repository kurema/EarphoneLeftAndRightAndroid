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
using EarphoneLeftAndRight.Droid.Manager.TtsEx;
using static System.Net.Mime.MediaTypeNames;
using static Android.Provider.UserDictionary;
using Android.Gms.Auth.Api.SignIn.Internal;

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
			return new Dependency.TextToSpeechOptions(null, Storages.ConfigStorage.VoicePitch.Value, Storages.ConfigStorage.VoiceVolume.Value, null, Storages.ConfigStorage.VoiceSpeed.Value);
		}


		public static async Task SpeakLeft()
		{
			await SpeakLeftOrRightCommon(LeftRight.Left);
		}


		private enum LeftRight
		{
			Left = 1, Right = 2
		}

		static async Task SpeakLeftOrRightCommon(LeftRight leftRight)
		{
			var option = GetCurrentOption();
			option.Pan = Storages.ConfigStorage.VoicePan.Value * (leftRight switch
			{
				LeftRight.Left => -1,
				LeftRight.Right => 1,
				_ => 0,
			});
			string word = leftRight switch { LeftRight.Left => "Left", LeftRight.Right => "Right", _ => string.Empty };
			if (Storages.ConfigStorage.VoiceForeceEnglish.Value) goto English;

			static async Task<(Java.Util.Locale, string)> GetLocalizedTextLocalAsync(Storages.ConfigStorage.ConfigEntryString entry, int key1, int key2)
			{
				var (locale, text) = await Content.GetLocalizedTextAsync(key1, key2);
				return string.IsNullOrWhiteSpace(entry.Value) ? (locale, text) : (locale, entry.Value);
			}

			var (locale, text) = leftRight switch
			{
				LeftRight.Left => await GetLocalizedTextLocalAsync(Storages.ConfigStorage.VoiceOverrideLeft, Resource.String.word_left, Resource.String.word_left_voice),
				LeftRight.Right => await GetLocalizedTextLocalAsync(Storages.ConfigStorage.VoiceOverrideRight, Resource.String.word_right, Resource.String.word_right_voice),
				_ => (null, string.Empty),
			};
			if (text.Equals(word, StringComparison.InvariantCultureIgnoreCase) && (await Content.GetLocalesAsync()).Any(a => a.Language == "English")) goto English;
			option.Locale = TextToSpeechImplementation.GetLocaleFromJavaLocale(locale);
			await Content.SpeakAsync(new (string Text, Dependency.TextToSpeechOptions Options)[] { (text, option) });
			//await SpeakWithPan(local.Item2, +1.0f, local.Item2.ToUpperInvariant() == "RIGHT" && Content.IsLanguageAvailable(Java.Util.Locale.English) >= LanguageAvailableResult.Available ? Java.Util.Locale.English : local.Item1);
			return;

		English:;
			await SpeakWordInEnglish(word, option);
			return;

		}

		private static async Task SpeakWordInEnglish(string text, TextToSpeechOptions options)
		{
			var op = new TextToSpeechOptions(options);
			op.Locale = TextToSpeechImplementation.GetLocaleFromJavaLocale(Java.Util.Locale.English);
			await Content.SpeakAsync(new (string Text, Dependency.TextToSpeechOptions Options)[] { (text, op) });
		}

		public static async Task SpeakRight()
		{
			await SpeakLeftOrRightCommon(LeftRight.Right);
		}

		public static async Task SpeakLeftRight()
		{
			var option = GetCurrentOption();
			option.Pan = Storages.ConfigStorage.VoicePan.Value;
			if (Storages.ConfigStorage.VoiceForeceEnglish.Value) goto English;
			{
				var (locale, textLeft) = await Content.GetLocalizedTextAsync(Resource.String.word_left, Resource.String.word_left_voice);
				var (_, textRight) = await Content.GetLocalizedTextAsync(Resource.String.word_right, Resource.String.word_right_voice);
				var (textLeftOr, textRightOr) = (Storages.ConfigStorage.VoiceOverrideLeft.Value, Storages.ConfigStorage.VoiceOverrideRight.Value);
				textLeft = string.IsNullOrWhiteSpace(textLeftOr) ? textLeft : textLeftOr;
				textRight = string.IsNullOrWhiteSpace(textRightOr) ? textRight : textRightOr;
				option.Locale = TextToSpeechImplementation.GetLocaleFromJavaLocale(locale);
				var optionLeft = new TextToSpeechOptions(option);
				optionLeft.Pan = -option.Pan;
				if (textLeft.Equals("Left", StringComparison.InvariantCultureIgnoreCase) && (await Content.GetLocalesAsync()).Any(a => a.Language == "English")) goto English;
				await Content.SpeakAsync(new (string Text, Dependency.TextToSpeechOptions Options)[] { (textLeft, optionLeft), (textRight, option) });
				return;
			}

		English:;
			{
				option.Locale = TextToSpeechImplementation.GetLocaleFromJavaLocale(Java.Util.Locale.English);
				var optionLeft = new TextToSpeechOptions(option);
				optionLeft.Pan = -option.Pan;
				await Content.SpeakAsync(new (string Text, Dependency.TextToSpeechOptions Options)[] { ("Left", optionLeft), ("Right", option) });
				return;
			}
		}

		public static Task<bool> Initialize() => Content.Initialize();

		public static async Task SpeakAsync(string text, TextToSpeechOptions optionOverride)
		{
			var option = GetCurrentOption();
			await Content.SpeakAsync(new (string Text, TextToSpeechOptions Options)[] { (text, new TextToSpeechOptions(optionOverride?.Locale ?? option?.Locale, optionOverride?.Pitch ?? option?.Pitch, optionOverride?.Volume ?? option?.Volume, optionOverride?.Pan ?? option?.Pan, optionOverride?.SpeechRate ?? option.SpeechRate)) });
		}
	}
}
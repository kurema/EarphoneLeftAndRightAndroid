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
	//TextToSpeechと名前被りを避けるためTts
	//Tts not TextToSpeech in order to avoid same name.
	public static class Tts
	{
		static TtsListner listner;

		private static TextToSpeech _Content;
		public static TextToSpeech Content { get => _Content ??= new TextToSpeech(Application.Context, listner = new TtsListner()); private set { _Content = value; } }

		private static int ResetCount = 0;

		internal static void ResetEngine()
		{
			if (ResetCount < 3) Content = null;
			ResetCount++;
		}

		public static void StopIfSpeaking()
		{
			if (Content.IsSpeaking) Content.Stop();
		}

		public static (Java.Util.Locale, string) GetLocalized(int key1, int key2)
		{
			//TTSエンジンがサポートしてる言語の翻訳を優先する。
			//strings.xmlがその言語を対応しているのか見分ける方法は良く分からない。
			//If TextToSpeech engine support the language in your preference list, we use the string of the locale.
			//It seems there's no way to know which strings.xml is used. (or embbed lang code as string).
			//https://stackoverflow.com/questions/68227872/get-fallback-locale-when-device-locale-strings-xml-is-not-present
			Java.Util.Locale[] locales;
			if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
			{
				var localesLL = Application.Context.Resources.Configuration.Locales;

				locales = new Java.Util.Locale[localesLL.Size() + 1];
				for (int i = 0; i < localesLL.Size(); i++)
				{
					locales[i] = localesLL.Get(i);
				}
				locales[localesLL.Size()] = Java.Util.Locale.English;
			}
			else
			{
				//旧APIをサポート。機能しています。
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
				locales = new[] { Application.Context.Resources.Configuration.Locale, Java.Util.Locale.English };
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
			}

			foreach (var item in locales)
			{
				if (Content.IsLanguageAvailable(item) < LanguageAvailableResult.Available) continue;

				{
					var config = new Android.Content.Res.Configuration(Application.Context.Resources.Configuration);
					config.SetLocale(item);
					var context = Application.Context.CreateConfigurationContext(config);
					var text2 = context.GetString(key2);
					return (item, string.IsNullOrWhiteSpace(text2) ? context.GetString(key1) : text2);
				}
			}
			{
				var text2 = Application.Context.GetString(key2);
				return (null, string.IsNullOrWhiteSpace(text2) ? Application.Context.GetString(key1) : text2);
			}
		}

		public static async Task SpeakLeft()
		{
			var local = GetLocalized(Resource.String.word_left, Resource.String.word_left_voice);
			await SpeakWithPan(local.Item2, -1.0f, local.Item2.ToUpperInvariant() == "LEFT" && Content.IsLanguageAvailable(Java.Util.Locale.English) >= LanguageAvailableResult.Available ? Java.Util.Locale.English : local.Item1);
		}

		public static async Task SpeakRight()
		{
			var local = GetLocalized(Resource.String.word_right, Resource.String.word_right_voice);
			await SpeakWithPan(local.Item2, +1.0f, local.Item2.ToUpperInvariant() == "RIGHT" && Content.IsLanguageAvailable(Java.Util.Locale.English) >= LanguageAvailableResult.Available ? Java.Util.Locale.English : local.Item1);
		}

		public static async Task WaitReadyAsync()
		{
			await listner.SemaphoreWaitAsync();
		}

		public static async Task SpeakWithPan(string text, float paramPan, Java.Util.Locale locale)
		{
			//https://stackoverflow.com/questions/76319978/text-to-speech-not-reading-some-words-on-android-14
			await listner.SemaphoreWaitAsync();
			if (!(locale is null)) Content.SetLanguage(locale);
			//Content.PlaySilentUtterance(1, QueueMode.Add, Guid.NewGuid().ToString());
			Content.SetSpeechRate(1.0f);
			Content.SetPitch(1.0f);

			var map = new Dictionary<string, string>(StringComparer.Ordinal)
			{
				{ TextToSpeech.Engine.KeyParamVolume, "1.0"},
				{ TextToSpeech.Engine.KeyParamUtteranceId, Guid.NewGuid().ToString() },
			};
			if (paramPan != 0.0f) map.Add(TextToSpeech.Engine.KeyParamPan, paramPan.ToString(System.Globalization.CultureInfo.InvariantCulture));
			//Maui Essentials use this to support older API levels at runtime. New version worked on Android 5 but anyway.
			//https://github.com/dotnet/maui/blob/main/src/Essentials/src/TextToSpeech/TextToSpeech.android.cs
#pragma warning disable CS0618
			Content.Speak(text, QueueMode.Add, map);
#pragma warning restore CS0618
			//System.Diagnostics.Debug.WriteLine(result);

			//var bundle = new Bundle();
			//if (paramPan != 0.0f) bundle.PutFloat(TextToSpeech.Engine.KeyParamPan, paramPan);
			//bundle.PutFloat(TextToSpeech.Engine.KeyParamVolume, 1.0f);
			//Content.Speak(text, QueueMode.Add, bundle, Guid.NewGuid().ToString());
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
				switch (status)
				{
					case OperationResult.Error:
					case OperationResult.Stopped:
						ResetEngine();
						break;
					case OperationResult.Success:
						semaphoreSlim.Release(1);
						break;
					default:
						break;
				}
			}
		}
	}
}
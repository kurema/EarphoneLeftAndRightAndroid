using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Gms.Ads.Formats;
using Android.OS;
using Android.Speech.Tts;
using EarphoneLeftAndRight.Dependency;
using Xamarin.Essentials;
using AndroidTextToSpeech = Android.Speech.Tts.TextToSpeech;
using Debug = System.Diagnostics.Debug;
using JavaLocale = Java.Util.Locale;

//Based on Maui.
//https://github.com/dotnet/maui/blob/main/src/Essentials/src/TextToSpeech/TextToSpeech.android.cs

#nullable enable
namespace EarphoneLeftAndRight.Droid.Manager.TtsEx
{
	public partial class TextToSpeechImplementation
	{
		public static TextToSpeechLocale GetLocaleFromJavaLocale(JavaLocale locale) => new TextToSpeechLocale(locale.Language, locale.Country, locale.DisplayName, string.Empty);

		SemaphoreSlim? semaphore;

		public Task<IEnumerable<TextToSpeechLocale>> GetLocalesAsync() => PlatformGetLocalesAsync();

		public async Task SpeakAsync((string Text, TextToSpeechOptions Options)[] words, CancellationToken cancelToken = default)
		{
			foreach (var (text, options) in words)
			{
				if (string.IsNullOrEmpty(text))
					throw new ArgumentNullException(nameof(text), "Text cannot be null or empty string");

				if (options?.Volume.HasValue ?? false)
				{
					if (options.Volume.Value < TextToSpeechOptions.VolumeMin || options.Volume.Value > TextToSpeechOptions.VolumeMax)
						throw new ArgumentOutOfRangeException($"Volume must be >= {TextToSpeechOptions.VolumeMin} and <= {TextToSpeechOptions.VolumeMax}");
				}

				if (options?.Pitch.HasValue ?? false)
				{
					if (options.Pitch.Value < TextToSpeechOptions.PitchMin || options.Pitch.Value > TextToSpeechOptions.PitchMax)
						throw new ArgumentOutOfRangeException($"Pitch must be >= {TextToSpeechOptions.PitchMin} and <= {TextToSpeechOptions.PitchMin}");
				}
			}

			semaphore ??= new SemaphoreSlim(1, 1);

			try
			{
				await semaphore.WaitAsync(cancelToken);
				await PlatformSpeakAsync(words, cancelToken);
			}
			finally
			{
				if (semaphore.CurrentCount == 0)
					semaphore.Release();
			}
		}
	}
#nullable restore

	public partial class TextToSpeechImplementation
	{
		//const int maxSpeechInputLengthDefault = 4000;

		WeakReference<TextToSpeechInternalImplementation> textToSpeechRef = null;

		TextToSpeechInternalImplementation GetTextToSpeech()
		{
			if (textToSpeechRef == null || !textToSpeechRef.TryGetTarget(out var tts))
			{
				tts = new TextToSpeechInternalImplementation();
				textToSpeechRef = new WeakReference<TextToSpeechInternalImplementation>(tts);
			}

			return tts;
		}
		const string NotSupportedMessage = "Unable to start text-to-speech engine, not supported on device.";

		public async Task<bool> Initialize()
		{
			var textToSpeech = GetTextToSpeech() ?? throw new PlatformNotSupportedException(NotSupportedMessage);
			return await textToSpeech.Initialize();
		}

		public async Task<(JavaLocale, string)> GetLocalizedTextAsync(int key1, int key2)
		{
			var textToSpeech = GetTextToSpeech() ?? throw new PlatformNotSupportedException(NotSupportedMessage);
			return await textToSpeech.GetLocalizedTextAsync(key1, key2);
		}

		public bool IsSpeaking
		{
			get
			{
				var textToSpeech = GetTextToSpeech() ?? throw new PlatformNotSupportedException(NotSupportedMessage);
				return textToSpeech.IsSpeaking;
			}
		}

		Task PlatformSpeakAsync((string Text, TextToSpeechOptions Options)[] words, CancellationToken cancelToken)
		{
			var textToSpeech = GetTextToSpeech() ?? throw new PlatformNotSupportedException(NotSupportedMessage);
			//var max = maxSpeechInputLengthDefault;
			//if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr2)
			//	max = AndroidTextToSpeech.MaxSpeechInputLength;

			return textToSpeech.SpeakAsync(words, cancelToken);
		}

		Task<IEnumerable<TextToSpeechLocale>> PlatformGetLocalesAsync()
		{
			var textToSpeech = GetTextToSpeech();
			return textToSpeech == null
				? throw new PlatformNotSupportedException(NotSupportedMessage)
				: textToSpeech.GetLocalesAsync();
		}

		public bool Stop()
		{
			var textToSpeech = GetTextToSpeech() ?? throw new PlatformNotSupportedException(NotSupportedMessage);
			return textToSpeech.Stop();
		}
	}

	class TextToSpeechInternalImplementation : Java.Lang.Object, AndroidTextToSpeech.IOnInitListener,
#pragma warning disable CS0618
		AndroidTextToSpeech.IOnUtteranceCompletedListener
#pragma warning restore CS0618
	{
		AndroidTextToSpeech tts;
		TaskCompletionSource<bool> tcsInitialize;
		TaskCompletionSource<bool> tcsUtterances;

		public Task<bool> Initialize()
		{
			if (tcsInitialize != null && tts != null)
				return tcsInitialize.Task;

			tcsInitialize = new TaskCompletionSource<bool>();
			try
			{
				// set up the TextToSpeech object
				tts = new AndroidTextToSpeech(Application.Context, this);
#pragma warning disable CS0618
				tts.SetOnUtteranceCompletedListener(this);
#pragma warning restore CS0618

			}
			catch (Exception e)
			{
				tcsInitialize.TrySetException(e);
			}

			return tcsInitialize.Task;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				tts?.Stop();
				tts?.Shutdown();
				tts = null;
			}

			base.Dispose(disposing);
		}

		int numExpectedUtterances = 0;
		int numCompletedUtterances = 0;

		public async Task SpeakAsync((string Text, TextToSpeechOptions Options)[] words, CancellationToken cancelToken)
		{
			await Initialize();

			// Wait for any previous calls to finish up
			if (tcsUtterances?.Task != null)
				await tcsUtterances.Task;

			tcsUtterances = new TaskCompletionSource<bool>();

			if (cancelToken != default)
			{
				cancelToken.Register(() =>
				{
					try
					{
						tts?.Stop();

						tcsUtterances?.TrySetResult(true);
					}
					catch
					{
					}
				});
			}

			int i = 0;
			foreach (var (text, options) in words)
			{
				if (options?.Locale?.Language != null)
				{
					JavaLocale locale = null;
					if (!string.IsNullOrWhiteSpace(options?.Locale.Country))
						locale = new JavaLocale(options.Locale.Language, options.Locale.Country);
					else
						locale = new JavaLocale(options.Locale.Language);

					tts.SetLanguage(locale);
				}
				else
				{
					SetDefaultLanguage();
				}

				if (options?.Pitch.HasValue ?? false)
					tts.SetPitch(options.Pitch.Value);
				else
					tts.SetPitch(1.0f);

				if (options?.SpeechRate.HasValue ?? false)
					tts.SetSpeechRate(options.SpeechRate.Value);
				else
					tts.SetSpeechRate(1.0f);

				var guid = Guid.NewGuid().ToString();

				{
					// We require the utterance id to be set if we want the completed listener to fire
					var map = new Dictionary<string, string>(StringComparer.Ordinal)
				{
					{ AndroidTextToSpeech.Engine.KeyParamUtteranceId, $"{guid}" }
				};

					if (options != null && options.Volume.HasValue)
						map.Add(AndroidTextToSpeech.Engine.KeyParamVolume, options.Volume.Value.ToString(CultureInfo.InvariantCulture));

					if (options?.Pan.HasValue ?? false)
						map.Add(AndroidTextToSpeech.Engine.KeyParamPan, options.Pan.Value.ToString(CultureInfo.InvariantCulture));

					// We use an obsolete overload here so it works on older API levels at runtime
					// Flush on first entry and add (to not flush our own previous) subsequent entries
#pragma warning disable CS0618
					tts.Speak(text, i == 0 ? QueueMode.Flush : QueueMode.Add, map);
#pragma warning restore CS0618
				}
				i++;
			}

			await tcsUtterances.Task;
		}

		public void OnInit(OperationResult status)
		{
			if (status == OperationResult.Success)
				tcsInitialize.TrySetResult(true);
			else
				tcsInitialize.TrySetException(new ArgumentException("Failed to initialize Text to Speech engine."));
		}

		public async Task<IEnumerable<TextToSpeechLocale>> GetLocalesAsync()
		{
			await Initialize();

			try
			{
				return tts.AvailableLanguages.Select(a => new TextToSpeechLocale(a.Language, a.Country, a.DisplayName, string.Empty));
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Unable to query language on new API, attempting older api: " + ex);
			}

			return JavaLocale.GetAvailableLocales()
				.Where(IsLocaleAvailable)
				.Select(l => new TextToSpeechLocale(l.Language, l.Country, l.DisplayName, string.Empty))
				.GroupBy(c => c.ToString())
				.Select(g => g.First());
		}

		bool IsLocaleAvailable(JavaLocale l)
		{
			try
			{
				var r = tts.IsLanguageAvailable(l);
				return
					r == LanguageAvailableResult.Available ||
					r == LanguageAvailableResult.CountryAvailable ||
					r == LanguageAvailableResult.CountryVarAvailable;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Error checking language; " + l + " " + ex);
			}
			return false;
		}

		public void OnUtteranceCompleted(string utteranceId)
		{
			numCompletedUtterances++;
			if (numCompletedUtterances >= numExpectedUtterances)
				tcsUtterances?.TrySetResult(true);
		}

#pragma warning disable 0618
		void SetDefaultLanguage()
		{
			try
			{
				if (tts.DefaultLanguage == null && tts.Language != null)
					tts.SetLanguage(tts.Language);
				else if (tts.DefaultLanguage != null)
					tts.SetLanguage(tts.DefaultLanguage);
			}
			catch
			{
				if (tts.Language != null)
					tts.SetLanguage(tts.Language);
			}
		}
#pragma warning restore 0618

		//kurema追加分
		public async Task<(JavaLocale, string)> GetLocalizedTextAsync(int key1, int key2)
		{
			await Initialize();

			//TTSエンジンがサポートしてる言語の翻訳を優先する。
			//strings.xmlがその言語を対応しているのか見分ける方法は良く分からない。
			//If TextToSpeech engine support the language in your preference list, we use the string of the locale.
			//It seems there's no way to know which strings.xml is used. (or embbed lang code as string).
			//https://stackoverflow.com/questions/68227872/get-fallback-locale-when-device-locale-strings-xml-is-not-present
			JavaLocale[] locales;
			if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
			{
				var localesLL = Application.Context.Resources.Configuration.Locales;

				locales = new JavaLocale[localesLL.Size() + 1];
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
				if (tts.IsLanguageAvailable(item) < LanguageAvailableResult.Available) continue;

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

		//kurema追加分
		public bool IsSpeaking => tts?.IsSpeaking ?? false;

		public bool Stop()
		{
			return tts?.Stop() switch
			{
				OperationResult.Error => false,
				OperationResult.Stopped => true,
				OperationResult.Success => true,
				null => true,
				_ => false,
			};
		}
	}
}

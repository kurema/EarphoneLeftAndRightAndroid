using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Speech.Tts;
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
		internal const float PitchMax = 2.0f;
		internal const float PitchDefault = 1.0f;
		internal const float PitchMin = 0.0f;

		internal const float VolumeMax = 1.0f;
		internal const float VolumeDefault = 0.5f;
		internal const float VolumeMin = 0.0f;

		SemaphoreSlim? semaphore;

		public Task<IEnumerable<Dependency.TextToSpeechLocale>> GetLocalesAsync() => PlatformGetLocalesAsync();

		public async Task SpeakAsync((string Text, Dependency.TextToSpeechOptions Options)[] words, CancellationToken cancelToken = default)
		{
			foreach (var (text, options) in words)
			{
				if (string.IsNullOrEmpty(text))
					throw new ArgumentNullException(nameof(text), "Text cannot be null or empty string");

				if (options?.Volume.HasValue ?? false)
				{
					if (options.Volume.Value < VolumeMin || options.Volume.Value > VolumeMax)
						throw new ArgumentOutOfRangeException($"Volume must be >= {VolumeMin} and <= {VolumeMax}");
				}

				if (options?.Pitch.HasValue ?? false)
				{
					if (options.Pitch.Value < PitchMin || options.Pitch.Value > PitchMax)
						throw new ArgumentOutOfRangeException($"Pitch must be >= {PitchMin} and <= {PitchMin}");
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
		const int maxSpeechInputLengthDefault = 4000;

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

		Task PlatformSpeakAsync((string Text, Dependency.TextToSpeechOptions Options)[] words, CancellationToken cancelToken)
		{
			var textToSpeech = GetTextToSpeech() ?? throw new PlatformNotSupportedException("Unable to start text-to-speech engine, not supported on device.");
			//var max = maxSpeechInputLengthDefault;
			//if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr2)
			//	max = AndroidTextToSpeech.MaxSpeechInputLength;

			return textToSpeech.SpeakAsync(words, cancelToken);
		}

		Task<IEnumerable<Dependency.TextToSpeechLocale>> PlatformGetLocalesAsync()
		{
			var textToSpeech = GetTextToSpeech();
			return textToSpeech == null
				? throw new PlatformNotSupportedException("Unable to start text-to-speech engine, not supported on device.")
				: textToSpeech.GetLocalesAsync();
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

		Task<bool> Initialize()
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

		public async Task SpeakAsync((string Text, Dependency.TextToSpeechOptions Options)[] words, CancellationToken cancelToken)
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

		public async Task<IEnumerable<Dependency.TextToSpeechLocale>> GetLocalesAsync()
		{
			await Initialize();

			try
			{
				return tts.AvailableLanguages.Select(a => new Dependency.TextToSpeechLocale(a.Language, a.Country, a.DisplayName, string.Empty));
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Unable to query language on new API, attempting older api: " + ex);
			}

			return JavaLocale.GetAvailableLocales()
				.Where(IsLocaleAvailable)
				.Select(l => new Dependency.TextToSpeechLocale(l.Language, l.Country, l.DisplayName, string.Empty))
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
	}
}

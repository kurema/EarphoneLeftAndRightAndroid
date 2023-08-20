using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EarphoneLeftAndRight.Dependency
{
	public interface ITextToSpeech
	{
		bool IsSpeaking { get; }
		Task Clear();
		Task SpeakLeft();
		Task SpeakRight();
		void Load();

		Task<bool> SpeakWithPan(string text, float pan, System.Globalization.CultureInfo cultureInfo);
	}

	public class TextToSpeechLocale
	{
		//Constructer of Xamarin.Essentials is internal.
		public string Language { get; }

		public string Country { get; }

		public string Name { get; }

		public string Id { get; }

		public TextToSpeechLocale(string language, string country, string name, string id)
		{
			Language = language;
			Country = country;
			Name = name;
			Id = id;
		}
	}

	public class TextToSpeechOptions
	{
		public TextToSpeechOptions()
		{
		}

		public TextToSpeechOptions(TextToSpeechOptions original) : this(original.Locale, original.Pitch, original.Volume, original.Pan)
		{
		}

		public TextToSpeechOptions(TextToSpeechLocale? locale, float? pitch, float? volume, float? pan)
		{
			Locale = locale;
			Pitch = pitch;
			Volume = volume;
			Pan = pan;
		}

		public TextToSpeechLocale? Locale { get; set; }

		public float? Pitch { get; set; }

		public float? Volume { get; set; }

		public float? Pan { get; set; }
	}
}

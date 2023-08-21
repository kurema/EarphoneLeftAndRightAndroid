using EarphoneLeftAndRight.Dependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace EarphoneLeftAndRight.Storages;

public static class ConfigStorage
{
	private static ConfigEntryBool? _VoiceForeceEnglish;
	public static ConfigEntryBool VoiceForeceEnglish { get => _VoiceForeceEnglish ??= new ConfigEntryBool("VoiceForceEnglish", false); }

	private static ConfigEntryFloat? _VoicePan;
	public static ConfigEntryFloat VoicePan { get => _VoicePan ??= new ConfigEntryFloat("VoicePan", TextToSpeechOptions.PanAbsDefault); }

	private static ConfigEntryFloat? _VoicePitch;
	public static ConfigEntryFloat VoicePitch { get => _VoicePitch ??= new ConfigEntryFloat("VoicePitch", TextToSpeechOptions.PitchDefault); }

	private static ConfigEntryFloat? _VoiceVolume;
	public static ConfigEntryFloat VoiceVolume { get => _VoiceVolume ??= new ConfigEntryFloat("VoiceVolume", TextToSpeechOptions.VolumeMax); }

	
	public abstract class ConfigEntry<T>
	{
		public ConfigEntry(string key, T defaultValue)
		{
			Key = key ?? throw new ArgumentNullException(nameof(key));
			DefaultValue = defaultValue;
		}

		public string Key { get; }
		public T DefaultValue { get; }

		public abstract T Value { get; set; }

	}

	public class ConfigEntryBool : ConfigEntry<bool>
	{
		public ConfigEntryBool(string key, bool defaultValue) : base(key, defaultValue)
		{
		}

		public override bool Value
		{
			get => Xamarin.Essentials.Preferences.Get(Key, DefaultValue);
			set { Xamarin.Essentials.Preferences.Set(Key, value); }
		}
	}

	public class ConfigEntryFloat : ConfigEntry<float>
	{
		public ConfigEntryFloat(string key, float defaultValue) : base(key, defaultValue)
		{
		}

		public override float Value
		{
			get => Xamarin.Essentials.Preferences.Get(Key, DefaultValue);
			set { Xamarin.Essentials.Preferences.Set(Key, value); }
		}
	}

}

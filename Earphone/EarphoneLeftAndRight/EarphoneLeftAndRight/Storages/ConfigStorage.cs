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

	private static ConfigEntryFloat? _VoiceSpeed;
	public static ConfigEntryFloat VoiceSpeed { get => _VoiceSpeed ??= new ConfigEntryFloat("VoiceSpeed", 1.0f); }

	private static ConfigEntryString? _VoiceOverrideLeft;
	public static ConfigEntryString VoiceOverrideLeft { get => _VoiceOverrideLeft ??= new ConfigEntryString("VoiceOverrideLeft", string.Empty); }

	private static ConfigEntryString? _VoiceOverrideRight;
	public static ConfigEntryString VoiceOverrideRight { get => _VoiceOverrideRight ??= new ConfigEntryString("VoiceOverrideRight", string.Empty); }

	private static ConfigEntryBool? _TileForceBeep;
	public static ConfigEntryBool TileForceBeep { get => _TileForceBeep ??= new ConfigEntryBool("TileForceBeep", false); }


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

	public class ConfigEntryString : ConfigEntry<string>
	{
		public ConfigEntryString(string key, string defaultValue) : base(key, defaultValue)
		{
		}

		public override string Value
		{
			get => Xamarin.Essentials.Preferences.Get(Key, DefaultValue);
			set { Xamarin.Essentials.Preferences.Set(Key, value); }
		}
	}


}

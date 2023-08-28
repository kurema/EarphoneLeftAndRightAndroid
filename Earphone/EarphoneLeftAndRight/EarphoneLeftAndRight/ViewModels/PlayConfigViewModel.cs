using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml.Internals;

namespace EarphoneLeftAndRight.ViewModels
{
	public class PlayConfigViewModel : BaseViewModel
	{
		public PlayConfigViewModel()
		{
			SpeakCommand = new Command((o) => { Save(); PlayViewModel.Speak(o); });
			RestoreCommand = new Command(Restore);
			SaveCommand = new Command(Save);
			ResetValueCommand=new Command((o)=>Reset(o.ToString()));

			Restore();
		}

		public void Save()
		{
			Storages.ConfigStorage.VoiceForeceEnglish.Value = ForceEnglish;
			Storages.ConfigStorage.VoiceVolume.Value = Volume;
			Storages.ConfigStorage.VoicePitch.Value = Pitch;
			Storages.ConfigStorage.VoicePan.Value = Pan;
			Storages.ConfigStorage.VoiceSpeed.Value = SpeechRate;
			Storages.ConfigStorage.VoiceOverrideLeft.Value = OverrideLeft;
			Storages.ConfigStorage.VoiceOverrideRight.Value = OverrideRight;
		}

		public void Restore()
		{
			ForceEnglish = Storages.ConfigStorage.VoiceForeceEnglish.Value;
			Volume = Storages.ConfigStorage.VoiceVolume.Value;
			Pitch = Storages.ConfigStorage.VoicePitch.Value;
			Pan = Storages.ConfigStorage.VoicePan.Value;
			SpeechRate = Storages.ConfigStorage.VoiceSpeed.Value;
			OverrideLeft = Storages.ConfigStorage.VoiceOverrideLeft.Value;
			OverrideRight = Storages.ConfigStorage.VoiceOverrideRight.Value;
		}

		public void Reset(string text)
		{
			switch (text)
			{
				case "Volume": Volume = 1.0f; return;
				case "Pitch": Pitch = this.PitchDefault; return;
				case "Pan": Pan = 1.0f; return;
				case "SpeechRate": case "Speed": SpeechRate = 1.0f; return;
			}
		}


		public ICommand SpeakCommand { get; }
		public ICommand RestoreCommand { get; }
		public ICommand SaveCommand { get; }
		public ICommand ResetValueCommand { get; }


		private bool _ForceEnglish;
		public bool ForceEnglish { get => _ForceEnglish; set => SetProperty(ref _ForceEnglish, value); }

		private float _Pitch;
		public float Pitch { get => _Pitch; set => SetProperty(ref _Pitch, value); }

		private float _Pan;
		public float Pan { get => _Pan; set => SetProperty(ref _Pan, value); }


		private float _Volume;
		public float Volume { get => _Volume; set => SetProperty(ref _Volume, value); }

		private float _SpeechRate;
		public float SpeechRate { get => _SpeechRate; set => SetProperty(ref _SpeechRate, value); }


		private string _OverrideLeft = string.Empty;
		public string OverrideLeft { get => _OverrideLeft; set => SetProperty(ref _OverrideLeft, value); }


		private string _OverrideRight = string.Empty;
		public string OverrideRight { get => _OverrideRight; set => SetProperty(ref _OverrideRight, value); }


		public float PitchMax => Dependency.TextToSpeechOptions.PitchMax;
		public float PitchDefault => Dependency.TextToSpeechOptions.PitchDefault;
		public float PitchMin => Dependency.TextToSpeechOptions.PitchMin;
		public float VolumeMax => Dependency.TextToSpeechOptions.VolumeMax;
		public float VolumeDefault => Dependency.TextToSpeechOptions.VolumeDefault;
		public float VolumeMin => Dependency.TextToSpeechOptions.VolumeMin;
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace EarphoneLeftAndRight.ViewModels
{
	public class PlayConfigViewModel : BaseViewModel
	{
		public PlayConfigViewModel()
		{
			SpeakCommand = new Command((o) => { Save(); PlayViewModel.Speak(o); });
			RestoreCommand = new Command(Restore);
			SaveCommand = new Command(Save);

			Restore();
		}

		public void Save()
		{
			Storages.ConfigStorage.VoiceForeceEnglish.Value = ForceEnglish;
			Storages.ConfigStorage.VoiceVolume.Value = Volume;
			Storages.ConfigStorage.VoicePitch.Value = Pitch;
			Storages.ConfigStorage.VoicePan.Value = Pan;
		}

		public void Restore()
		{
			ForceEnglish = Storages.ConfigStorage.VoiceForeceEnglish.Value;
			Volume = Storages.ConfigStorage.VoiceVolume.Value;
			Pitch = Storages.ConfigStorage.VoicePitch.Value;
			Pan = Storages.ConfigStorage.VoicePan.Value;
		}


		public ICommand SpeakCommand { get; }
		public ICommand RestoreCommand { get; }
		public ICommand SaveCommand { get; }


		private bool _ForceEnglish;
		public bool ForceEnglish { get => _ForceEnglish; set => SetProperty(ref _ForceEnglish, value); }

		private float _Pitch;
		public float Pitch { get => _Pitch; set => SetProperty(ref _Pitch, value); }

		private float _Pan;
		public float Pan { get => _Pan; set => SetProperty(ref _Pan, value); }


		private float _Volume;
		public float Volume { get => _Volume; set => SetProperty(ref _Volume, value); }

		public float PitchMax => Dependency.TextToSpeechOptions.PitchMax;
		public float PitchDefault => Dependency.TextToSpeechOptions.PitchDefault;
		public float PitchMin => Dependency.TextToSpeechOptions.PitchMin;
		public float VolumeMax => Dependency.TextToSpeechOptions.VolumeMax;
		public float VolumeDefault => Dependency.TextToSpeechOptions.VolumeDefault;
		public float VolumeMin => Dependency.TextToSpeechOptions.VolumeMin;


	}
}

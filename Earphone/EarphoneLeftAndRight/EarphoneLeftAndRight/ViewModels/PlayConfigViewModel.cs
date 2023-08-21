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
			SpeakCommand = new Command(PlayViewModel.Speak);
			ForceEnglish = Storages.ConfigStorage.VoiceForeceEnglish.Value;
		}

		public void Save()
		{
			Storages.ConfigStorage.VoiceForeceEnglish.Value = ForceEnglish;
		}

		public ICommand SpeakCommand { get; }

		private bool _ForceEnglish;
		public bool ForceEnglish { get => _ForceEnglish; set => SetProperty(ref _ForceEnglish, value); }
	}
}

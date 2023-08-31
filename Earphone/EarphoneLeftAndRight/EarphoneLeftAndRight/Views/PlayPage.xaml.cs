using EarphoneLeftAndRight.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EarphoneLeftAndRight.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlayPage : ContentPage
	{
		public PlayPage()
		{
			InitializeComponent();

			//IsEnabledは初回しか見てくれない。
			//https://github.com/xamarin/Xamarin.Forms/issues/3838
			Storages.StatusBarManagerStorage.IsTileAddedChanged += OnIsTileAddedChanged;
		}

		protected override async void OnAppearing()
		{
			OnIsTileAddedChanged(this, EventArgs.Empty);

			base.OnAppearing();

			//Xiaomi端末で起動時TextToSpeechのPanが機能しないことがある。モノラルモード的なものになっている。
			//何かしら音を流せば修正可能。
			if (!Storages.AudioStorage.AudioTest.IsPlaying)
			{
				await Storages.AudioStorage.RegisterWave(0, 5.0 / 44100, 0, 0);
				Storages.AudioStorage.AudioTest.Play();
			}
		}

		public void OnIsTileAddedChanged(object sender, EventArgs eventArgs)
		{
			if ((BindingContext as PlayViewModel)?.IsStatusBarManagerSupported != false) return;
			this.ToolbarItems.Remove(ToolbarItemRequestAddTile);
		}
	}
}
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
	public partial class PlayConfigPage : ContentPage
	{
		public PlayConfigPage ()
		{
			InitializeComponent ();
		}

		protected override void OnDisappearing()
		{
			(BindingContext as PlayConfigViewModel)?.Save();

			base.OnDisappearing();
		}

		protected override void OnAppearing()
		{
			(BindingContext as PlayConfigViewModel)?.Restore();

			base.OnAppearing();
		}
	}
}
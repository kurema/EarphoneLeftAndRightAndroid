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
	public partial class LicenseInfoPage : ContentPage
	{
        EarphoneLeftAndRight.Models.License.ILicenseEntry Model => (this.BindingContext as EarphoneLeftAndRight.Models.License.ILicenseEntry);

        public LicenseInfoPage ()
		{
			InitializeComponent ();
		}

        public LicenseInfoPage(EarphoneLeftAndRight.Models.License.ILicenseEntry entry)
        {
            InitializeComponent();

            this.BindingContext = entry;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Xamarin.Essentials.Launcher.OpenAsync(new Uri(Model.ProjectUrl));
            }
            catch { }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            try
            {
                await Xamarin.Essentials.Launcher.OpenAsync(new Uri(Model.LicenseUrl));
            }
            catch { }
        }
    }
}
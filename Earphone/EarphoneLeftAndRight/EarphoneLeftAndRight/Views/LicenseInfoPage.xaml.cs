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
        Models.License.ILicenseEntry? Model => this.BindingContext as Models.License.ILicenseEntry;

        public LicenseInfoPage()
        {
            InitializeComponent();
        }

        public LicenseInfoPage(Models.License.ILicenseEntry entry)
        {
            InitializeComponent();

            this.BindingContext = entry;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (Model is not null) await Xamarin.Essentials.Browser.OpenAsync(new Uri(Model.ProjectUrl));
            }
            catch { }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            try
            {
                if (Model is not null) await Xamarin.Essentials.Browser.OpenAsync(new Uri(Model.LicenseUrl));
            }
            catch { }
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Globalization;

using EarphoneLeftAndRight.Resx;
using EarphoneLeftAndRight.ViewModels;

namespace EarphoneLeftAndRight.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigPage : ContentPage
    {
        public ConfigPage(IEnumerable<SettingItems> items)
        {
            InitializeComponent();

            BindingContext = new ConfigViewModel(items);
            ((ConfigViewModel)BindingContext).Navigation = this.Navigation;
        }

        public ConfigPage()
        {
            InitializeComponent();

            ((ConfigViewModel)BindingContext).Navigation = this.Navigation;
        }

        private bool Pushing = false;
        private async void MyListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (Pushing) return;
            ((ListView)sender).SelectedItem = null;

            if (e.SelectedItem is not SettingItem item) { return; }
            Pushing = true;
            if (item.Action is not null) await item.Action.Invoke(item);
            if (item.BoolSetting) item.BoolValue = !item.BoolValue;
            if (item.Children != null)
            {
                var page = new ConfigPage(item.Children);
                page.Disappearing += (s, ev) => item.SettingUpdate();
                await Navigation.PushAsync(page, true);
            }
            item.SettingUpdate();
            Pushing = false;
        }
    }
}

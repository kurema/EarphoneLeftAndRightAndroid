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

namespace EarphoneLeftAndRight.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigPage : ContentPage
    {
        public ObservableCollection<SettingItems> Items { get; set; }

        public bool SaveOnDisappearing { get; set; } = true;

        protected override void OnDisappearing()
        {
            //if (SaveOnDisappearing)
            //    EarphoneLeftAndRight.Storage.ConfigStorage.SaveLocalData();

            base.OnDisappearing();
        }

        public ConfigPage(ObservableCollection<SettingItems> items) {
            InitializeComponent();

            MyListView.ItemsSource = Items = items;
        }

        public ConfigPage()
        {
            InitializeComponent();

            SaveOnDisappearing = true;

            var licenseChildren = GetLicenseChildren(Navigation);

            Items = new ObservableCollection<SettingItems>
            {
                new SettingItems("About")
                {
                    new SettingItem("License", "")
                    {
                        Children=licenseChildren
                    }
                }
            };
			
			MyListView.ItemsSource = Items;
        }

        public static ConfigPage GetLicensePage(INavigation navigation)
        {
            return new ConfigPage(GetLicenseChildren(navigation));
        }

        public static ObservableCollection<SettingItems> GetLicenseChildren(INavigation navigation)
        {
            var licenseChildren = new ObservableCollection<SettingItems>();
            {
                var licenseChildrenDic = new Dictionary<string, List<SettingItem>>();
                var datas = Storages.LicenseStorage.NugetDatas;
                foreach (var item in datas)
                {
                    if (!licenseChildrenDic.ContainsKey(item.ProjectName)) licenseChildrenDic.Add(item.ProjectName, new List<SettingItem>());
                    licenseChildrenDic[item.ProjectName].Add(new SettingItem(item.Name, item.Version)
                    {
                        Action = async (a) =>
                        {
                            await navigation.PushAsync(new LicenseInfoPage(item));
                        }
                    });
                }
                foreach (var item in licenseChildrenDic)
                {
                    licenseChildren.Add(new SettingItems(item.Value, item.Key));
                }
            }
            return licenseChildren;
        }

        public class SettingItems : ObservableCollection<SettingItem>
        {
            public string Title { get; set; }

            public SettingItems(string Title = "") { this.Title = Title; }
            public SettingItems(IEnumerable<SettingItem> items, string Title = "") : base(items) { this.Title = Title; }
        }

        public class SettingItem: ViewModels.BaseViewModel
        {
            private string text = string.Empty;
            public string Text { get => text; set => SetProperty(ref text, value); }
            private string detail;
            public string Detail { get => detail ?? DetailFunc?.Invoke(this) ?? ""; set => SetProperty(ref detail, value); }
            private Func<SettingItem, string> detailFunc;
            public Func<SettingItem, string> DetailFunc { get => detailFunc; set { SetProperty(ref detailFunc, value); OnPropertyChanged(nameof(Detail)); } }

            public bool BoolSetting { get => boolValue != null; }
            private bool? boolValue=null;
            public bool BoolValue { get => boolValue ?? false; set
                {
                    //Note: CachingStrategy="RecycleElement"にしておくと、IsVisibleのバインディングが評価される前に表示されているSwitchのIsToggledが往復してBoolSettingがtrueになってしまうようだ。
                    //Note: そんなん分からん。再現しづらいから気を付けよう。
                    if (boolValue == null)
                        return;
                    SetProperty(ref boolValue, value); OnPropertyChanged(nameof(Detail)); Action(this);
                } }
            private Func<SettingItem,Task> action;
            public Func<SettingItem,Task> Action { get => action; set => SetProperty(ref action, value); }
            private ObservableCollection<SettingItems> children;
            public ObservableCollection<SettingItems> Children { get => children; set => SetProperty(ref children, value); }

            public SettingItem(string Text, Func<SettingItem, string> Detail, bool? SwitchStatus = null)
            {
                this.Text = Text;
                this.DetailFunc = Detail;
                this.boolValue = SwitchStatus;
            }

            public SettingItem(string Text, string Detail, bool? SwitchStatus = null)
            {
                this.Text = Text;
                this.Detail = Detail;
                this.boolValue = SwitchStatus;
            }

            public void SettingUpdate()
            {
                OnPropertyChanged(nameof(Detail));
            }
        }

        private bool Pushing = false;
        private async void MyListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (Pushing) return;
            ((ListView)sender).SelectedItem = null;

            SettingItem item;
            if ((item = e.SelectedItem as SettingItem) == null) { return; }
            Pushing = true;
            if (item.Action != null) await item.Action?.Invoke(item);
            if (item.BoolSetting) item.BoolValue = !item.BoolValue;
            if (item.Children != null) {
                var page = new ConfigPage(item.Children);
                page.Disappearing += (s, ev) => item.SettingUpdate();
                await Navigation.PushAsync(page, true); }
            item.SettingUpdate();
            Pushing = false;
        }
    }
}

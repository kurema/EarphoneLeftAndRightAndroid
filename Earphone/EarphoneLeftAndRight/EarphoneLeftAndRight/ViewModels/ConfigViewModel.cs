using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;

using EarphoneLeftAndRight.Resx;

namespace EarphoneLeftAndRight.ViewModels
{
    public class ConfigViewModel : BaseViewModel
    {
        public ConfigViewModel() : this("Menu")
        {
        }

        public ConfigViewModel(string itemsId)
        {
            this.Title = AppResources.Config_Title;
            this.ItemsId = itemsId;
        }

        public ConfigViewModel(IEnumerable<SettingItems> items)
        {
            this.Title = AppResources.Config_Title;
            Items = new ObservableCollection<SettingItems>(items);
        }

        public ObservableCollection<SettingItems> Items { get; } = new ObservableCollection<SettingItems>();

        private void SetItems(params SettingItems[] items)
        {
            Items.Clear();
            foreach (var item in items) Items.Add(item);
        }

        private INavigation _Navigation;
        public INavigation Navigation { get => _Navigation; set => SetProperty(ref _Navigation, value); }


        private string _ItemsId;
        public string ItemsId
        {
            get => _ItemsId;
            set
            {
                SetProperty(ref _ItemsId, value);
                switch (value)
                {
                    case "Menu":
                        {
                            new Action(async () =>
                            {
                                await Storages.LicenseStorage.LoadNugetDatasLicenseText();
                                var licenseChildren = GetLicenseChildren(this);
                                SetItems(
                                    new SettingItems(AppResources.Config_Menu_About_Title)
                                    {
                                        new SettingItem(AppResources.Config_Menu_About_Developer_Title, AppResources.Config_Menu_About_Developer_Desc)
                                        {
                                            Action= async (a)=>{
                                                await Shell.Current.GoToAsync($"/{nameof( Views.DeveloperInfoPage)}");
                                            }
                                        },
                                        new SettingItem(AppResources.Config_Menu_About_LicenseApp_Title, AppResources.Config_Menu_About_LicenseApp_Desc){
                                            Action = async (a) =>
                                            {
                                                await Navigation.PushAsync(new Views.LicenseInfoPage(new Models.License.NormalLicense(){
                                                    LicenseText =await Storages.LicenseStorage.LoadLicenseText(nameof(EarphoneLeftAndRight))
                                                    ,Name=nameof(EarphoneLeftAndRight)
                                                    ,ProjectName=nameof(EarphoneLeftAndRight)
                                                    ,LicenseUrl=AppResources.Config_Menu_About_LicenseApp_LicenseUrl
                                                    ,ProjectUrl="https://github.com/kurema/EarphoneLeftAndRightAndroid"
                                                }));
                                            }
                                        },
                                        new SettingItem(AppResources.Config_Menu_About_LicenseOSS_Title,AppResources.Config_Menu_About_LicenseOSS_Desc)
                                        {
                                            Children=licenseChildren
                                        },
                                        new SettingItem(AppResources.Config_Menu_About_PrivacyPolicy_Title, AppResources.Config_Menu_About_PrivacyPolicy_Desc)
                                        {
                                            Action = async (a) =>
                                            {
                                                await Xamarin.Essentials.Browser.OpenAsync("https://github.com/kurema/EarphoneLeftAndRightAndroid/blob/master/Privacy.md");
                                            }
                                        },
                                        new SettingItem(AppResources.Config_Menu_About_RateThisApp_Title, AppResources.Config_Menu_About_RateThisApp_Desc)
                                        {
                                            Action = async (a) =>
                                            {
                                                await Xamarin.Essentials.Launcher.OpenAsync("https://play.google.com/store/apps/details?id=com.github.kurema.earphoneleftandright");
                                            }
                                        },
                                    },
                                    new SettingItems(AppResources.Config_Menu_OtherApps_Title)
                                    {
                                        new SettingItem(AppResources.Config_Menu_OtherApps_MobileWB_Name, AppResources.Config_Menu_OtherApps_MobileWB_Desc)
                                        {
                                            Action = async (a) =>
                                            {
                                                await Xamarin.Essentials.Launcher.OpenAsync("https://play.google.com/store/apps/details?id=com.github.kurema.WordbookImpressApp");
                                            }
                                        },
                                        new SettingItem(AppResources.Config_Menu_OtherApps_BDManager_Name, AppResources.Config_Menu_OtherApps_BDManager_Desc)
                                        {
                                            Action = async (a) =>
                                            {
                                                await Xamarin.Essentials.Launcher.OpenAsync("https://play.google.com/store/apps/details?id=com.github.kurema.BDVideoLibraryManager");
                                            }
                                        },
                                    }
                                    );
                            }).Invoke();
                            break;
                        }
                }
            }
        }

        public static ObservableCollection<SettingItems> GetLicenseChildren(ConfigViewModel viewModel)
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
                            await viewModel.Navigation.PushAsync(new Views.LicenseInfoPage(item));
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
    }

    public class SettingItems : ObservableCollection<SettingItem>
    {
        public string Title { get; set; }

        public SettingItems(string Title = "") { this.Title = Title; }
        public SettingItems(IEnumerable<SettingItem> items, string Title = "") : base(items) { this.Title = Title; }
    }

    public class SettingItem : BaseViewModel
    {
        private string text = string.Empty;
        public string Text { get => text; set => SetProperty(ref text, value); }
        private string detail;
        public string Detail { get => detail ?? DetailFunc?.Invoke(this) ?? ""; set => SetProperty(ref detail, value); }
        private Func<SettingItem, string> detailFunc;
        public Func<SettingItem, string> DetailFunc { get => detailFunc; set { SetProperty(ref detailFunc, value); OnPropertyChanged(nameof(Detail)); } }

        public bool BoolSetting { get => boolValue != null; }
        private bool? boolValue = null;
        public bool BoolValue
        {
            get => boolValue ?? false; set
            {
                //Note: CachingStrategy="RecycleElement"にしておくと、IsVisibleのバインディングが評価される前に表示されているSwitchのIsToggledが往復してBoolSettingがtrueになってしまうようだ。
                //Note: そんなん分からん。再現しづらいから気を付けよう。
                if (boolValue == null)
                    return;
                SetProperty(ref boolValue, value); OnPropertyChanged(nameof(Detail)); Action(this);
            }
        }
        private Func<SettingItem, Task> action;
        public Func<SettingItem, Task> Action { get => action; set => SetProperty(ref action, value); }
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
}

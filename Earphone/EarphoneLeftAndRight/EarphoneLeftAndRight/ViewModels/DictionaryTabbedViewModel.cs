using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using EarphoneLeftAndRight.Resx;
using System.Linq;
using Xamarin.Forms;

namespace EarphoneLeftAndRight.ViewModels
{
    [QueryProperty(nameof(SelectedItemId), nameof(SelectedItemId))]
    public class DictionaryTabbedViewModel : BaseViewModel
    {
        public DictionaryTabbedViewModel()
        {
            Items.Clear();
            Items.Add(new DictionaryViewModel() { Title = AppResources.Dict_Left_Title, DictionaryTitle = AppResources.Dict_Dict_Title, Html = AppResources.Dict_Left_Html, Id = "Left",WebDictionaryLink=AppResources.Dict_WebDic_Left_Url });
            Items.Add(new DictionaryViewModel() { Title = AppResources.Dict_Right_Title, DictionaryTitle = AppResources.Dict_Dict_Title, Html = AppResources.Dict_Right_Html, Id = "Right", WebDictionaryLink = AppResources.Dict_WebDic_Right_Url });
            SelectedItem = Items[0];
        }

        public ObservableCollection<DictionaryViewModel> Items { get; } = new ObservableCollection<DictionaryViewModel>();

        private DictionaryViewModel _SelectedItem;
        public DictionaryViewModel SelectedItem
        {
            get => _SelectedItem;
            set
            {
                SetProperty(ref _SelectedItem, value);
                OnPropertyChanged(nameof(SelectedItemId));
            }
        }

        public string SelectedItemId
        {
            get => SelectedItem?.Id ?? "";
            set
            {
                var result = Items?.FirstOrDefault(a => a.Id == value);
                if(!(result is null))
                {
                    SelectedItem = result;
                }
            }
        }

    }
}

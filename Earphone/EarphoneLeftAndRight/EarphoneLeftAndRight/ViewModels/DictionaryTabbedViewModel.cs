using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using EarphoneLeftAndRight.Resx;
using System.Linq;
using Xamarin.Forms;
using System.Windows.Input;

namespace EarphoneLeftAndRight.ViewModels
{
    [QueryProperty(nameof(SelectedItemId), nameof(SelectedItemId))]
    public class DictionaryTabbedViewModel : BaseViewModel
    {
        public DictionaryTabbedViewModel()
        {
            Items.Clear();
            Items.Add(new DictionaryViewModel() { Title = LocalResources.Left, DictionaryTitle = AppResources.Dict_Dict_Title, Html = AppResources.Dict_Left_Html, Id = "Left",WebDictionaryLink=AppResources.Dict_WebDic_Left_Url });
            Items.Add(new DictionaryViewModel() { Title = LocalResources.Right, DictionaryTitle = AppResources.Dict_Dict_Title, Html = AppResources.Dict_Right_Html, Id = "Right", WebDictionaryLink = AppResources.Dict_WebDic_Right_Url });
            SelectedItem = Items[0];

            ShellGoToCommand = new Xamarin.Forms.Command(async a =>
            {
                await Xamarin.Forms.Shell.Current.GoToAsync(a?.ToString());
            });
        }

        public ObservableCollection<DictionaryViewModel> Items { get; } = new ObservableCollection<DictionaryViewModel>();

        public ICommand ShellGoToCommand { get; }

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

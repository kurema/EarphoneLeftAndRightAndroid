using EarphoneLeftAndRight.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace EarphoneLeftAndRight.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}
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
    public partial class DictionaryPage : ContentPage
    {
        public DictionaryPage()
        {
            InitializeComponent();
        }

        public string Html
        {
            get => (string)GetValue(HtmlProperty);
            set => SetValue(HtmlProperty, value);
        }

        public void UpdateHtml()
        {
            layoutMain.Children.Clear();
            var labels = Helper.Helpers.XhtmlToFormattedString(Html);
            foreach (var item in labels) layoutMain.Children.Add(item);
        }

        public static readonly BindableProperty HtmlProperty =
            BindableProperty.Create(nameof(Html), typeof(string), typeof(DictionaryPage), "", BindingMode.OneWay, propertyChanged: (bindable, oldValue, newValue) =>
            {
                (bindable as DictionaryPage)?.UpdateHtml();
            });

        public string DictionaryName
        {
            get => (string)GetValue(DictionaryNameProperty);
            set => SetValue(DictionaryNameProperty, value);
        }

        public static readonly BindableProperty DictionaryNameProperty =
            BindableProperty.Create(nameof(DictionaryName), typeof(string), typeof(DictionaryPage), "", BindingMode.OneWay, propertyChanged: (bindable, oldValue, newValue) =>
            {
                (bindable as DictionaryPage).DictionaryName = newValue.ToString();
            });


    }
}
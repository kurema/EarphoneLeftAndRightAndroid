using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Windows.Input;


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
            //Operation is too slow so I measured time it takes.
            layoutMain.Children.Clear();//Ticks: 33,828
            {
                //var labels = Helper.Helpers.XhtmlToLabelsClassical(Html);//Ticks: 1,364,394! This is the problem!
                //foreach (var item in labels) layoutMain.Children.Add(item);//Ticks: 20,413
            }
            {
                var labels = Helper.Helpers.XhtmlToLabels(Html);
                foreach (var item in labels) layoutMain.Children.Add(item);
            }

            //{
            //    var label = new Label()
            //    {
            //        TextType = TextType.Html,
            //        Text = Html,
            //    };
            //    layoutMain.Children.Add(label);
            //}
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

        public ICommand OpenWebDictionaryCommand
        {
            get => (ICommand)GetValue(OpenWebDictionaryCommandProperty);
            set => SetValue(OpenWebDictionaryCommandProperty, value);
        }

        public static readonly BindableProperty OpenWebDictionaryCommandProperty =
            BindableProperty.Create(nameof(OpenWebDictionaryCommand), typeof(ICommand), typeof(DictionaryPage), new Command(() => { }, () => false), BindingMode.OneWay);
    }
}
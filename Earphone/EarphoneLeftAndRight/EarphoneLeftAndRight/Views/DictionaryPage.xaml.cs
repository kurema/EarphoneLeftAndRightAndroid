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
                var labels = Helper.Helpers.XhtmlToLabels(Html);
                var sizeD = Device.GetNamedSize(NamedSize.Body, typeof(Label));
                foreach (var item in labels)
                {
                    item.InputTransparent = true;
                    item.FontSize = sizeD;
                    layoutMain.Children.Add(item);
                }
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

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var sizeM = Device.GetNamedSize(NamedSize.Body, typeof(Label));
            var sizeS = sizeM / 1.3;
            var sizeL = sizeM * 1.3;
            foreach (var item in layoutMain.Children)
            {
                if (item is not Label l) continue;
                if (l.FontSize == sizeM)
                {
                    l.FontSize = sizeL;
                }
                else if (l.FontSize == sizeL)
                {
                    l.FontSize = sizeS;
                }
                else
                {
                    l.FontSize = sizeM;
                }
            }
        }


        private void PinchGestureRecognizer_PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            double currentSize = layoutMain.Children.OfType<Label>().FirstOrDefault()?.FontSize ?? -1;
            var scale = (e.Scale - 1) * 5 + 1;
            switch (e.Status)
            {
                case GestureStatus.Started:
                case GestureStatus.Running:
                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    currentSize *= scale;
                    break;
                default:return;
            }

            if (currentSize < 0) return;

            foreach (var item in layoutMain.Children)
            {
                if (item is not Label l) continue;
                l.FontSize = currentSize;
            }
        }
    }
}
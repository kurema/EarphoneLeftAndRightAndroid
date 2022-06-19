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
            layoutMain.Children.Clear();//Ticks: 33,828
            var innnerStack = new StackLayout() { InputTransparent = true };
            {
                var labels = Helper.Helpers.XhtmlToLabels(Html);
                var sizeD = Device.GetNamedSize(NamedSize.Body, typeof(Label));
                foreach (var item in labels)
                {
                    item.InputTransparent = true;
                    item.FontSize = sizeD;
                    innnerStack.Children.Add(item);
                }
                layoutMain.Children.Add(innnerStack);
            }

            this.InvalidateMeasure();
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
            if (layoutMain.Children.Count == 0 || layoutMain.Children[0] is not StackLayout stack) return;
            foreach (var item in stack.Children)
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
            //var scale = (e.Scale - 1) * 5 + 1;
            var scale = e.Scale;
            switch (e.Status)
            {
                case GestureStatus.Started:
                case GestureStatus.Running:
                    {
                        var lScale = layoutMain.Scale * scale;
                        lScale = Math.Max(0.1, lScale);
                        layoutMain.Scale = lScale;
                    }
                    return;
                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    break;
                default: return;
            }

            if (layoutMain.Children.Count == 0 || layoutMain.Children[0] is not StackLayout stack) return;
            double fontSize = stack.Children.OfType<Label>().FirstOrDefault()?.FontSize ?? -1;
            fontSize *= layoutMain.Scale;
            layoutMain.Scale = 1.0d;
            if (fontSize < 0) return;

            foreach (var item in stack.Children)
            {
                if (item is not Label l) continue;
                l.FontSize = fontSize;
            }
        }
    }
}
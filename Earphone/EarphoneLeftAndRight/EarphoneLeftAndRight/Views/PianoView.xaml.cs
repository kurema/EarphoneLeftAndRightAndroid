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
    public partial class PianoView : ContentView
    {
        public PianoView()
        {
            InitializeComponent();

            this.SizeChanged += (_, _) => PreparePiano();
        }

        public delegate void KeyTappedEventHandler(object sender, KeyTappedEventArgs args);
        public event KeyTappedEventHandler KeyTapped;

        public class KeyTappedEventArgs : EventArgs
        {
            public KeyTappedEventArgs(int pressedKey)
            {
                PressedKey = pressedKey;
            }

            public int PressedKey { get; }
        }



        public void PreparePiano()
        {
            pianoGrid.Children.Clear();
            int blackKeyExists = 1 | 2 | 8 | 16 | 32;
            int count = (int)Math.Floor(this.Width / KeyWidth);
            int keyCount = 0;
            pianoGrid.ColumnDefinitions.Clear();

            List<Grid> blackKeys = new();
            for (int i = 0; i < count; i++)
            {
                pianoGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                pianoGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                {
                    var whiteKey = new Grid() { BackgroundColor = Color.White };
                    Grid.SetColumnSpan(whiteKey, 2);
                    Grid.SetRowSpan(whiteKey, 2);
                    Grid.SetColumn(whiteKey, i * 2);
                    {
                        int keyCountCurrent = keyCount;
                        var gr = new TapGestureRecognizer();
                        gr.Tapped += (_, _) => this.KeyTapped?.Invoke(this, new KeyTappedEventArgs(keyCountCurrent));
                        whiteKey.GestureRecognizers.Add(gr);
                    }
                    pianoGrid.Children.Add(whiteKey);
                }
                keyCount++;
                if ((blackKeyExists & 1 << (i % 7)) != 0)
                {
                    var blackKey = new Grid() { BackgroundColor = Color.Black };
                    Grid.SetColumn(blackKey, i * 2 + 1);
                    if (i + 1 < count) Grid.SetColumnSpan(blackKey, 2);
                    blackKeys.Add(blackKey);
                    {
                        int keyCountCurrent = keyCount;
                        var gr = new TapGestureRecognizer();
                        gr.Tapped += (_, _) => this.KeyTapped?.Invoke(this, new KeyTappedEventArgs(keyCountCurrent));
                        blackKey.GestureRecognizers.Add(gr);
                    }
                    keyCount++;
                }
            }
            foreach (var item in blackKeys) pianoGrid.Children.Add(item);
        }



        public double KeyWidth
        {
            get { return (double)GetValue(KeyWidthProperty); }
            set { SetValue(KeyWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyWidth.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty KeyWidthProperty =
            BindableProperty.Create("KeyWidth", typeof(double), typeof(PianoView), 12.0);


    }
}
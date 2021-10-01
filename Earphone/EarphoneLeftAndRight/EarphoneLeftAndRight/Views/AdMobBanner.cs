using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using Xamarin.Forms;

namespace EarphoneLeftAndRight.Views
{
    public class AdMobBanner : ContentView
    {
        bool _IsAdaptive = true;

        public bool IsAdaptive { get => _IsAdaptive; set
            {
                _IsAdaptive = value;
                OnPropertyChanged();
            }
        }

        bool _IsAdLoaded = false;

        public bool IsAdLoaded
        {
            get => _IsAdLoaded;
            set
            {
                _IsAdLoaded = value;
                OnPropertyChanged();
            }
        }

        public AdMobBanner()
        {
        }
    }
}

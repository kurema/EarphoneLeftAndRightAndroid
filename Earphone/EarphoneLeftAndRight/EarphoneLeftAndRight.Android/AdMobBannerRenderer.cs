using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using EarphoneLeftAndRight.Views;
using EarphoneLeftAndRight.Droid.Renderers;
using Android.Gms.Ads;

//https://qiita.com/t-miyake/items/3f92a6601848e5b21de4

[assembly: ExportRenderer(typeof(AdMobBanner), typeof(AdMobBannerRenderer))]
namespace EarphoneLeftAndRight.Droid.Renderers
{
    public class AdMobBannerRenderer : ViewRenderer<AdMobBanner, Android.Gms.Ads.AdView>
    {
        public const string AdUnitIdBanner = "ca-app-pub-3940256099942544/6300978111";

        public AdMobBannerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobBanner> e)
        {
            base.OnElementChanged(e);

            if (Control is null)
            {
                if (e.NewElement.IsAdaptive)
                {
                    //https://stackoverflow.com/questions/66661164/xamarin-forms-admob-adaptive-banner-ad-android
                    var w = (int)(Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width / Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density);
                    var size = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSize(Context, w);
                    var adView = new AdView(Context)
                    {
                        AdUnitId = AdUnitIdBanner,
                        LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                        AdSize = size
                    };
                    if (size?.IsAutoHeight != true) e.NewElement.AdHeight = size.Height;

                    var reqbuilder = new AdRequest.Builder();
                    adView.LoadAd(reqbuilder.Build());

                    {
                        var adListner = new AdListnerDelegate();
                        adListner.AdLoaded += (s, _) => e.NewElement.IsAdLoaded = true;
                        adView.AdListener = adListner;
                    }
                    SetNativeControl(adView);
                }
                else
                {
                    var adMobBanner = new AdView(Context)
                    {
                        AdSize = AdSize.Fluid,
                        AdUnitId = AdUnitIdBanner
                    };

                    var reqbuilder = new AdRequest.Builder();
                    adMobBanner.LoadAd(reqbuilder.Build());

                    {
                        var adListner = new AdListnerDelegate();
                        adListner.AdLoaded += (s, _) => e.NewElement.IsAdLoaded = true;
                        adMobBanner.AdListener = adListner;
                    }
                    SetNativeControl(adMobBanner);
                }
            }
        }

        public class AdListnerDelegate : AdListener
        {
            //Why Android do not do this? Seriously why?

            public event EventHandler AdLoaded;
            public event EventHandler<AdError> AdFailedToLoad;
            public event EventHandler AdOpened;
            public event EventHandler AdClicked;
            public event EventHandler AdClosed;
            public event EventHandler AdImpression;

            public override void OnAdLoaded()
            {
                AdLoaded?.Invoke(this, new EventArgs());
                base.OnAdLoaded();
            }

            public override void OnAdFailedToLoad(LoadAdError p0)
            {
                AdFailedToLoad?.Invoke(this, p0);
                base.OnAdFailedToLoad(p0);
            }

            public override void OnAdOpened()
            {
                AdOpened?.Invoke(this, new EventArgs());
                base.OnAdOpened();
            }

            public override void OnAdClicked()
            {
                AdClicked?.Invoke(this, new EventArgs());
                base.OnAdClicked();
            }

            public override void OnAdClosed()
            {
                AdClosed?.Invoke(this, new EventArgs());
                base.OnAdClosed();
            }

            public override void OnAdImpression()
            {
                AdImpression?.Invoke(this, new EventArgs());
                base.OnAdImpression();
            }
        }

    }
}
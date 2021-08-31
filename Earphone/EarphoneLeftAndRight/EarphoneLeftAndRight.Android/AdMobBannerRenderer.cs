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

//https://qiita.com/t-miyake/items/3f92a6601848e5b21de4

[assembly: ExportRenderer(typeof(AdMobBanner),typeof(AdMobBannerRenderer))]
namespace EarphoneLeftAndRight.Droid.Renderers
{
    public class AdMobBannerRenderer : ViewRenderer<AdMobBanner, Android.Gms.Ads.AdView>
    {
        public AdMobBannerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobBanner> e)
        {
            string adUnitId = Secrets.AdUnitIdTest;

            base.OnElementChanged(e);

            if(Control is null)
            {
                var adMobBanner = new Android.Gms.Ads.AdView(Context);
                adMobBanner.AdSize = Android.Gms.Ads.AdSize.Fluid;
                adMobBanner.AdUnitId = adUnitId;

                var reqbuilder = new Android.Gms.Ads.AdRequest.Builder();
                adMobBanner.LoadAd(reqbuilder.Build());

                SetNativeControl(adMobBanner);
            }
        }
    }
}
using EarphoneLeftAndRight.ViewModels;
using EarphoneLeftAndRight.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

using System.Threading.Tasks;

namespace EarphoneLeftAndRight
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(DeveloperInfoPage), typeof(DeveloperInfoPage));

            //Device.BeginInvokeOnMainThread(async () =>
            //{
            //    await Prepare();
            //});

        }

        //private async Task Prepare()
        //{
        //    var ump = UserMessagingPlatform.Instance;
        //    if (ump.IsSupported)
        //    {
        //        var parameters = new RequestParameters
        //        {
        //            DebugSettings = new ConsentDebugSettings
        //            {
        //                // Optional, for testing
        //                Geography = DebugGeography.Eea,
        //            }
        //        };
        //        try
        //        {
        //            var consent = await ump.GetConsentInformationAsync(parameters);

        //            if (consent.ConsentStatus == ConsentStatus.Required && consent.FormStatus == FormStatus.Available)
        //            {
        //                await ump.LoadConsentFormAsync();
        //                await ump.ShowFormAsync();
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //        }
        //    }
        //}

    }
}

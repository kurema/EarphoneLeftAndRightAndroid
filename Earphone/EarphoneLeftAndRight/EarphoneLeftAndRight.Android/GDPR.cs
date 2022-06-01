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

namespace EarphoneLeftAndRight.Droid
{
    //https://supernovaic.blogspot.com/2022/02/guide-for-using-xamaringoogleusermessag.html
    internal class GDPR
    {
        public void SetGDPR(Activity activity, ContentResolver ContentResolver)
        {
            try
            {
#if DEBUG
                var debugSettings = new Xamarin.Google.UserMesssagingPlatform.ConsentDebugSettings
                    .Builder(activity)
                    .SetDebugGeography(Xamarin.Google.UserMesssagingPlatform.ConsentDebugSettings
                        .DebugGeography
                        .DebugGeographyEea)
                .AddTestDeviceHashedId(Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId))
                .Build();
#endif

                var requestParameters = new Xamarin.Google.UserMesssagingPlatform.ConsentRequestParameters
                    .Builder()
                    .SetTagForUnderAgeOfConsent(false)
#if DEBUG
                    .SetConsentDebugSettings(debugSettings)
#endif
                    .Build();

                var consentInformation = Xamarin.Google.UserMesssagingPlatform.UserMessagingPlatform.GetConsentInformation(activity);
//#if DEBUG
//                consentInformation.Reset();
//#endif

                consentInformation.RequestConsentInfoUpdate(
                    activity,
                    requestParameters,
                    new GoogleUMPConsentUpdateSuccessListener(
                        () =>
                        {
                            // The consent information state was updated.
                            // You are now ready to check if a form is available.
                            if (consentInformation.IsConsentFormAvailable)
                            {
                                Xamarin.Google.UserMesssagingPlatform.UserMessagingPlatform.LoadConsentForm(
                                    activity,
                                    new GoogleUMPFormLoadSuccessListener((Xamarin.Google.UserMesssagingPlatform.IConsentForm f) =>
                                    {
                                        googleUMPConsentForm = f;
                                        googleUMPConsentInformation = consentInformation;
                                        DisplayAdvertisingConsentFormIfNecessary(activity);
                                    }),
                                    new GoogleUMPFormLoadFailureListener((Xamarin.Google.UserMesssagingPlatform.FormError e) =>
                                    {
                                    }));
                            }
                            else
                            {
                            }
                        }),
                    new GoogleUMPConsentUpdateFailureListener(
                        (Xamarin.Google.UserMesssagingPlatform.FormError e) =>
                        {
                        })
                    );
            }
            catch
            {
            }
        }

        private Xamarin.Google.UserMesssagingPlatform.IConsentForm googleUMPConsentForm = null;
        private Xamarin.Google.UserMesssagingPlatform.IConsentInformation
        googleUMPConsentInformation = null;
        public void DisplayAdvertisingConsentFormIfNecessary(Activity activity)
        {
            try
            {
                if (googleUMPConsentForm != null && googleUMPConsentInformation != null)
                {
                    /* ConsentStatus:
                        Unknown = 0,
                        NotRequired = 1,
                        Required = 2,
                        Obtained = 3
                    */
                    if (googleUMPConsentInformation.ConsentStatus == 2)
                    {
                        DisplayAdvertisingConsentForm(activity);
                    }
                    else
                    {
                    }
                }
                else
                {
                }
            }
            catch
            {
            }
        }

        public void DisplayAdvertisingConsentForm(Activity activity)
        {
            try
            {
                if (googleUMPConsentForm != null && googleUMPConsentInformation != null)
                {
                    googleUMPConsentForm.Show(activity, new GoogleUMPConsentFormDismissedListener(
                            (Xamarin.Google.UserMesssagingPlatform.FormError f) =>
                            {
                                if (googleUMPConsentInformation.ConsentStatus == 2) // required
                                {
                                    DisplayAdvertisingConsentForm(activity);
                                }
                            }));
                }
                else
                {
                }
            }
            catch
            {
            }
        }

        public class GoogleUMPConsentFormDismissedListener : Java.Lang.Object,
        Xamarin.Google.UserMesssagingPlatform.IConsentFormOnConsentFormDismissedListener
        {
            public GoogleUMPConsentFormDismissedListener(
        Action<Xamarin.Google.UserMesssagingPlatform.FormError> failureAction)
            {
                a = failureAction;
            }
            public void OnConsentFormDismissed(Xamarin.Google.UserMesssagingPlatform.FormError f)
            {
                a(f);
            }

            private Action<Xamarin.Google.UserMesssagingPlatform.FormError> a = null;
        }

        public class GoogleUMPConsentUpdateFailureListener : Java.Lang.Object,
        Xamarin.Google.UserMesssagingPlatform.IConsentInformationOnConsentInfoUpdateFailureListener
        {
            public GoogleUMPConsentUpdateFailureListener(
        Action<Xamarin.Google.UserMesssagingPlatform.FormError> failureAction)
            {
                a = failureAction;
            }
            public void OnConsentInfoUpdateFailure(Xamarin.Google.UserMesssagingPlatform.FormError f)
            {
                a(f);
            }

            private Action<Xamarin.Google.UserMesssagingPlatform.FormError> a = null;
        }

        public class GoogleUMPConsentUpdateSuccessListener : Java.Lang.Object,
        Xamarin.Google.UserMesssagingPlatform.IConsentInformationOnConsentInfoUpdateSuccessListener
        {
            public GoogleUMPConsentUpdateSuccessListener(Action successAction)
            {
                a = successAction;
            }

            public void OnConsentInfoUpdateSuccess()
            {
                a();
            }

            private Action a = null;
        }

        public class GoogleUMPFormLoadFailureListener : Java.Lang.Object,
        Xamarin.Google.UserMesssagingPlatform.UserMessagingPlatform
        .IOnConsentFormLoadFailureListener
        {
            public GoogleUMPFormLoadFailureListener(
        Action<Xamarin.Google.UserMesssagingPlatform.FormError> failureAction)
            {
                a = failureAction;
            }
            public void OnConsentFormLoadFailure(Xamarin.Google.UserMesssagingPlatform.FormError e)
            {
                a(e);
            }

            private Action<Xamarin.Google.UserMesssagingPlatform.FormError> a = null;
        }

        public class GoogleUMPFormLoadSuccessListener : Java.Lang.Object,
        Xamarin.Google.UserMesssagingPlatform.UserMessagingPlatform
        .IOnConsentFormLoadSuccessListener
        {
            public GoogleUMPFormLoadSuccessListener(
        Action<Xamarin.Google.UserMesssagingPlatform.IConsentForm> successAction)
            {
                a = successAction;
            }
            public void OnConsentFormLoadSuccess(
        Xamarin.Google.UserMesssagingPlatform.IConsentForm f)
            {
                a(f);
            }

            private Action<Xamarin.Google.UserMesssagingPlatform.IConsentForm> a = null;
        }

    }
}
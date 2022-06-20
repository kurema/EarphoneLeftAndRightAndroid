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

[assembly: Xamarin.Forms.Dependency(typeof(EarphoneLeftAndRight.Droid.PlatformLocalizationDependency))]

namespace EarphoneLeftAndRight.Droid
{
    public class PlatformLocalizationDependency : Dependency.IPlatformLocalization
    {
        public string WordLeft => Application.Context.GetString(Resource.String.word_left);

        public string WordRight => Application.Context.GetString(Resource.String.word_right);
    }
}
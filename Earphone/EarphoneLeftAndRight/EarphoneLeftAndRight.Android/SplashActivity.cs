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

using System.Threading.Tasks;

namespace EarphoneLeftAndRight.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/icon", Theme = "@style/MainTheme.Splash", MainLauncher = true, NoHistory = true, Exported = true)]
    public class SplashActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        //https://itblogdsi.blog.fc2.com/blog-entry-181.html
        //https://docs.microsoft.com/xamarin/android/user-interface/splash-screen
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            base.OnCreate(savedInstanceState, persistentState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            var startupWork = new Task(() => { StartActivity(new Intent(Application.Context, typeof(MainActivity))); });
            startupWork.Start();
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
        }
    }
}
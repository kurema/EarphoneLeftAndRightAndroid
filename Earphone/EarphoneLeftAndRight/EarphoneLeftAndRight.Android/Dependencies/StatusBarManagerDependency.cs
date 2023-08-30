using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(EarphoneLeftAndRight.Droid.StatusBarManagerDependency))]
namespace EarphoneLeftAndRight.Droid
{
	public class StatusBarManagerDependency : Dependency.IStatusBarManager
	{
		public bool RequestAddTileServiceIsSupported => Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu;

		public void RequestAddTileService()
		{
			try
			{
				if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
				{

					if (!(Application.Context.GetSystemService(Context.StatusBarService) is StatusBarManager manager)) return;
					manager.RequestAddTileService(new ComponentName(Application.Context, "com.github.kurema.earphoneleftandright.PlayService"),
						 Application.Context.GetString(Resource.String.tile_play_name),
						Android.Graphics.Drawables.Icon.CreateWithResource(Application.Context, Resource.Drawable.outline_earbuds_24),
						new MyExecutor(),
						new MyConsumer());
					//Java.Util.Concurrent.Executors.NewSingleThreadExecutor()
				}
			}
			catch
			{
			}
		}

		public class MyExecutor : Java.Lang.Object, Java.Util.Concurrent.IExecutor
		{
			public void Execute(Java.Lang.IRunnable command)
			{
			}
		}


		public class MyConsumer : Java.Lang.Object, Java.Util.Functions.IConsumer, AndroidX.Core.Util.IConsumer
		{
			public void Accept(Java.Lang.Object t)
			{
			}
		}

	}
}
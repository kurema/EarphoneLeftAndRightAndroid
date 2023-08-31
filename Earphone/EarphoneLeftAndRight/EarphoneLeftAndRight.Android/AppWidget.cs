using Android.App;
using Android.Appwidget;
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
	[BroadcastReceiver(Label = "@string/widget_name", Exported = true)]
	[IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
	[MetaData("android.appwidget.provider", Resource = "@xml/appwidget_provider")]
	public class AppWidget : AppWidgetProvider
	{
		public const string ACTION_SELECTED = "com.github.kurema.earphoneleftandright.widget.ACTION_SELECTED";

		public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
		{
			//https://developer.android.com/guide/topics/appwidgets
			//var me = new ComponentName(context, Java.Lang.Class.FromType(typeof(AppWidget)).Name);
			foreach (var appWidgetId in appWidgetIds)
			{
				appWidgetManager.UpdateAppWidget(appWidgetId, BuildRemoteViews(context, appWidgetIds));
			}

			//base.OnUpdate(context, appWidgetManager, appWidgetIds);
		}

#pragma warning disable IDE0060 // 未使用のパラメーターを削除します
		RemoteViews BuildRemoteViews(Context context, int[] appWidgetIds)
#pragma warning restore IDE0060 // 未使用のパラメーターを削除します
		{
			var widgetView = new RemoteViews(context.PackageName, Resource.Layout.app_widget);
			var intent = new Intent(context, typeof(AppWidget));
			intent.SetAction(ACTION_SELECTED);
			//https://stackoverflow.com/questions/72330310/cannot-execute-pendingintent-in-app-widget-android12
			//https://star-zero.medium.com/pendingintent%E3%81%AEflag-immutable%E3%81%A8flag-mutable-d72bf7c90135
			//Target SDK 31(Android 12)から PendingIntent のmutability(可変性)を指定する必要があります。
			var pi = PendingIntent.GetBroadcast(context, 0, intent, Build.VERSION.SdkInt >= BuildVersionCodes.S ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable : PendingIntentFlags.UpdateCurrent);
			widgetView.SetOnClickPendingIntent(Resource.Id.imageView1, pi);
			//Note:
			//android:src="@mipmap/icon_round" in app_widget.xml crashed on the Lilipop 5.0. But @mipmap/icon is good enough. So next line is not required anyway.
			//if (Build.VERSION.SdkInt >= BuildVersionCodes.O) widgetView.SetImageViewResource(Resource.Id.imageView1, Resource.Mipmap.icon_round);
			return widgetView;
		}

		public override async void OnReceive(Context context, Intent intent)
		{
			base.OnReceive(context, intent);

			switch (intent.Action)
			{
				case ACTION_SELECTED:
					await PlayTileService.PlayLeftRightSound();
					break;
			}
		}
	}
}
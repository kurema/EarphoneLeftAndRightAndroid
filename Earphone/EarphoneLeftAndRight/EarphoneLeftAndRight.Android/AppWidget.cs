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
    [BroadcastReceiver(Label = "Stereo Test Widget")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/appwidget_provider")]
    public class AppWidget : AppWidgetProvider
    {
        public const string ACTION_SELECTED = "com.github.kurema.earphoneleftandright.widget.ACTION_SELECTED";

        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            var me = new ComponentName(context, Java.Lang.Class.FromType(typeof(AppWidget)).Name);

            appWidgetManager.UpdateAppWidget(me, BuildRemoteViews(context, appWidgetIds));

            //base.OnUpdate(context, appWidgetManager, appWidgetIds);
        }

        RemoteViews BuildRemoteViews(Context context, int[] appWidgetIds)
        {
            var widgetView = new RemoteViews(context.PackageName, Resource.Layout.app_widget);
            var intent = new Intent(context, typeof(AppWidget));
            intent.SetAction(ACTION_SELECTED);
            var pi = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            widgetView.SetOnClickPendingIntent(Resource.Id.imageView1, pi);
            return widgetView;
        }

        public override async void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);

            switch (intent.Action)
            {
                case ACTION_SELECTED:
                    Manager.Tts.StopIfSpeaking();

                    await Manager.Tts.SpeakLeft();
                    await Manager.Tts.SpeakRight();
                    break;
            }
        }
    }
}
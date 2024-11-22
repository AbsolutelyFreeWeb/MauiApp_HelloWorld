using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Firebase.Messaging;
using Java.Util;
using MauiLib1;

[Service(Exported = true)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class FirebaseService : FirebaseMessagingService
{
    public FirebaseService() { }

    public override void OnNewToken(string token)
    {
        base.OnNewToken(token);
        if (Preferences.ContainsKey("DeviceToken"))
        {
            Preferences.Remove("DeviceToken");
        }
        Preferences.Set("DeviceToken", token);
        //FirebaseMessaging.Instance.SubscribeToTopic("General");
    }

    public override void OnMessageReceived(RemoteMessage message)
    {
        base.OnMessageReceived(message);
        var notification = message.GetNotification();
        SendNotification(notification.Body, notification.Title, message.Data);
    }

    public static void RetrieveCurrentToken()
    {
        //Firebase.FirebaseApp.InitializeApp()
        var token = (string) FirebaseMessaging.Instance.GetToken().Result;
        Preferences.Set("DeviceToken", token);
    }
    private void SendNotification(string messageBody, string title, IDictionary<string, string> data)
    {
        var intent = new Intent(this, typeof(MyActivity));
        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

        foreach (var key in data.Keys)
        {
            string value = data[key];
            intent.PutExtra(key, value);
        }

        var pendingIntent = PendingIntent.GetActivity(this, MyActivity.NotificationID, intent, PendingIntentFlags.OneShot | PendingIntentFlags.Immutable);

        var notificationBuilder = new NotificationCompat.Builder(this, MyActivity.Channel_ID)
            .SetContentTitle(title)
            .SetSmallIcon(FirebaseMyWorkerService.Resource.Drawable.tooltip_frame_light)
            .SetContentText(messageBody)
            .SetChannelId(MyActivity.Channel_ID)
            .SetContentIntent(pendingIntent)
            .SetAutoCancel(true)
            .SetPriority((int)NotificationPriority.Max);

        var notificationManager = NotificationManagerCompat.From(this);
        notificationManager.Notify(MyActivity.NotificationID, notificationBuilder.Build());
    }
}
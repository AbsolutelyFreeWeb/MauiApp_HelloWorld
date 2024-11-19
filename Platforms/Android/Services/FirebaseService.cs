﻿using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Firebase.Messaging;
using MauiApp_HellowWorld;

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
        var token = (string) Firebase.Messaging.FirebaseMessaging.Instance.GetToken().Result;
        Preferences.Set("DeviceToken", token);
    }
    private void SendNotification(string messageBody, string title, IDictionary<string, string> data)
    {
        var intent = new Intent(this, typeof(MainActivity));
        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

        foreach (var key in data.Keys)
        {
            string value = data[key];
            intent.PutExtra(key, value);
        }

        var pendingIntent = PendingIntent.GetActivity(this, MainActivity.NotificationID, intent, PendingIntentFlags.OneShot | PendingIntentFlags.Immutable);

        var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.Channel_ID)
            .SetContentTitle(title)
            .SetSmallIcon(MauiApp_HellowWorld.Resource.Mipmap.appicon)
            .SetContentText(messageBody)
            .SetChannelId(MainActivity.Channel_ID)
            .SetContentIntent(pendingIntent)
            .SetAutoCancel(true)
            .SetPriority((int)NotificationPriority.Max);

        var notificationManager = NotificationManagerCompat.From(this);
        notificationManager.Notify(MainActivity.NotificationID, notificationBuilder.Build());
    }
}
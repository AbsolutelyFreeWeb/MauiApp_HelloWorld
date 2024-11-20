using System.Diagnostics;

namespace MauiApp_HelloWorld;

public partial class MainPage : ContentPage
{
	int count = 0;
    private string _deviceToken;

    public MainPage()
	{
		InitializeComponent();
        if (Preferences.ContainsKey("DeviceToken"))
        {
            _deviceToken = Preferences.Get("DeviceToken", "");
        }
    }

	private async void OnCounterClicked(object sender, EventArgs e)
	{
        Trace.WriteLine(_deviceToken);
#if ANDROID
        if (_deviceToken == null)
        {
            Trace.WriteLine("Warning: Device token was empty");
            FirebaseService.RetrieveCurrentToken();
            _deviceToken = Preferences.Get("DeviceToken", "");
            Trace.WriteLine(_deviceToken);
            Trace.WriteLine("Done retrieving token");
        } 
#endif
        await Application.Current.MainPage.DisplayAlert(null, _deviceToken, "OK");


        SemanticScreenReader.Announce(CounterBtn.Text);
	}
}


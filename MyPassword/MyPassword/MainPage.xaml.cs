using Plugin.Fingerprint.Abstractions;

namespace MyPassword;

public partial class MainPage : ContentPage
{
    private readonly IFingerprint _fingerprint;
    private readonly MainPageViewModel _mainPage;

    public MainPage(IFingerprint fingerprint)
	{
		InitializeComponent();
        _mainPage = new MainPageViewModel();
        BindingContext = _mainPage;
        _fingerprint = fingerprint;

        this.Loaded += MainPage_Loaded;
    }

    private async void MainPage_Loaded(object sender, EventArgs e)
    {
        var request = new AuthenticationRequestConfiguration("Prove you have fingers!", "Because without it you can't have access");
        var result = await _fingerprint.AuthenticateAsync(request);
        if (!result.Authenticated)
        {
            App.Current.Quit();
        }
        else
        {
            _mainPage.LoadData();
        }
    }
}
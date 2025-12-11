using FinanzasApp.Services;
using FinanzasApp.ViewModels;

namespace FinanzasApp.Views;

public partial class LoginPage : ContentPage
{
    private CancellationTokenSource? _animationCts;

    public LoginPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "finanzas.db3");
        var database = new DatabaseService(dbPath);

        BindingContext = new LoginViewModel(database);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _animationCts = new CancellationTokenSource();
        _ = StartBackgroundAnimationAsync(_animationCts.Token);

        if (LoginContainer != null)
        {
            LoginContainer.Opacity = 0;
            LoginContainer.TranslationY = 60;

            await Task.WhenAll(
                LoginContainer.FadeTo(1, 800, Easing.CubicOut),
                LoginContainer.TranslateTo(0, 0, 800, Easing.CubicOut)
            );
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _animationCts?.Cancel();
        _animationCts?.Dispose();
        _animationCts = null;
    }

    private async Task StartBackgroundAnimationAsync(CancellationToken token)
    {
        await Task.Delay(100);
        var rand = new Random();

        while (!token.IsCancellationRequested)
        {
            if (BlueBlob == null || PurpleBlob == null)
                break;

            double blueX = rand.Next(-40, 40);
            double blueY = rand.Next(-40, 40);
            double purpleX = rand.Next(-60, 60);
            double purpleY = rand.Next(-60, 60);
            uint duration = (uint)rand.Next(4500, 7200);

            try
            {
                await Task.WhenAll(
                    BlueBlob.TranslateTo(blueX, blueY, duration, Easing.SinInOut),
                    PurpleBlob.TranslateTo(purpleX, purpleY, duration, Easing.SinInOut)
                );
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private async void OnInputFocused(object sender, FocusEventArgs e)
    {
        if (sender is not Entry entry) return;
        if (entry.Parent is not Grid grid) return;
        if (grid.Parent is not Border border) return;

        await Task.WhenAll(
            border.ScaleTo(1.02, 150, Easing.CubicOut),
            border.FadeTo(1, 150)
        );
    }

    private async void OnInputUnfocused(object sender, FocusEventArgs e)
    {
        if (sender is not Entry entry) return;
        if (entry.Parent is not Grid grid) return;
        if (grid.Parent is not Border border) return;

        await border.ScaleTo(1, 150, Easing.CubicOut);
    }
}

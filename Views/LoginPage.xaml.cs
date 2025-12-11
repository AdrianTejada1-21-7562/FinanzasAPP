using FinanzasApp.Services;
using FinanzasApp.ViewModels;

namespace FinanzasApp.Views;

public partial class LoginPage : ContentPage
{
    private CancellationTokenSource? _animationCts;

    public LoginPage()
    {
        InitializeComponent();

        // Asegúrate de que esta ruta sea correcta para tu servicio
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "finanzas.db3");
        var database = new DatabaseService(dbPath);

        BindingContext = new LoginViewModel(database);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // 1. Iniciar animación de fondo
        _animationCts = new CancellationTokenSource();
        _ = StartBackgroundAnimationAsync(_animationCts.Token);

        // 2. Animación de entrada del formulario
        if (LoginContainer != null)
        {
            LoginContainer.Opacity = 0;
            LoginContainer.TranslationY = 50;

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
        // Pequeña pausa para asegurar carga de UI
        await Task.Delay(100);

        var rand = new Random();

        // Bucle de animación infinito hasta cancelación
        while (!token.IsCancellationRequested)
        {
            if (BlueBlob == null || PinkBlob == null) break;

            double blueX = rand.Next(-50, 50);
            double blueY = rand.Next(-50, 50);
            double pinkX = rand.Next(-50, 50);
            double pinkY = rand.Next(-50, 50);

            uint duration = (uint)rand.Next(4000, 7000);

            try
            {
                await Task.WhenAll(
                    BlueBlob.TranslateTo(blueX, blueY, duration, Easing.SinInOut),
                    PinkBlob.TranslateTo(pinkX, pinkY, duration, Easing.SinInOut)
                );
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace FinanzasApp.Views
{
    public partial class RegisterPage : ContentPage
    {
        private CancellationTokenSource? _animationCts;

        public RegisterPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _animationCts = new CancellationTokenSource();
            _ = AnimateBackgroundAsync(_animationCts.Token);

            if (RegisterContainer != null)
            {
                RegisterContainer.Opacity = 0;
                RegisterContainer.TranslationY = 60;
                await Task.WhenAll(
                    RegisterContainer.FadeTo(1, 800, Easing.CubicOut),
                    RegisterContainer.TranslateTo(0, 0, 800, Easing.CubicOut)
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

        private async Task AnimateBackgroundAsync(CancellationToken token)
        {
            await Task.Delay(120);
            var random = new Random();

            while (!token.IsCancellationRequested)
            {
                if (TopGlow == null || BottomGlow == null)
                    break;

                double topX = random.Next(-30, 30);
                double topY = random.Next(-30, 30);
                double bottomX = random.Next(-40, 40);
                double bottomY = random.Next(-40, 40);
                uint duration = (uint)random.Next(4500, 7200);

                try
                {
                    await Task.WhenAll(
                        TopGlow.TranslateTo(topX, topY, duration, Easing.SinInOut),
                        BottomGlow.TranslateTo(bottomX, bottomY, duration, Easing.SinInOut)
                    );
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
                string.IsNullOrWhiteSpace(ConfirmEntry.Text))
            {
                await DisplayAlert("Aviso", "Completa todos los campos.", "OK");
                return;
            }

            if (PasswordEntry.Text != ConfirmEntry.Text)
            {
                await DisplayAlert("Aviso", "Las contraseñas no coinciden.", "OK");
                return;
            }

            await DisplayAlert("Listo", "Usuario registrado (simulado). Ahora inicia sesión.", "OK");
            await Shell.Current.GoToAsync("..");
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnInputFocused(object sender, FocusEventArgs e)
        {
            if (sender is not Entry entry) return;
            if (entry.Parent is not Grid grid) return;
            if (grid.Parent is not Border border) return;

            await border.ScaleTo(1.02, 150, Easing.CubicOut);
        }

        private async void OnInputUnfocused(object sender, FocusEventArgs e)
        {
            if (sender is not Entry entry) return;
            if (entry.Parent is not Grid grid) return;
            if (grid.Parent is not Border border) return;

            await border.ScaleTo(1, 150, Easing.CubicOut);
        }
    }
}

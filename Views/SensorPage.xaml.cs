using System;
using System.Threading;
using System.Threading.Tasks;
using FinanzasApp.ViewModels;
using Microsoft.Maui.Controls;

namespace FinanzasApp.Views
{
    public partial class SensorPage : ContentPage
    {
        private CancellationTokenSource? _animationCts;

        public SensorPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _animationCts = new CancellationTokenSource();
            _ = AnimateBackgroundAsync(_animationCts.Token);

            if (SensorContainer != null)
            {
                SensorContainer.Opacity = 0;
                SensorContainer.TranslationY = 60;
                await Task.WhenAll(
                    SensorContainer.FadeTo(1, 800, Easing.CubicOut),
                    SensorContainer.TranslateTo(0, 0, 800, Easing.CubicOut)
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

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

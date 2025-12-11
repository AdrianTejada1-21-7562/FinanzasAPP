using System;
using System.Threading;
using System.Threading.Tasks;
using FinanzasApp.ViewModels;
using Microsoft.Maui.Controls;

namespace FinanzasApp.Views
{
    public partial class ApiPage : ContentPage
    {
        private CancellationTokenSource? _animationCts;

        public ApiPage(ApiViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _animationCts = new CancellationTokenSource();
            _ = AnimateBackgroundAsync(_animationCts.Token);

            if (ApiContainer != null)
            {
                ApiContainer.Opacity = 0;
                ApiContainer.TranslationY = 60;
                await Task.WhenAll(
                    ApiContainer.FadeTo(1, 800, Easing.CubicOut),
                    ApiContainer.TranslateTo(0, 0, 800, Easing.CubicOut)
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

        private async void OnInputFocused(object sender, FocusEventArgs e)
        {
            if (sender is not View view) return;
            if (view.Parent is Grid grid)
            {
                if (grid.Parent is Border border)
                {
                    await border.ScaleTo(1.02, 150, Easing.CubicOut);
                }
            }
            else if (view.Parent is Border parentBorder)
            {
                await parentBorder.ScaleTo(1.02, 150, Easing.CubicOut);
            }
        }

        private async void OnInputUnfocused(object sender, FocusEventArgs e)
        {
            if (sender is not View view) return;
            if (view.Parent is Grid grid)
            {
                if (grid.Parent is Border border)
                {
                    await border.ScaleTo(1, 150, Easing.CubicOut);
                }
            }
            else if (view.Parent is Border parentBorder)
            {
                await parentBorder.ScaleTo(1, 150, Easing.CubicOut);
            }
        }
    }
}

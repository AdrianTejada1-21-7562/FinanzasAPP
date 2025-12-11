using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FinanzasApp.Models;
using FinanzasApp.Services;
using FinanzasApp.ViewModels;
using Microsoft.Maui.Controls;

namespace FinanzasApp.Views
{
    public partial class DashboardPage : ContentPage
    {
        private readonly DashboardViewModel _viewModel;
        private CancellationTokenSource? _animationCts;

        public DashboardPage()
        {
            InitializeComponent();

            string dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "finanzas.db3");

            var database = new DatabaseService(dbPath);
            _viewModel = new DashboardViewModel(database);

            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _animationCts = new CancellationTokenSource();
            _ = AnimateBackgroundAsync(_animationCts.Token);

            if (MainContainer != null)
            {
                MainContainer.Opacity = 0;
                MainContainer.TranslationY = 50;
                await Task.WhenAll(
                    MainContainer.FadeTo(1, 800, Easing.CubicOut),
                    MainContainer.TranslateTo(0, 0, 800, Easing.CubicOut)
                );
            }

            await _viewModel.LoadAsync();
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

        private async void OnNewMovementClicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(ExpenseDetailPage));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error",
                    $"No se pudo abrir Nuevo movimiento:\n{ex.Message}",
                    "OK");
            }
        }

        private async void OnApiClicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(ApiPage));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error",
                    $"No se pudo abrir la página API:\n{ex.Message}",
                    "OK");
            }
        }

        private async void OnSensorClicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(SensorPage));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error",
                    $"No se pudo abrir la página Sensor:\n{ex.Message}",
                    "OK");
            }
        }

        private async void OnTransactionTapped(object sender, EventArgs e)
        {
            if (sender is Border border && border.BindingContext is Transaction tx)
            {
                await OpenTransactionForEditAsync(tx);
            }
        }

        private async void OnEditSwipeItemInvoked(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipe && swipe.CommandParameter is Transaction tx)
            {
                await OpenTransactionForEditAsync(tx);
            }
        }

        private async Task OpenTransactionForEditAsync(Transaction tx)
        {
            try
            {
                string route = $"{nameof(ExpenseDetailPage)}?TransactionId={tx.Id}";
                await Shell.Current.GoToAsync(route);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error",
                    $"No se pudo abrir el movimiento para editar:\n{ex.Message}",
                    "OK");
            }
        }

        private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipe &&
                swipe.CommandParameter is Transaction tx)
            {
                bool ok = await DisplayAlert("Eliminar",
                    $"¿Eliminar el movimiento \"{tx.Description}\" por {tx.Amount:C}?",
                    "Sí", "No");

                if (!ok) return;

                try
                {
                    await _viewModel.DeleteTransactionAsync(tx);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error",
                        $"No se pudo eliminar el movimiento:\n{ex.Message}",
                        "OK");
                }
            }
        }
    }
}

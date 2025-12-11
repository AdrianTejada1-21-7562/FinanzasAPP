using System;
using System.IO;
using FinanzasApp.Models;
using FinanzasApp.Services;
using FinanzasApp.ViewModels;
using Microsoft.Maui.Controls;

namespace FinanzasApp.Views
{
    public partial class DashboardPage : ContentPage
    {
        private readonly DashboardViewModel _viewModel;

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
            await _viewModel.LoadAsync();
        }

        // =========================================
        // BOTONES
        // =========================================

        private async void OnNewMovementClicked(object sender, EventArgs e)
        {
            try
            {
                // Nuevo -> sin TransactionId
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

        // =========================================
        // ABRIR PARA EDITAR (tap en la card)
        // =========================================

        private async void OnTransactionTapped(object sender, EventArgs e)
        {
            if (sender is Frame frame && frame.BindingContext is Transaction tx)
            {
                await OpenTransactionForEditAsync(tx);
            }
        }

        // =========================================
        // SWIPE EDITAR
        // =========================================

        private async void OnEditSwipeItemInvoked(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipe && swipe.CommandParameter is Transaction tx)
            {
                await OpenTransactionForEditAsync(tx);
            }
        }

        private async System.Threading.Tasks.Task OpenTransactionForEditAsync(Transaction tx)
        {
            try
            {
                // Pasamos el Id como parámetro de query
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

        // =========================================
        // SWIPE ELIMINAR
        // =========================================

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

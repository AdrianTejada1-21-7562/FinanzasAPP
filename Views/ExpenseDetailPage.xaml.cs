using System;
using System.IO;
using System.Threading.Tasks;
using FinanzasApp.Services;
using FinanzasApp.ViewModels;
using Microsoft.Maui.Controls;

namespace FinanzasApp.Views
{
    [QueryProperty(nameof(TransactionId), "TransactionId")]
    public partial class ExpenseDetailPage : ContentPage
    {
        private readonly ExpenseDetailViewModel _viewModel;

        public string? TransactionId { get; set; }

        public ExpenseDetailPage()
        {
            InitializeComponent();

            string dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "finanzas.db3");

            var database = new DatabaseService(dbPath);
            _viewModel = new ExpenseDetailViewModel(database);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!string.IsNullOrWhiteSpace(TransactionId) &&
                int.TryParse(TransactionId, out int id))
            {
                await _viewModel.LoadByIdAsync(id);
            }
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

using System;
using System.IO;
using System.Threading;
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
        private CancellationTokenSource? _animationCts;

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

            _animationCts = new CancellationTokenSource();
            _ = AnimateBackgroundAsync(_animationCts.Token);

            if (DetailContainer != null)
            {
                DetailContainer.Opacity = 0;
                DetailContainer.TranslationY = 60;
                await Task.WhenAll(
                    DetailContainer.FadeTo(1, 800, Easing.CubicOut),
                    DetailContainer.TranslateTo(0, 0, 800, Easing.CubicOut)
                );
            }

            if (!string.IsNullOrWhiteSpace(TransactionId) &&
                int.TryParse(TransactionId, out int id))
            {
                await _viewModel.LoadByIdAsync(id);
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

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnInputFocused(object sender, FocusEventArgs e)
        {
            if (sender is not View view) return;
            if (view.Parent is not Grid grid) return;
            if (grid.Parent is not Border border) return;

            await border.ScaleTo(1.02, 150, Easing.CubicOut);
        }

        private async void OnInputUnfocused(object sender, FocusEventArgs e)
        {
            if (sender is not View view) return;
            if (view.Parent is not Grid grid) return;
            if (grid.Parent is not Border border) return;

            await border.ScaleTo(1, 150, Easing.CubicOut);
        }
    }
}

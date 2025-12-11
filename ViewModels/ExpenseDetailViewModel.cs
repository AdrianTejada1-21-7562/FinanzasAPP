using System;
using System.Threading.Tasks;
using System.Windows.Input;
using FinanzasApp.Models;
using FinanzasApp.Services;
using Microsoft.Maui.Controls;

namespace FinanzasApp.ViewModels
{
    public class ExpenseDetailViewModel : BaseViewModel
    {
        private readonly DatabaseService _db;

        private int _id;
        private string _description = string.Empty;
        private decimal _amount;
        private DateTime _date = DateTime.Today;
        private bool _isIncome;

        public ExpenseDetailViewModel(DatabaseService db)
        {
            _db = db;
            SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);
        }

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public bool IsIncome
        {
            get => _isIncome;
            set => SetProperty(ref _isIncome, value);
        }

        public ICommand SaveCommand { get; }

        // CARGAR EXISTENTE POR ID
        public async Task LoadByIdAsync(int id)
        {
            var all = await _db.GetTransactionsAsync();
            var tx = all.Find(t => t.Id == id);
            if (tx == null) return;

            Id = tx.Id;
            Description = tx.Description;
            Amount = tx.Amount;
            Date = tx.Date;
            IsIncome = tx.IsIncome;
        }

        private async Task SaveAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (string.IsNullOrWhiteSpace(Description))
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Validación",
                        "La descripción es obligatoria.",
                        "OK");
                    return;
                }

                if (Amount <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Validación",
                        "El monto debe ser mayor que cero.",
                        "OK");
                    return;
                }

                var tx = new Transaction
                {
                    Id = Id,
                    Description = Description,
                    Amount = Amount,
                    Date = Date,
                    IsIncome = IsIncome
                };

                await _db.SaveTransactionAsync(tx);

                // Volver al dashboard
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"No se pudo guardar el movimiento:\n{ex.Message}",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

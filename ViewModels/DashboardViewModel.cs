using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FinanzasApp.Models;
using FinanzasApp.Services;

namespace FinanzasApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly DatabaseService _database;

        public ObservableCollection<Transaction> Transactions { get; } = new();

        private decimal totalIngresos;
        public decimal TotalIngresos
        {
            get => totalIngresos;
            set => SetProperty(ref totalIngresos, value);
        }

        private decimal totalGastos;
        public decimal TotalGastos
        {
            get => totalGastos;
            set => SetProperty(ref totalGastos, value);
        }

        // ESTA es la propiedad que usará la ProgressBar (0.0 – 1.0)
        private double gastosProporcion;
        public double GastosProporcion
        {
            get => gastosProporcion;
            set => SetProperty(ref gastosProporcion, value);
        }

        public DashboardViewModel(DatabaseService database)
        {
            _database = database;
            Title = "Panel principal";
        }

        public async Task LoadAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Transactions.Clear();

                var list = await _database.GetTransactionsAsync();
                foreach (var t in list)
                    Transactions.Add(t);

                var totals = await _database.GetTotalsAsync();
                TotalIngresos = totals.ingresos;
                TotalGastos = totals.gastos;

                // cálculo de 0–1 para la ProgressBar
                var totalAbs = TotalIngresos + TotalGastos;
                if (totalAbs > 0)
                    GastosProporcion = (double)(TotalGastos / totalAbs);
                else
                    GastosProporcion = 0;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task DeleteTransactionAsync(Transaction tx)
        {
            await _database.DeleteTransactionAsync(tx);
            await LoadAsync(); // refresca totales y proporción
        }
    }
}

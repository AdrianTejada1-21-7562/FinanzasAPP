using SQLite;
using System;

namespace FinanzasApp.Models
{
    public class Transaction
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Description { get; set; } = string.Empty;

        // Positivo ingreso, negativo gasto
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public bool IsIncome { get; set; }
    }
}

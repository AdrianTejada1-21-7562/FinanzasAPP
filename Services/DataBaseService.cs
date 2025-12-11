using FinanzasApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanzasApp.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;
        private bool _initialized = false;

        public DatabaseService(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
        }

        private async Task InitAsync()
        {
            if (_initialized) return;

            await _db.CreateTableAsync<User>();
            await _db.CreateTableAsync<Transaction>();

            // Usuario por defecto
            var count = await _db.Table<User>().CountAsync();
            if (count == 0)
            {
                await _db.InsertAsync(new User
                {
                    Username = "admin",
                    Password = "123"
                });
            }

            _initialized = true;
        }

        // ========= USUARIOS =========

        public async Task<User?> GetUserAsync(string username, string password)
        {
            await InitAsync();
            return await _db.Table<User>()
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            await InitAsync();
            var user = await _db.Table<User>()
                .FirstOrDefaultAsync(u => u.Username == username);
            return user != null;
        }

        public async Task<int> AddUserAsync(string username, string password)
        {
            await InitAsync();
            var user = new User
            {
                Username = username,
                Password = password
            };
            return await _db.InsertAsync(user);
        }

        // ========= TRANSACCIONES =========

        public async Task<List<Transaction>> GetTransactionsAsync()
        {
            await InitAsync();
            return await _db.Table<Transaction>()
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<int> SaveTransactionAsync(Transaction transaction)
        {
            await InitAsync();
            if (transaction.Id != 0)
                return await _db.UpdateAsync(transaction);
            else
                return await _db.InsertAsync(transaction);
        }

        public async Task<int> DeleteTransactionAsync(Transaction transaction)
        {
            await InitAsync();
            return await _db.DeleteAsync(transaction);
        }

        public async Task<(decimal ingresos, decimal gastos)> GetTotalsAsync()
        {
            await InitAsync();
            var list = await GetTransactionsAsync();
            decimal ingresos = 0;
            decimal gastos = 0;

            foreach (var t in list)
            {
                if (t.IsIncome)
                    ingresos += t.Amount;
                else
                    gastos += t.Amount;
            }

            return (ingresos, gastos);
        }
    }
}

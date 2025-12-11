using FinanzasApp.Services;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinanzasApp.ViewModels
{
    public class ApiViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        public ObservableCollection<ApiPost> Posts { get; } = new();

        public ICommand LoadCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        public ApiViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "API REST";

            LoadCommand = new Command(async () => await LoadAsync());
            CreateCommand = new Command(async () => await CreateAsync());
            UpdateCommand = new Command(async () => await UpdateAsync());
            DeleteCommand = new Command(async () => await DeleteAsync());
        }

        private static Page? GetCurrentPage()
        {
            var app = Application.Current;
            if (app == null || app.Windows.Count == 0)
                return null;
            return app.Windows[0].Page;
        }

        private static Task ShowAlertAsync(string title, string message, string cancel)
        {
            var page = GetCurrentPage();
            if (page == null) return Task.CompletedTask;
            return page.DisplayAlert(title, message, cancel);
        }

        private async Task LoadAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Posts.Clear();
                var list = await _apiService.GetPostsAsync();
                foreach (var p in list)
                    Posts.Add(p);
            }
            catch (System.Exception ex)
            {
                await ShowAlertAsync("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CreateAsync()
        {
            try
            {
                var post = new ApiPost
                {
                    userId = 1,
                    title = "Ejemplo POST",
                    body = "Creado desde la app MAUI."
                };

                var result = await _apiService.CreatePostAsync(post);
                await ShowAlertAsync("POST", $"Creado id {result.id}", "OK");
            }
            catch (System.Exception ex)
            {
                await ShowAlertAsync("Error", ex.Message, "OK");
            }
        }

        private async Task UpdateAsync()
        {
            if (Posts.Count == 0)
            {
                await ShowAlertAsync("Info", "Primero ejecuta GET.", "OK");
                return;
            }

            try
            {
                var first = Posts.FirstOrDefault();
                if (first == null)
                    return;

                first.title = "Título actualizado";
                var result = await _apiService.UpdatePostAsync(first.id, first);
                await ShowAlertAsync("PUT", $"Actualizado id {result.id}", "OK");
            }
            catch (System.Exception ex)
            {
                await ShowAlertAsync("Error", ex.Message, "OK");
            }
        }

        private async Task DeleteAsync()
        {
            if (Posts.Count == 0)
            {
                await ShowAlertAsync("Info", "Primero ejecuta GET.", "OK");
                return;
            }

            try
            {
                var first = Posts.FirstOrDefault();
                if (first == null)
                    return;

                var ok = await _apiService.DeletePostAsync(first.id);
                await ShowAlertAsync("DELETE", ok ? "Eliminado (simulado)" : "Error al eliminar", "OK");
            }
            catch (System.Exception ex)
            {
                await ShowAlertAsync("Error", ex.Message, "OK");
            }
        }
    }
}

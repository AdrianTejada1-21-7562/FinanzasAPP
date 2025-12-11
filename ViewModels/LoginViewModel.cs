using System.Threading.Tasks;
using System.Windows.Input;
using FinanzasApp.Services;
using FinanzasApp.Views;

namespace FinanzasApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _message = string.Empty;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public LoginViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Inicio de sesión";

            // Para que veas rápido que funciona el login por defecto
            Username = "admin";

            LoginCommand = new Command(async () => await LoginAsync(), () => !IsBusy);
            RegisterCommand = new Command(async () => await GoToRegisterAsync(), () => !IsBusy);
        }

        private async Task LoginAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            Message = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    Message = "Escribe usuario y contraseña.";
                    return;
                }

                var user = await _databaseService.GetUserAsync(Username, Password);
                if (user == null)
                {
                    Message = "Credenciales incorrectas.";
                    return;
                }

                // Navegar al panel principal
                await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
            }
            finally
            {
                IsBusy = false;
                (LoginCommand as Command)?.ChangeCanExecute();
                (RegisterCommand as Command)?.ChangeCanExecute();
            }
        }

        private async Task GoToRegisterAsync()
        {
            if (IsBusy) return;
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }
    }
}

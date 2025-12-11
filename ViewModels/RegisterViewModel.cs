using System.Threading.Tasks;
using System.Windows.Input;
using FinanzasApp.Services;

namespace FinanzasApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public ICommand RegisterCommand { get; }

        public RegisterViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Registrar usuario";

            RegisterCommand = new Command(async () => await RegisterAsync(), () => !IsBusy);
        }

        private async Task RegisterAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            Message = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(Username) ||
                    string.IsNullOrWhiteSpace(Password) ||
                    string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    Message = "Completa todos los campos.";
                    return;
                }

                if (Password != ConfirmPassword)
                {
                    Message = "Las contraseñas no coinciden.";
                    return;
                }

                var exists = await _databaseService.UserExistsAsync(Username);
                if (exists)
                {
                    Message = "Ese usuario ya existe.";
                    return;
                }

                await _databaseService.AddUserAsync(Username, Password);
                Message = "Usuario registrado correctamente.";

                Username = string.Empty;
                Password = string.Empty;
                ConfirmPassword = string.Empty;
            }
            finally
            {
                IsBusy = false;
                (RegisterCommand as Command)?.ChangeCanExecute();
            }
        }
    }
}

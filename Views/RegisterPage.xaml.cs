using System;
using Microsoft.Maui.Controls;

namespace FinanzasApp.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // De momento solo simulamos el registro.
            // Más adelante podemos conectarlo a DatabaseService si quieres.
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
                string.IsNullOrWhiteSpace(ConfirmEntry.Text))
            {
                await DisplayAlert("Aviso", "Completa todos los campos.", "OK");
                return;
            }

            if (PasswordEntry.Text != ConfirmEntry.Text)
            {
                await DisplayAlert("Aviso", "Las contraseñas no coinciden.", "OK");
                return;
            }

            await DisplayAlert("Listo", "Usuario registrado (simulado). Ahora inicia sesión.", "OK");

            // Volver al login
            await Shell.Current.GoToAsync("..");
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

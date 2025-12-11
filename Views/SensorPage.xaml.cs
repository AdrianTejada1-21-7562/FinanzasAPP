using FinanzasApp.ViewModels;
using Microsoft.Maui.Controls;

namespace FinanzasApp.Views
{
    public partial class SensorPage : ContentPage
    {
        public SensorPage(SensorViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

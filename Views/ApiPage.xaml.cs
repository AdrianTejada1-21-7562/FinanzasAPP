using FinanzasApp.ViewModels;
using Microsoft.Maui.Controls;

namespace FinanzasApp.Views
{
    public partial class ApiPage : ContentPage
    {
        public ApiPage(ApiViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}

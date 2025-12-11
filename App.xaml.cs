using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace FinanzasApp
{
    public partial class App : Application
    {
        // ÚNICO constructor
        public App()
        {
            InitializeComponent();
        }

        // ÚNICA implementación de CreateWindow
        protected override Window CreateWindow(IActivationState? activationState)
        {
            // AppShell es tu Shell principal
            return new Window(new AppShell());
        }
    }
}

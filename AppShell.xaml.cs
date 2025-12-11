using FinanzasApp.Views;

namespace FinanzasApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Rutas de navegación usadas en toda la app
            Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
            Routing.RegisterRoute(nameof(ExpenseDetailPage), typeof(ExpenseDetailPage));
            Routing.RegisterRoute(nameof(ApiPage), typeof(ApiPage));
            Routing.RegisterRoute(nameof(SensorPage), typeof(SensorPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        }
    }
}

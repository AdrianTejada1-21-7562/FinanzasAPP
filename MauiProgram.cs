using FinanzasApp.Services;
using FinanzasApp.ViewModels;
using FinanzasApp.Views;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using System;
using System.IO;

namespace FinanzasApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
                });

            string dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "finanzas.db3");

            builder.Services.AddSingleton(new DatabaseService(dbPath));
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<ApiService>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<ExpenseDetailViewModel>();
            builder.Services.AddTransient<SensorViewModel>();
            builder.Services.AddTransient<ApiViewModel>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<ExpenseDetailPage>();
            builder.Services.AddTransient<SensorPage>();
            builder.Services.AddTransient<ApiPage>();

            return builder.Build();
        }
    }
}

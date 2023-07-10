using System;
using System.Windows;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

using EchoV3.Windows;
using EchoV3.Services;
using EchoV3.Utility;
using Microsoft.Web.WebView2.Core;

namespace EchoV3
{
    static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var application = new App();
            application.Run();
        }
    }

    public partial class App : Application
    {
        public static FFXIVStateManager? FFXIVState;
        private ServiceProvider serviceProvider;

        public App()
        {
            // DI
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
            // WPF
            InitializeComponent();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<FFXIVEventService>();
            services.AddSingleton<DeucalionService>();
            services.AddSingleton<InjectionService>();
            services.AddSingleton<MainWindow>();
        }

        private void OnStartup(object sender, StartupEventArgs eventArgs)
        {
            // Validate that WebView2 is installed!
            try
            {
                var version = CoreWebView2Environment.GetAvailableBrowserVersionString();
            }
            catch (WebView2RuntimeNotFoundException exception)
            {
                // Handle the runtime not being installed.
                MessageBox.Show(exception.Message, "Dependency not installed", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }

            FFXIVState = new FFXIVStateManager(serviceProvider);
            var wnd = serviceProvider.GetRequiredService<MainWindow>();
            if (wnd != null)
            {
                wnd.Show();
            }
            else
            {
                MessageBox.Show("Failed to acquire a handle to MainWindow!", "An error occurred while starting Echo", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }
    }
}

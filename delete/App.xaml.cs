using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace DataObliterate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Unhandled exception: {e.Exception.Message}");
            e.Handled = true;
        }

    }

}

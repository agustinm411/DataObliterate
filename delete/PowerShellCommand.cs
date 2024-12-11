using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DeletionMaster
{
    public class PowerShellCommand
    {
        public async Task ExecutePowerShellCommandAsync(string command)
        {
            try
            {
                ProgressBar progressBar = new ProgressBar
                {
                    Minimum = 0,
                    Maximum = 100,
                    Value = 0,
                    Visibility = Visibility.Visible
                };

                Window progressWindow = new Window
                {
                    Title = "Ejecución en progreso",
                    Content = progressBar,
                    Width = 400,
                    Height = 100,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                progressWindow.Show();

                await Task.Run(() =>
                {
                    using (PowerShell powerShell = PowerShell.Create())
                    {
                        powerShell.AddScript(command);

                        powerShell.Streams.Progress.DataAdded += (sender, e) =>
                        {
                            var progressRecord = ((PSDataCollection<ProgressRecord>)sender)[e.Index];
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                progressBar.Value = progressRecord.PercentComplete;
                            });
                        };

                        var results = powerShell.Invoke();

                        if (powerShell.HadErrors)
                        {
                            throw new Exception("Se produjo un error durante la ejecución del comando de PowerShell.");
                        }
                    }
                });

                progressWindow.Close();

                MessageBox.Show("La operación ha finalizado con éxito.", "Operación completada", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al ejecutar el comando de PowerShell: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

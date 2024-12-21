using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DeletionMaster
{
    public class PowerShellCommand
    {
        public async Task<string> ExecutePowerShellCommandAsync(string command)
        {
            string output = string.Empty;
            Window progressWindow = null;
            ProgressBar progressBar = null;
            try
            {
                progressBar = new ProgressBar
                {
                    Minimum = 0,
                    Maximum = 100,
                    Value = 0,
                    Visibility = Visibility.Visible
                };

                progressWindow = new Window
                {
                    Title = "Ejecución en progreso",
                    Content = progressBar,
                    Width = 400,
                    Height = 100,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                progressWindow.Show();

                // Ejecutar el comando de PowerShell en una tarea asíncrona
                output = await Task.Run(() =>
                {
                    using (PowerShell powerShell = PowerShell.Create())
                    {
                        try
                        {
                            powerShell.AddScript(command);

                            // Manejo de progreso
                            powerShell.Streams.Progress.DataAdded += (sender, e) =>
                            {
                                var progressRecord = ((PSDataCollection<ProgressRecord>)sender)[e.Index];
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    progressBar.Value = progressRecord.PercentComplete;
                                });
                            };

                            // Invocar el comando
                            var results = powerShell.Invoke();

                            // Verificar si hubo errores
                            if (powerShell.HadErrors)
                            {
                                throw new Exception("Se produjo un error durante la ejecución del comando de PowerShell.");
                            }

                            // Recoger y concatenar la salida del comando
                            StringBuilder stringBuilder = new StringBuilder();
                            foreach (var result in results)
                            {
                                stringBuilder.AppendLine(result.ToString());
                            }

                            return stringBuilder.ToString();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Error al procesar el comando de PowerShell: {ex.Message}");
                        }
                    }
                });

                progressWindow.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al ejecutar el comando de PowerShell: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                progressWindow?.Close();
            }

            return output;
        }
    }
}

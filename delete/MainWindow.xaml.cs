using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Microsoft.Win32;
using System.Management.Automation.Runspaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DeletionMaster
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> Files { get; set; }
        int totalItemsToDelete;
        Elements elements = new Elements();
        public MainWindow()
        {
            InitializeComponent();
elements.RequestElevation();
            Files = new ObservableCollection<string>();
            listBox.ItemsSource = Files;
            buttonCancel.Visibility = Visibility.Collapsed;
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] selectedFiles = DialogService.OpenFiles();
                if (selectedFiles != null)
                {
                    foreach (string file in selectedFiles)
                    {
                        if (!Files.Contains(file))
                        {
                            Files.Add(file);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonBrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] selectedFolders = DialogService.OpenFolders();
                if (selectedFolders != null)
                {
                    foreach (string folder in selectedFolders)
                    {
                        if (!Files.Contains(folder))
                        {
                            Files.Add(folder);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private CancellationTokenSource _cancellationTokenSource;

        private async void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
                        try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                bool isConfirmed = DialogService.Confirm("¿Estás seguro de que deseas eliminar los archivos seleccionados?");
                if (!isConfirmed) return;
                var itemsToRemove = new List<string>(Files);
                totalItemsToDelete = elements.CountFilesAndFolders(itemsToRemove);
                if (totalItemsToDelete == 0)
                {
                    MessageBox.Show("No hay archivos para eliminar.");
                    return;
                }

                UIUpdate uiUpdate = new UIUpdate();
                                progressBar.Maximum = totalItemsToDelete;
                uiUpdate.PrepareUIForDeletion(buttonBrowse, buttonBrowseFolder, radioButtonSimple, radioButtonGutman, buttonDelete, listBox, buttonCancel, progressBar, Files);

                bool wasCancelled = false;

                bool isSimpleChecked = radioButtonSimple.IsChecked == true;
                bool isGutmanChecked = radioButtonGutman.IsChecked == true;

                await Task.Run(async () =>
                {
                    DeletionService deletion = new DeletionService();

                    Action incrementProgress = () => Dispatcher.Invoke(() =>
                    {
                        progressBar.Value += 1;
                        progressBar.InvalidateVisual();
                    });

                    for (int i = 0; i < itemsToRemove.Count; i++)
                    {
                        if (_cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            wasCancelled = true;
                            break;
                        }

                        string file = itemsToRemove[i];


                        if (isSimpleChecked)
                        {
                            deletion.DeleteFiles(file, incrementProgress);
                        }
                        else if (isGutmanChecked)
                        {
                            deletion.GutmanDeleteFiles(file, incrementProgress);
                        }

                        await Dispatcher.InvokeAsync(() =>
                        {
                            Files.Remove(file);
                        });
                    }
                }, _cancellationTokenSource.Token);

                uiUpdate.RestoreUIAfterDeletion(progressBar, buttonCancel, buttonBrowse, buttonBrowseFolder, radioButtonSimple, radioButtonGutman, buttonDelete, listBox);

                MessageBox.Show(wasCancelled ? "Eliminación cancelada." : "Eliminación completada.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
            }
                    }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }
            UIUpdate uiUpdate = new UIUpdate();
            uiUpdate.RestoreUIAfterDeletion(progressBar, buttonCancel, buttonBrowse, buttonBrowseFolder, radioButtonSimple, radioButtonGutman, buttonDelete, listBox);
            MessageBox.Show("Eliminación cancelada.");
        }

        private void buttonAbout_Click(object sender, RoutedEventArgs e)
        {
            string aboutMessage = "Esta es la versión 0.01 beta 1 del software.\n" +
                                  "Desarrollado por Agustín Martínez.\n" +
                                  "Es un software de código abierto con licencia GPL v2.\n\n" +
                                  "Bajo esta licencia, tienes la libertad de:\n" +
                                  "- Usar el software para cualquier propósito.\n" +
                                  "- Estudiar cómo funciona y modificarlo.\n" +
                                  "- Redistribuir copias del software.\n" +
                                  "- Mejorar el software y compartir tus mejoras.\n\n" +
                                  "Consulta el texto completo de la licencia para más detalles.";
            MessageBox.Show(aboutMessage, "Acerca de DeletionMaster", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void CleanReservedSpace_Click(object sender, RoutedEventArgs e)
        {
PowerShellCommand command = new PowerShellCommand();
                string result = await command.ExecutePowerShellCommandAsync("DISM.exe /Online /Get-ReservedStorageState");
            if (!result.Contains("deshabilitado", StringComparison.OrdinalIgnoreCase))
            {
                await command.ExecutePowerShellCommandAsync("DISM.exe /Online /Set-ReservedStorageState /State:Disabled");
                MessageBox.Show("Almacenamiento reservado deshabilitado con éxito.");
            }
            else
            {
                MessageBox.Show("El almacenamiento reservado ya está desactivado.");
            }
        }

        private async void CleanHibernateFile_Click(object sender, RoutedEventArgs e)
        {
PowerShellCommand command = new PowerShellCommand();
            string hibernationFilePath = @"C:\hiberfil.sys";
            if (File.Exists(hibernationFilePath))
            {
                await command.ExecutePowerShellCommandAsync("powercfg -h off");
                MessageBox.Show("El archivo de hibernación ha sido eliminado correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("El archivo de hibernación no existe. Ya fue eliminado.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
        }

        private void AtaSecureErase_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("esta función aún no está implementada. Estará en la próxima versión.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void CleanWindowsSXS_Click(object sender, RoutedEventArgs e)
        {
PowerShellCommand command = new PowerShellCommand();
            await command.ExecutePowerShellCommandAsync("Dism.exe /online /Cleanup-Image /StartComponentCleanup\r\n");
                    }
    }
}

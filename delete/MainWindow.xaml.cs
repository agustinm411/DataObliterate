using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace DataObliterate
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

                // Validar los RadioButton en el hilo de la interfaz
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

                        // Realiza la eliminación en el hilo de fondo
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
                _cancellationTokenSource?.Dispose(); // Asegúrate de liberar siempre el token
            }
                    }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose(); // Liberar recursos del token de cancelación
            }
            UIUpdate uiUpdate = new UIUpdate();
            uiUpdate.RestoreUIAfterDeletion(progressBar, buttonCancel, buttonBrowse, buttonBrowseFolder, radioButtonSimple, radioButtonGutman, buttonDelete, listBox);
            MessageBox.Show("Eliminación cancelada.");
        }
    }
}

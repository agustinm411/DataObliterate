using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataObliterate
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> Files { get; set; }

        public MainWindow()
        {
            InitializeComponent();
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
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                bool isConfirmed = DialogService.Confirm("¿Estás seguro de que deseas eliminar los archivos seleccionados?");
                if (isConfirmed)
                {
                    var itemsToRemove = new List<string>(Files);
                    int totalItems = itemsToRemove.Count;

                    if (totalItems == 0)
                    {
                        MessageBox.Show("No hay archivos para eliminar.");
                        return;
                    }
                    UIUpdate uIUpdate = new UIUpdate();
                    uIUpdate .PrepareUIForDeletion(buttonBrowse, buttonBrowseFolder, radioButtonSimple, radioButtonGutman, buttonDelete, listBox, buttonCancel, progressBar, Files);
                    bool wasCancelled = false;
                    var deletedFiles = new List<string>();

                    await Task.Run(() =>
                    {
                        DeletionService deletion = new DeletionService();
                        for (int i = 0; i < totalItems; i++)
                        {
                            if (_cancellationTokenSource.Token.IsCancellationRequested)
                            {
                                wasCancelled = true;
                                break;
                            }

                            string file = itemsToRemove[i];

                            bool isSimpleChecked = false;
                            bool isGutmanChecked = false;

                            Dispatcher.Invoke(() =>
                            {
                                isSimpleChecked = radioButtonSimple.IsChecked == true;
                                isGutmanChecked = radioButtonGutman.IsChecked == true;
                            });

                            if (isSimpleChecked)
                            {
                                deletion.DeleteFiles(file);
                            }
                            else if (isGutmanChecked)
                            {
                                deletion.GutmanDeleteFiles(file);
                            }

                            Dispatcher.Invoke(() =>
                            {
                                deletedFiles.Add(file);
                                Files.Remove(file);
                                progressBar.Value = i + 1;
                            });
                        }
                    }, _cancellationTokenSource.Token);

                    uIUpdate .RestoreUIAfterDeletion(progressBar, buttonCancel, buttonBrowse, buttonBrowseFolder, radioButtonSimple, radioButtonGutman, buttonDelete, listBox);

                    if (wasCancelled)
                    {
                        MessageBox.Show("Eliminación cancelada.");
                    }
                    else
                    {
                        MessageBox.Show("Eliminación completada.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            progressBar.Visibility = Visibility.Collapsed;
            MessageBox.Show("Eliminación cancelada.");
            buttonBrowse.Visibility = Visibility.Visible;
            buttonBrowseFolder.Visibility = Visibility.Visible;
            radioButtonSimple.Visibility = Visibility.Visible;
            radioButtonGutman.Visibility = Visibility.Visible;
            buttonDelete.Visibility = Visibility.Visible;
            listBox.Visibility = Visibility.Visible;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;

namespace DeletionMaster
{
    public class UIUpdate
    {
        public void PrepareUIForDeletion(Button buttonBrowse, Button buttonBrowseFolder, RadioButton radioButtonSimple, RadioButton radioButtonGutman, Button buttonDelete, ListBox listBox, Button buttonCancel, ProgressBar progressBar, ObservableCollection<string> files)
        {
Elements elements = new Elements();
            List<string> ListElements = new List<string>(files);
                        int totalItemsToDelete = elements.CountFilesAndFolders(ListElements);
            progressBar.Maximum = totalItemsToDelete;
            progressBar.Value = 0;

            buttonBrowse.Visibility = Visibility.Collapsed;
            buttonBrowseFolder.Visibility = Visibility.Collapsed;
            radioButtonSimple.Visibility = Visibility.Collapsed;
            radioButtonGutman.Visibility = Visibility.Collapsed;
            buttonDelete.Visibility = Visibility.Collapsed;
            listBox.Visibility = Visibility.Collapsed;

            buttonCancel.Visibility = Visibility.Visible;
            progressBar.Minimum = 0;
            progressBar.Visibility = Visibility.Visible;
        }

        public void RestoreUIAfterDeletion(ProgressBar progressBar, Button buttonCancel, Button buttonBrowse, Button buttonBrowseFolder, RadioButton radioButtonSimple, RadioButton radioButtonGutman, Button buttonDelete, ListBox listBox)
        {
            progressBar.Visibility = Visibility.Collapsed;
            buttonCancel.Visibility = Visibility.Collapsed;
            buttonBrowse.Visibility = Visibility.Visible;
            buttonBrowseFolder.Visibility = Visibility.Visible;
            radioButtonSimple.Visibility = Visibility.Visible;
            radioButtonGutman.Visibility = Visibility.Visible;
            buttonDelete.Visibility = Visibility.Visible;
            listBox.Visibility = Visibility.Visible;
        }
    }
}

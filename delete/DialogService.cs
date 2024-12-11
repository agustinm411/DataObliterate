using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;

namespace DeletionMaster
{
    public static class DialogService
    {
        public static string[] OpenFiles()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "Todos los archivos (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                    return openFileDialog.FileNames;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Error al mostrar el cuadro de diálogo: " + ex.Message);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                MessageBox.Show("Error del sistema al intentar abrir el cuadro de diálogo: " + ex.Message);
            }
            return null;
        }

        public static string[] OpenFolders()
        {
            try
            {
                OpenFolderDialog openFolderDialog = new OpenFolderDialog();
                openFolderDialog.Multiselect = true;
                if (openFolderDialog.ShowDialog() == true)
                    return openFolderDialog.FolderNames;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Error al mostrar el cuadro de diálogo: " + ex.Message);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                MessageBox.Show("Error del sistema al intentar abrir el cuadro de diálogo: " + ex.Message);
            }
            return null;
        }

        public static bool Confirm(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, "Confirmar la supresión de archivos", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }


    }
}

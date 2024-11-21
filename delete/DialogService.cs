using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;

namespace delete
{
    public static class DialogService
    {
        public static string[] OpenFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Todos los archivos (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileNames;
            return null;
        }

  public static string [] OpenFolders()
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            openFolderDialog.Multiselect = true;
                        if (openFolderDialog.ShowDialog() == true)
                return openFolderDialog.FolderNames;
            return null;
                    }  
        public static bool Confirm(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, "Confirmar la supresión de archivos", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }


    }
}

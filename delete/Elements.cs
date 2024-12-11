using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DeletionMaster
{
    public class Elements
    {
        public List<string> GetFilesAndFolders(List<string> elements)
        {
            List<string> foundElements = new List<string>();
            foreach (var element in elements)
            {
                try
                {
                    if (File.Exists(element))
                    {
                        foundElements.Add(element);
                    }
                    else if (Directory.Exists(element))
                    {
                        foundElements.AddRange(GetDirectoryContents(element));
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"No se tiene acceso a {element}:\n{ex.Message}", "Error de acceso", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (PathTooLongException ex)
                {
                    MessageBox.Show($"La ruta es demasiado larga: {element}:\n{ex.Message}", "Error de ruta", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (DirectoryNotFoundException ex)
                {
                    MessageBox.Show($"El directorio no existe: {element}:\n{ex.Message}", "Error de directorio", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Error de entrada/salida al procesar {element}:\n{ex.Message}", "Error de E/S", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Restricción de seguridad al intentar acceder a {element}:\n{ex.Message}", "Error de seguridad", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inesperado al procesar {element}:\n{ex.Message}", "Error desconocido", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return foundElements;
        }

        private List<string> GetDirectoryContents(string directoryPath)
        {
            List<string> contents = new List<string>();

            try
            {
                contents.AddRange(Directory.GetFiles(directoryPath));
                var subdirectories = Directory.GetDirectories(directoryPath);
                foreach (var subdirectory in subdirectories)
                {
                    contents.Add(subdirectory);
                    contents.AddRange(GetDirectoryContents(subdirectory));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"No se tiene acceso al directorio {directoryPath}:\n{ex.Message}", "Error de acceso", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (PathTooLongException ex)
            {
                MessageBox.Show($"La ruta del directorio es demasiado larga:\n{directoryPath}\n{ex.Message}", "Error de ruta", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show($"El directorio no existe:\n{directoryPath}\n{ex.Message}", "Error de directorio", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Error de entrada/salida en el directorio {directoryPath}:\n{ex.Message}", "Error de E/S", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SecurityException ex)
            {
                MessageBox.Show($"Restricción de seguridad al intentar acceder al directorio {directoryPath}:\n{ex.Message}", "Error de seguridad", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al procesar el directorio {directoryPath}:\n{ex.Message}", "Error desconocido", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return contents;
        }

        public int CountFilesAndFolders(List<string> elements)
        {
            int totalCount = 0;

            foreach (var element in elements)
            {
                try
                {
                    if (File.Exists(element))
                    {
                        totalCount++;
                    }
                    else if (Directory.Exists(element))
                    {
                        totalCount += CountDirectoryContents(element);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"No se tiene acceso a {element}:\n{ex.Message}", "Error de acceso", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (PathTooLongException ex)
                {
                    MessageBox.Show($"La ruta es demasiado larga:\n{element}\n{ex.Message}", "Error de ruta", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (DirectoryNotFoundException ex)
                {
                    MessageBox.Show($"El directorio no existe:\n{element}\n{ex.Message}", "Error de directorio", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Error de entrada/salida al procesar {element}:\n{ex.Message}", "Error de E/S", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Restricción de seguridad al intentar acceder a {element}:\n{ex.Message}", "Error de seguridad", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inesperado al procesar {element}:\n{ex.Message}", "Error desconocido", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return totalCount;
        }

        private int CountDirectoryContents(string directoryPath)
        {
            int count = 0;

            try
            {
                count += Directory.GetFiles(directoryPath).Length;
                var subdirectories = Directory.GetDirectories(directoryPath);
                foreach (var subdirectory in subdirectories)
                {
                    count++;
                    count += CountDirectoryContents(subdirectory);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"No se tiene acceso al directorio {directoryPath}:\n{ex.Message}", "Error de acceso", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (PathTooLongException ex)
            {
                MessageBox.Show($"La ruta del directorio es demasiado larga:\n{directoryPath}\n{ex.Message}", "Error de ruta", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show($"El directorio no existe:\n{directoryPath}\n{ex.Message}", "Error de directorio", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Error de entrada/salida al procesar el directorio {directoryPath}:\n{ex.Message}", "Error de E/S", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SecurityException ex)
            {
                MessageBox.Show($"Restricción de seguridad al intentar acceder al directorio {directoryPath}:\n{ex.Message}", "Error de seguridad", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al procesar el directorio {directoryPath}:\n{ex.Message}", "Error desconocido", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return count;
        }

        public bool RequestElevation()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

                if (isAdmin)
                {
                    return true;
                }

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = Process.GetCurrentProcess().MainModule.FileName,
                    Verb = "runas"
                };

                Process.Start(startInfo);
                Environment.Exit(0);
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"No se pudo solicitar permisos de administrador:\n{ex.Message}", "Error de permisos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                MessageBox.Show($"Error del sistema al intentar ejecutar con privilegios elevados:\n{ex.Message}", "Error del sistema", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Operación inválida al solicitar permisos de administrador:\n{ex.Message}", "Error de operación", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"El archivo ejecutable principal no se encontró:\n{ex.Message}", "Error de archivo faltante", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al intentar solicitar permisos de administrador:\n{ex.Message}", "Error desconocido", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DeletionMaster
{
    public class DeletionService
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public void DeleteFiles(string path, Action incrementProgress)
        {
            try
            {
                if (File.Exists(path))
                {
                    var fileInfo = new FileInfo(path);
                    long length = fileInfo.Length;

                    using (var stream = new FileStream(path, FileMode.Open, FileAccess.Write))
                    {
                        byte[] buffer = new byte[1024];
                        for (long i = 0; i < length; i += buffer.Length)
                        {
                            if (_cancellationTokenSource.Token.IsCancellationRequested)
                            {
                                return;
                            }
                            stream.Write(buffer, 0, buffer.Length);
                        }
                    }
                    File.Delete(path);
                    incrementProgress();
                }
                else if (Directory.Exists(path))
                {
                    var directoryInfo = new DirectoryInfo(path);
                    foreach (var file in directoryInfo.GetFiles())
                    {
                        DeleteFiles(file.FullName, incrementProgress);
                    }
                    foreach (var dir in directoryInfo.GetDirectories())
                    {
                        DeleteFiles(dir.FullName, incrementProgress);
                    }
                    Directory.Delete(path, true);
                    incrementProgress();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("No tienes permiso para acceder o eliminar: " + path + "\n" + ex.Message);
            }
            catch (PathTooLongException ex)
            {
                MessageBox.Show("La ruta es demasiado larga: " + path + "\n" + ex.Message);
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("El directorio no existe o fue eliminado: " + path + "\n" + ex.Message);
            }
            catch (IOException ex) when ((ex.HResult & 0x0000FFFF) == 32)
            {
                                GC.Collect();
                GC.WaitForPendingFinalizers();
                MessageBox.Show("El archivo está siendo utilizado por otro proceso: " + path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void GutmanDeleteFiles(string path, Action incrementProgress)
        {
            try
            {
                if (File.Exists(path))
                {
                    var fileInfo = new FileInfo(path);
                    long length = fileInfo.Length;
                    Random random = new Random();

                    for (int pass = 0; pass < 35; pass++)
                    {
                        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Write))
                        {
                            byte[] buffer = new byte[1024];
                            for (long i = 0; i < length; i += buffer.Length)
                            {
                                if (_cancellationTokenSource.Token.IsCancellationRequested)
                                {
                                    return;
                                }
                                random.NextBytes(buffer);
                                stream.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }
                    File.Delete(path);
                    incrementProgress();
                }
                else if (Directory.Exists(path))
                {
                    var directoryInfo = new DirectoryInfo(path);
                    foreach (var file in directoryInfo.GetFiles())
                    {
                        GutmanDeleteFiles(file.FullName, incrementProgress);
                    }
                    foreach (var dir in directoryInfo.GetDirectories())
                    {
                        GutmanDeleteFiles(dir.FullName, incrementProgress);
                    }
                    Directory.Delete(path, true);
                    incrementProgress();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("No tienes permiso para acceder o eliminar: " + path + "\n" + ex.Message);
            }
            catch (PathTooLongException ex)
            {
                MessageBox.Show("La ruta es demasiado larga: " + path + "\n" + ex.Message);
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("El directorio no existe o fue eliminado: " + path + "\n" + ex.Message);
            }
            catch (IOException ex) when ((ex.HResult & 0x0000FFFF) == 32)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                MessageBox.Show("El archivo está siendo utilizado por otro proceso: " + path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}

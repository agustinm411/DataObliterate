using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DataObliterate
{
    public class DeletionService
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public void DeleteFiles(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    var fileInfo = new System.IO.FileInfo(path);
                    long length = fileInfo.Length;

                    using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Write))
                    {
                        byte[] buffer = new byte[1024];
                        for (long i = 0; i < length; i += buffer.Length)
                        {
                            if (_cancellationTokenSource.Token.IsCancellationRequested)
                            {
                                // Si la operación se cancela, salimos del método y cerramos el flujo
                                stream.Close();
                                return;
                            }
                            stream.Write(buffer, 0, buffer.Length);
                        }
                    }
                    System.IO.File.Delete(path);
                }
                else if (System.IO.Directory.Exists(path))
                {
                    var directoryInfo = new System.IO.DirectoryInfo(path);
                    foreach (var file in directoryInfo.GetFiles())
                    {
                        DeleteFiles(file.FullName);
                    }
                    foreach (var dir in directoryInfo.GetDirectories())
                    {
                        DeleteFiles(dir.FullName);
                    }
                    System.IO.Directory.Delete(path, true);
                }
            }
            catch (IOException ex) when ((ex.HResult & 0x0000FFFF) == 32) // ERROR_SHARING_VIOLATION
            {
                MessageBox.Show("El archivo está siendo utilizado por otro proceso: " + path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void GutmanDeleteFiles(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    var fileInfo = new System.IO.FileInfo(path);
                    long length = fileInfo.Length;
                    Random random = new Random();

                    for (int pass = 0; pass < 35; pass++)
                    {
                        using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Write))
                        {
                            byte[] buffer = new byte[1024];
                            for (long i = 0; i < length; i += buffer.Length)
                            {
                                if (_cancellationTokenSource.Token.IsCancellationRequested)
                                {
                                    // Si la operación se cancela, salimos del método y cerramos el flujo
                                    stream.Close();
                                    return;
                                }
                                random.NextBytes(buffer);
                                stream.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }
                    System.IO.File.Delete(path);
                }
                else if (System.IO.Directory.Exists(path))
                {
                    var directoryInfo = new System.IO.DirectoryInfo(path);
                    foreach (var file in directoryInfo.GetFiles())
                    {
                        GutmanDeleteFiles(file.FullName);
                    }
                    foreach (var dir in directoryInfo.GetDirectories())
                    {
                        GutmanDeleteFiles(dir.FullName);
                    }
                    System.IO.Directory.Delete(path, true);
                }
            }
            catch (IOException ex) when ((ex.HResult & 0x0000FFFF) == 32) // ERROR_SHARING_VIOLATION
            {
                MessageBox.Show("El archivo está siendo utilizado por otro proceso: " + path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DataObliterate
{
    public class Elements
    {

        public List<string> GetFilesAndFolders(List<string> elements)
        {
            List<string> foundElements = new List<string>();

            foreach (var element in elements)
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
                Console.WriteLine($"Access denied to {directoryPath}: {ex.Message}");
            }

            return contents;
        }

        public int CountFilesAndFolders(List<string> elements)
        {
            int totalCount = 0;

            foreach (var element in elements)
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
                Console.WriteLine($"Access denied to {directoryPath}: {ex.Message}");
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
            catch (Exception ex)
            {
                Console.WriteLine($"Elevation request failed: {ex.Message}");
                return false;
            }
        }

    }
}

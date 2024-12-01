using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    // It's a file, add it to the list
                    foundElements.Add(element);
                }
                else if (Directory.Exists(element))
                {
                    // It's a directory, add all its contents recursively
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
                // Add files in the current directory
                contents.AddRange(Directory.GetFiles(directoryPath));

                // Get subdirectories and add their contents recursively
                var subdirectories = Directory.GetDirectories(directoryPath);
                foreach (var subdirectory in subdirectories)
                {
                    contents.Add(subdirectory); // Add the subdirectory itself
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
                    // It's a file, count it
                    totalCount++;
                }
                else if (Directory.Exists(element))
                {
                    // It's a directory, count all its contents recursively
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
                // Count files in the current directory
                count += Directory.GetFiles(directoryPath).Length;

                // Get subdirectories and count their contents recursively
                var subdirectories = Directory.GetDirectories(directoryPath);
                foreach (var subdirectory in subdirectories)
                {
                    count++; // Count the subdirectory itself
                    count += CountDirectoryContents(subdirectory);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied to {directoryPath}: {ex.Message}");
            }

            return count;
        }

    }
}

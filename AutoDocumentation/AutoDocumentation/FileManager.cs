using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutoDocumentation
{
    class FileManager
    {
        #region Constructors

        // Default Constructor
        public FileManager() { }
        public FileManager(string fileName, string directory)
        {
            FileName = fileName;
            Directory = directory;
            FilePath = Directory + "\\" + FileName + ".csv";
        }

        #endregion

        #region Properties

        public string FileName { get; set; }
        public string Directory { get; set; }
        public string FilePath { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Writes output strings to file.
        /// </summary>
        /// <param name="outputStrings">The output strings to write.</param>
        internal void WriteToFile(List<string> outputStrings)
        {
            // Create file if it doesn't already exist.
            using (var writer = new StreamWriter(FilePath, true))
            {
                // Append each item in outputStrings list to file.
                foreach (var item in outputStrings)
                {
                    writer.WriteLine(item);
                    writer.Flush();
                }

            }
        }

        /// <summary>
        /// Checks if file already exists, delete if so.
        /// </summary>
        /// <param name="filePath">The pato to the file.</param>
        internal void DeleteFileIfExists(string filePath)
        {
            if (File.Exists(filePath))
            { File.Delete(filePath); }
        }

        #endregion
    }
}

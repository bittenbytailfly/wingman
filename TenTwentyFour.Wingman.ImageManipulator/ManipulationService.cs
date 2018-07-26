using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenTwentyFour.Wingman.ImageManipulator.Manipulations;
using TenTwentyFour.Wingman.ImageManipulator.Models;

namespace TenTwentyFour.Wingman.ImageManipulator
{
    public class ManipulationService
    {
        public string SourceDirectory { get; set; }
        private string DerivedDirectory { get; set; }

        public ManipulationService(string sourceDirectory, string derivedDirectory)
        {
            this.SourceDirectory = sourceDirectory;
            this.DerivedDirectory = derivedDirectory;
        }

        public ImageDetail DeriveManipulatedImage(string relativePath, string originalExtension, Manipulation imageManipulation)
        {
            var derivedFileName = imageManipulation.GetDerivedFileName(relativePath);

            if (originalExtension != null)
            {
                relativePath = ReplaceExtension(relativePath, originalExtension);
            }

            var mimeType = GetMimeType(Path.GetExtension(relativePath));
            var originPath = Path.Combine(this.SourceDirectory, relativePath);
            if (System.IO.File.Exists(originPath))
            {
                var newFileDirectory = Path.Combine(this.DerivedDirectory, relativePath);
                var newFilePath = Path.Combine(newFileDirectory, derivedFileName);

                var derivedFolderCreated = new DirectoryInfo(newFileDirectory).CreationTime;
                var originalFileModified = new FileInfo(originPath).LastWriteTime;

                if (Directory.Exists(newFileDirectory) && (originalFileModified > derivedFolderCreated))
                {
                    Directory.Delete(newFileDirectory, true);
                }

                if (File.Exists(newFilePath))
                {
                    return new ImageDetail(newFilePath, mimeType);
                }

                if (!Directory.Exists(newFileDirectory))
                {
                    Directory.CreateDirectory(newFileDirectory);
                }

                imageManipulation.Manipulate(originPath, newFilePath);

                return new ImageDetail(newFilePath, mimeType);
            }

            throw new FileNotFoundException();
        }

        #region Helper Methods

        private string ReplaceExtension(string path, string originalExtension)
        {
            return Path.Combine(Path.GetDirectoryName(path), $"{Path.GetFileNameWithoutExtension(path)}.{originalExtension}");
        }

        public string GetMimeType(string extension)
        {
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".webp":
                    return "image/webp";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}

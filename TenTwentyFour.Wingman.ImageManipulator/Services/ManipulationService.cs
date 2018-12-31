using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using TenTwentyFour.Wingman.ImageManipulator.Manipulations;
using TenTwentyFour.Wingman.ImageManipulator.Models;

namespace TenTwentyFour.Wingman.ImageManipulator.Services
{
    public class ManipulationService
    {
        public string SourceDirectory { get; set; }
        private string DerivedDirectory { get; set; }
        public Cache Cache { get; set; }

        public ManipulationService(string sourceDirectory, string derivedDirectory, Cache httpCache)
        {
            this.SourceDirectory = sourceDirectory;
            this.DerivedDirectory = derivedDirectory;
            this.Cache = httpCache;
        }

        public ImageDetail DeriveManipulatedImage(string relativePath, string originalExtension, Manipulation imageManipulation)
        {
            var derivedFileName = imageManipulation.GetDerivedFileName(relativePath);
            var newFileDirectory = Path.Combine(this.DerivedDirectory, relativePath);
            var newFilePath = Path.Combine(newFileDirectory, derivedFileName);

            var cachedFile = GetCachedImageDetail(newFilePath);
            if (cachedFile != null)
            {
                return cachedFile;
            }

            var mimeType = GetMimeType(Path.GetExtension(relativePath));

            if (originalExtension != null)
            {
                relativePath = ReplaceExtension(relativePath, originalExtension);
            }
            
            var originPath = Path.Combine(this.SourceDirectory, relativePath);
            if (System.IO.File.Exists(originPath))
            {
                var derivedFolderCreated = new DirectoryInfo(newFileDirectory).CreationTime;
                var originalFileModified = new FileInfo(originPath).LastWriteTime;

                if (Directory.Exists(newFileDirectory) && (originalFileModified > derivedFolderCreated))
                {
                    Directory.Delete(newFileDirectory, true);
                }

                if (File.Exists(newFilePath))
                {
                    return CacheAndReturnImageDetail(newFilePath, mimeType);
                }

                if (!Directory.Exists(newFileDirectory))
                {
                    Directory.CreateDirectory(newFileDirectory);
                }

                imageManipulation.Manipulate(originPath, newFilePath);

                return CacheAndReturnImageDetail(newFilePath, mimeType);
            }

            throw new FileNotFoundException();
        }

        #region Caching

        public ImageDetail GetCachedImageDetail(string filePath)
        {
            if (Cache[filePath] == null)
            {
                return null;
            }

            return new ImageDetail(filePath, Cache[filePath].ToString());
        }

        public ImageDetail CacheAndReturnImageDetail(string filePath, string mimeType)
        {
            var dependency = new CacheDependency(filePath);
            Cache.Insert(filePath, mimeType, dependency);

            return new ImageDetail(filePath, mimeType);
        }

        #endregion

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
                    return "application/octet-stream";
            }
        }

        #endregion
    }
}

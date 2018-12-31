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

        public async Task<ImageDetail> DeriveManipulatedImage(string relativePath, string originalExtension, Manipulation imageManipulation)
        {
            var cachedFile = GetCachedImageDetail(relativePath, imageManipulation);
            if (cachedFile != null)
            {
                return cachedFile;
            }

            var derivedFileName = imageManipulation.GetDerivedFileName(relativePath);
            var newFileDirectory = Path.Combine(this.DerivedDirectory, relativePath);
            var newFilePath = Path.Combine(newFileDirectory, derivedFileName);

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
                    return CacheAndReturnImageDetail(relativePath, newFilePath, mimeType, imageManipulation);
                }

                if (!Directory.Exists(newFileDirectory))
                {
                    Directory.CreateDirectory(newFileDirectory);
                }

                imageManipulation.Manipulate(originPath, newFilePath);

                return CacheAndReturnImageDetail(relativePath, newFilePath, mimeType, imageManipulation);
            }

            throw new FileNotFoundException();
        }

        #region Caching

        public ImageDetail GetCachedImageDetail(string relativePath, Manipulation manipulation = null)
        {
            var cacheKey = (manipulation?.CacheKey ?? "") + relativePath;
            var detail = Cache[cacheKey] as ImageDetail;

            if (Cache[cacheKey] == null)
            {
                return null;
            }

            return detail;
        }

        public ImageDetail CacheAndReturnImageDetail(string relativePath, string filePath, string mimeType, Manipulation manipulation = null)
        {
            var cacheKey = (manipulation?.CacheKey ?? "") + relativePath;
            var imageDetail = new ImageDetail(filePath, mimeType);

            var dependency = new CacheDependency(filePath);
            Cache.Insert(cacheKey, imageDetail, dependency);

            return imageDetail;
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

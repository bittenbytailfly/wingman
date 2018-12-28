using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenTwentyFour.Wingman.ImageManipulator.Manipulations
{
    public abstract class Manipulation
    {
        public int Quality { get; set; }
        
        public abstract void Manipulate(string sourceFilePath, string destinationFilePath);

        /// <summary>
        /// Recommended to use Manipulation name + properties (e.g. "square_200")
        /// </summary>
        protected abstract string ManipulatedFileNameWithoutExtension { get; }

        public Manipulation(int quality)
        {
            this.Quality = quality;
        }

        protected void ResizeImage(string sourceFilePath, string destinationFilePath, Size size, ResizeMode mode)
        {
            byte[] photoBytes = File.ReadAllBytes(sourceFilePath);
            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                ResizeImage(inStream, destinationFilePath, size, mode);
            }
        }

        public string GetDerivedFileName(string filePath)
        {
            var fileExtension = Path.GetExtension(filePath);
            return $"{this.ManipulatedFileNameWithoutExtension}_q{this.Quality}{fileExtension}";
        }

        #region Helper Methods

        private void ResizeImage(Stream sourceFileStream, string destinationFilePath, Size size, ResizeMode mode)
        {
            ISupportedImageFormat format = GetImageFormat(destinationFilePath, this.Quality);
            using (MemoryStream outStream = new MemoryStream())
            {
                // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
                {
                    // Load, resize, set the format and quality and save an image.
                    var image = imageFactory
                        .AutoRotate()
                        .Load(sourceFileStream);

                    image.Resize(new ResizeLayer(size, resizeMode: mode))
                                .Format(format)
                                .Save(outStream);
                }
                // Do something with the stream.
                var path = Path.GetDirectoryName(destinationFilePath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                FileStream file = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write);
                outStream.WriteTo(file);
                file.Close();
            }
        }

        public Size GetDimensions(string sourceFilePath)
        {
            using (ImageFactory imageFactory = new ImageFactory())
            {
                // Load, resize, set the format and quality and save an image.
                var image = imageFactory.Load(sourceFilePath);
                return image.Image.Size;
            }
        }

        public ISupportedImageFormat GetImageFormat(string filename, int quality)
        {
            switch (Path.GetExtension(filename))
            {
                case ".jpg":
                case ".jpeg":
                    return new JpegFormat { Quality = quality };
                case ".png":
                    return new PngFormat { Quality = quality };
                case ".webp":
                    return new WebPFormat { Quality = quality };
                case ".gif":
                    return new GifFormat { Quality = quality };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}

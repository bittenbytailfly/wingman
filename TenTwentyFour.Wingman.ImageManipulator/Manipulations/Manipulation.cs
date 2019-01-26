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
    public class Manipulation
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Color BackgroundColor { get; set; }
        public ResizeMode ResizeMode { get; set; }
        public int Quality { get; set; }

        public string Key
        {
            get { return $"q{this.Quality}_w{this.Width}_h{this.Height}_bgc{this.BackgroundColor.ToString().ToLower()}_rm{this.ResizeMode.ToString().ToLower()}"; }
        }

        public Manipulation()
        {
            this.Width = 0;
            this.Height = 0;
            this.BackgroundColor = Color.Empty;
            this.ResizeMode = ResizeMode.Max;
            this.Quality = 80;
        }
        
        public void Manipulate(string sourceFilePath, string destinationFilePath)
        {
            this.ResizeImage(sourceFilePath, destinationFilePath, new Size(this.Width, this.Height), this.ResizeMode, this.BackgroundColor);
        }

        protected void ResizeImage(string sourceFilePath, string destinationFilePath, Size size, ResizeMode mode, Color? backgroundColor = null)
        {
            byte[] photoBytes = File.ReadAllBytes(sourceFilePath);
            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                ResizeImage(inStream, destinationFilePath, size, mode, backgroundColor);
            }
        }

        public string GetDerivedFileName(string filePath)
        {
            var fileExtension = Path.GetExtension(filePath);
            return $"{this.Key}{fileExtension}";
        }

        #region Helper Methods

        private void ResizeImage(Stream sourceFileStream, string destinationFilePath, Size size, ResizeMode mode,Color? backgroundColor = null)
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
                                .Format(format);

                    //Any padded areas in the output for image formats that do not contain an alpha channel will display as black (the default encoder output). To change this color to another use this option
                    if (backgroundColor != null)
                    {
                        image.BackgroundColor(backgroundColor.Value);
                    }

                    image.Save(outStream);
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

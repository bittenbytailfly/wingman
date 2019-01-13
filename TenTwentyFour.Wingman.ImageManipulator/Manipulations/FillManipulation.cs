using ImageProcessor.Imaging;
using System.Drawing;
using System.Text;

namespace TenTwentyFour.Wingman.ImageManipulator.Manipulations
{
    public class FillManipulation : Manipulation
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string BackgroundColor { get; set; }

        protected override string ManipulatedFileNameWithoutExtension
        {
            get
            {
                var fileName = new StringBuilder("fill");
                if (Width != default(int))
                {
                    fileName.Append($"_w{Width}");
                }
                if (Height != default(int))
                {
                    fileName.Append($"_h{Height}");
                }
                if (!string.IsNullOrWhiteSpace(BackgroundColor))
                {
                    fileName.Append($"_cl{BackgroundColor}");
                }
                return fileName.ToString();
            }
        }

        public FillManipulation(int width, int height, int quality, string backgroundColor = null) : base(quality)
        {
            this.Width = width;
            this.Height = height;
            this.BackgroundColor = backgroundColor;
        }

        public override void Manipulate(string sourceFilePath, string destinationFilePath)
        {
            var backgroundColor = string.IsNullOrWhiteSpace(BackgroundColor) ? default(Color?) : ColorTranslator.FromHtml($"#{BackgroundColor}");
            base.ResizeImage(sourceFilePath, destinationFilePath, new Size(this.Width, this.Height), ResizeMode.Pad, backgroundColor);
        }
    }
}

using ImageProcessor.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenTwentyFour.Wingman.ImageManipulator.Manipulations
{
    public class SquareManipulation : Manipulation
    {
        public int Width { get; set; }

        protected override string ManipulatedFileNameWithoutExtension => "square" + this.Width;

        public SquareManipulation(int width, int quality) : base(quality)
        {
            this.Width = width;
        }

        public override void Manipulate(string sourceFilePath, string destinationFilePath)
        {
            base.ResizeImage(sourceFilePath, destinationFilePath, new Size(this.Width, this.Width), ResizeMode.Crop);
        }
    }
}

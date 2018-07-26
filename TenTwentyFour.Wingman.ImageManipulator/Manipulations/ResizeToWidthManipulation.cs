using ImageProcessor.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenTwentyFour.Wingman.ImageManipulator.Manipulations
{
    public class ResizeToWidthManipulation : Manipulation
    {
        public int Width { get; set; }

        public ResizeToWidthManipulation(int width, int quality) : base(quality)
        {
            this.Width = width;
        }

        protected override string ManipulatedFileNameWithoutExtension => "resizetowidth" + this.Width;

        public override void Manipulate(string sourceFilePath, string destinationFilePath)
        {
            base.ResizeImage(sourceFilePath, destinationFilePath, new Size(this.Width, 0), ResizeMode.Max);
        }
    }
}

using ImageProcessor.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenTwentyFour.Wingman.ImageManipulator.Manipulations
{
    public class ManipulationArguments
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

        public ManipulationArguments()
        {
            this.Width = 0;
            this.Height = 0;
            this.BackgroundColor = Color.Empty;
            this.ResizeMode = ResizeMode.Max;
            this.Quality = 80;
        }
    }
}

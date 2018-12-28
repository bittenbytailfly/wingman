using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenTwentyFour.Wingman.ImageManipulator.Models
{
    public class ImageDetail
    {
        public string FilePath { get; set; }
        public string MimeType { get; set; }

        public ImageDetail(string filePath, string mimeType)
        {
            FilePath = filePath;
            MimeType = mimeType;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TenTwentyFour.Wingman.ImageManipulator;
using TenTwentyFour.Wingman.ImageManipulator.Manipulations;

namespace TenTwentyFour.Wingman.UserInterface.Controllers
{
    public class ImageServeController : Controller
    {
        ManipulationService Service { get; set; }

        public ImageServeController(ManipulationService service)
        {
            this.Service = service;
        }

        public ImageServeController()
            : this(new ManipulationService(ConfigurationManager.AppSettings["SourceDirectory"], ConfigurationManager.AppSettings["DerivedDirectory"]))
            { }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            base.HttpContext.Response.AppendHeader("Cache-Control", "max-age=2592000");
        }

        public ActionResult Plain(string path)
        {
            var mimeType = this.Service.GetMimeType(Path.GetExtension(path));
            var originPath = Path.Combine(this.Service.SourceDirectory, path);

            if (System.IO.File.Exists(originPath))
            {
                base.HttpContext.Response.AppendHeader("Cache-Control", "max-age=2592000");
                return base.File(originPath, mimeType);
            }

            throw new HttpException(404, "File not found");
        }

        public ActionResult Square(int quality, int size, string path, string originalExtension = null)
        {
            var fileExtension = Path.GetExtension(path);
            var derivedFileName = "square_" + size + fileExtension;
            var manipulation = new SquareManipulation(size, quality);

            return this.ServeManipulatedImage(path, derivedFileName, originalExtension, manipulation);
        }

        public ActionResult ResizeToWidth(int quality, int width, string path, string originalExtension = null)
        {
            var fileExtension = Path.GetExtension(path);
            var derivedFileName = "resize_w_" + width + fileExtension;
            var manipulation = new ResizeToWidthManipulation(width, quality);

            return this.ServeManipulatedImage(path, derivedFileName, originalExtension, manipulation);
        }

        #region Helper Methods

        public ActionResult ServeManipulatedImage(string relativePath, string derivedFileName, string originalExtension, Manipulation imageManipulation)
        {
            try
            {
                var derivedImage = this.Service.DeriveManipulatedImage(relativePath, derivedFileName, originalExtension, imageManipulation);
                return base.File(derivedImage.FilePath, derivedImage.MimeType);
            }
            catch (FileNotFoundException ex)
            {
                throw new HttpException(404, "File not found");
            }
            catch (Exception ex)
            {
                throw new HttpException(500, ex.Message, ex);
            }
        }

        #endregion
    }
}
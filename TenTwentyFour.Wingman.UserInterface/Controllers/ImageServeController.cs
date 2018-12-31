using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using TenTwentyFour.Wingman.ImageManipulator;
using TenTwentyFour.Wingman.ImageManipulator.Manipulations;

namespace TenTwentyFour.Wingman.UserInterface.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
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
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new HttpException(404, "File not found");
            }

            var mimeType = this.Service.GetMimeType(Path.GetExtension(path));
            var originPath = Path.Combine(this.Service.SourceDirectory, path);

            if (System.IO.File.Exists(originPath))
            {
                base.HttpContext.Response.AppendHeader("Cache-Control", "max-age=2592000");
                return base.File(originPath, mimeType);
            }

            throw new HttpException(404, "File not found");
        }

        public ActionResult Square(int quality, int width, string path, string originalExtension = null)
        {
            var manipulation = new SquareManipulation(width, quality);
            return this.ServeManipulatedImage(path, originalExtension, manipulation);
        }

        public ActionResult ResizeToWidth(int quality, int width, string path, string originalExtension = null)
        {
            var manipulation = new ResizeToWidthManipulation(width, quality);
            return this.ServeManipulatedImage(path, originalExtension, manipulation);
        }

        #region Helper Methods

        public ActionResult ServeManipulatedImage(string relativePath, string originalExtension, Manipulation imageManipulation)
        {
            try
            {
                var derivedImage = this.Service.DeriveManipulatedImage(relativePath, originalExtension, imageManipulation);
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
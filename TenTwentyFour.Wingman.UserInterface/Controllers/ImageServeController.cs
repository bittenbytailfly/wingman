using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TenTwentyFour.Wingman.ImageManipulator;
using TenTwentyFour.Wingman.ImageManipulator.Manipulations;
using TenTwentyFour.Wingman.ImageManipulator.Services;

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
        {
            this.Service = new ManipulationService(ConfigurationManager.AppSettings["SourceDirectory"],
                ConfigurationManager.AppSettings["DerivedDirectory"],
                null);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            this.Service.Cache = base.HttpContext.ApplicationInstance.Context.Cache;
            base.HttpContext.Response.AppendHeader("Cache-Control", "max-age=2592000");
        }

        public ActionResult Plain(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new HttpException(404, "File not found");
            }

            var originPath = Path.Combine(this.Service.SourceDirectory, path);
            var cachedFile = this.Service.GetCachedImageDetail(originPath);
            if (cachedFile != null)
            {
                return base.File(cachedFile.FilePath, cachedFile.MimeType);
            }

            var mimeType = this.Service.GetMimeType(Path.GetExtension(path));

            if (System.IO.File.Exists(originPath))
            {
                base.HttpContext.Response.AppendHeader("Cache-Control", "max-age=2592000");
                var image = this.Service.CacheAndReturnImageDetail(originPath, mimeType);
                return base.File(image.FilePath, image.MimeType);
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
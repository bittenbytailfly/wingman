using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using TenTwentyFour.Wingman.ImageManipulator;
using TenTwentyFour.Wingman.ImageManipulator.Manipulations;
using TenTwentyFour.Wingman.ImageManipulator.Services;

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

        [OutputCache(Duration = 300, VaryByParam = "*")]
        public ActionResult Plain(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new HttpException(404, "File not found");
            }

            var cachedFile = this.Service.GetCachedImageDetail(path);
            if (cachedFile != null)
            {
                return base.File(cachedFile.FilePath, cachedFile.MimeType);
            }

            var originPath = Path.Combine(this.Service.SourceDirectory, path);
            var mimeType = this.Service.GetMimeType(Path.GetExtension(path));

            if (System.IO.File.Exists(originPath))
            {
                base.HttpContext.Response.AppendHeader("Cache-Control", "max-age=2592000");
                var image = this.Service.CacheAndReturnImageDetail(path, originPath, mimeType);
                return base.File(image.FilePath, image.MimeType);
            }

            throw new HttpException(404, "File not found");
        }

        [OutputCache(Duration = 300, VaryByParam = "*")]
        public async Task<ActionResult> Square(int quality, int width, string path, string originalExtension = null)
        {
            var manipulation = new SquareManipulation(width, quality);
            return await this.ServeManipulatedImage(path, originalExtension, manipulation);
        }

        [OutputCache(Duration = 300, VaryByParam = "*")]
        public async Task<ActionResult> ResizeToWidth(int quality, int width, string path, string originalExtension = null)
        {
            var manipulation = new ResizeToWidthManipulation(width, quality);
            return await this.ServeManipulatedImage(path, originalExtension, manipulation);
        }

        #region Helper Methods

        public async Task<ActionResult> ServeManipulatedImage(string relativePath, string originalExtension, Manipulation imageManipulation)
        {
            try
            {
                var derivedImage = await this.Service.DeriveManipulatedImage(relativePath, originalExtension, imageManipulation);
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
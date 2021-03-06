﻿using ImageProcessor.Imaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
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
                return base.File(originPath, mimeType);
            }

            throw new HttpException(404, "File not found");
        }

        public ActionResult Square(int quality, int rotationDegrees, int width, string pathPrefix, string path, string originalExtension = null)
        {
            var manipulation = new Manipulation
            {
                Quality = quality,
                Width = width,
                Height = width,
                ResizeMode = ResizeMode.Crop,
                RotationDegrees = rotationDegrees
            };
            var relativePath = String.IsNullOrWhiteSpace(pathPrefix)
                ? path
                : pathPrefix + "/" + path;
            return this.ServeManipulatedImage(relativePath, originalExtension, manipulation);
        }

        public ActionResult ResizeToWidth(int quality, int rotationDegrees, int width, string pathPrefix, string path, string originalExtension = null)
        {
            var manipulation = new Manipulation
            {
                Quality = quality,
                Width = width,
                ResizeMode = ResizeMode.Max,
                RotationDegrees = rotationDegrees
            };
            var relativePath = String.IsNullOrWhiteSpace(pathPrefix)
                ? path
                : pathPrefix + "/" + path;
            return this.ServeManipulatedImage(relativePath, originalExtension, manipulation);
        }

        public ActionResult Crop(int quality, int rotationDegrees, int width, int height, string pathPrefix, string path, string originalExtension = null)
        {
            var manipulation = new Manipulation
            {
                Quality = quality,
                Width = width,
                Height = height,
                ResizeMode = ResizeMode.Crop,
                RotationDegrees = rotationDegrees
            };
            var relativePath = String.IsNullOrWhiteSpace(pathPrefix)
                ? path
                : pathPrefix + "/" + path;
            return this.ServeManipulatedImage(relativePath, originalExtension, manipulation);
        }

        public ActionResult Pad(int quality, int rotationDegrees, int width, int height, string pathPrefix, string path, string originalExtension = null, string bgColor = null)
        {
            var manipulation = new Manipulation
            {
                Quality = quality,
                Width = width,
                Height = height,
                ResizeMode = ResizeMode.Pad,
                RotationDegrees = rotationDegrees
            };
            if (!String.IsNullOrWhiteSpace(bgColor))
            {
                manipulation.BackgroundColor = ColorTranslator.FromHtml($"#{bgColor}");
            }
            var relativePath = String.IsNullOrWhiteSpace(pathPrefix)
                 ? path
                 : pathPrefix + "/" + path;
            return this.ServeManipulatedImage(relativePath, originalExtension, manipulation);
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
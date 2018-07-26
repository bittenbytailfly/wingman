using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TenTwentyFour.Wingman.UserInterface.RouteConstraints;

namespace TenTwentyFour.Wingman.UserInterface
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var maxDimensionInt = 500;
            var maxDimension = ConfigurationManager.AppSettings["MaximumImageDimension"];
            if (maxDimension == null)
            {
                throw new ConfigurationErrorsException("You must specify a key for MaximumImageDimension");
            }

            if (!Int32.TryParse(maxDimension, out maxDimensionInt))
            {
                throw new ConfigurationErrorsException("You must specify a valid integer for MaximumImageDimension");
            }

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var maxImageSizeConstraint = new MaximumImageSizeConstraint(maxDimensionInt);

            routes.MapRoute(
                name: "Crop Square With Format Change",
                url: "derived/square/{quality}/{originalExtension}/{size}/{*path}",
                defaults: new { controller = "ImageServe", action = "Square" },
                constraints: new { size = maxImageSizeConstraint, path = "(.*).(jpg|png|gif|webp)", originalExtension = "jpg|png|gif|webp" }
            );

            routes.MapRoute(
               name: "Crop Square",
               url: "derived/square/{quality}/{size}/{*path}",
               defaults: new { controller = "ImageServe", action = "Square" },
               constraints: new { size = maxImageSizeConstraint, path = "(.*).(jpg|png|gif|webp)" }
           );

            routes.MapRoute(
                name: "Resize Width With Format Change",
                url: "derived/resize-w/{quality}/{originalExtension}/{size}/{*path}",
                defaults: new { controller = "ImageServe", action = "ResizeToWidth" },
                constraints: new { size = maxImageSizeConstraint, path = "(.*).(jpg|png|gif|webp)", originalExtension = "jpg|png|gif|webp" }
            );

            routes.MapRoute(
                name: "Resize Width",
                url: "derived/resize-w/{quality}/{size}/{*path}",
                defaults: new { controller = "ImageServe", action = "ResizeToWidth" },
                constraints: new { size = maxImageSizeConstraint, path = "(.*).(jpg|png|gif|webp)" }
            );

            routes.MapRoute(
               name: "Plain",
               url: "{*path}",
               defaults: new { controller = "ImageServe", action = "Plain" }
           );
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TenTwentyFour.Wingman.UserInterface.ApplicationSettings;
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
            var systemColorConstraint = new SystemColorConstraint();

            var customRouteSection = ConfigurationManager.GetSection("wingmanCustomRoutes") as WingmanRoutesConfigurationSection;
            foreach (WingmanCustomRouteElement route in customRouteSection.CustomRoutes)
            {
                routes.MapRoute(
                    name: route.Name,
                    url: $"{route.UriRoot}/{{*path}}",
                    defaults: new { controller = "ImageServe", action = route.Manipulation, quality = route.Quality, width = route.Width, originalExtension = route.OriginalExtension, bgColor = route.BackgroundColour, height = route.Height },
                    constraints: new { path = "(.*).(jpg|png|gif|webp)", originalExtension = "jpg|png|gif|webp" }
                );
            }

            if (!customRouteSection.DisableDefaultRouting)
            {
                routes.MapRoute(
                    name: "Crop Square With Format Change",
                    url: "derived/square/{quality}/{originalExtension}/{width}/{*path}",
                    defaults: new { controller = "ImageServe", action = "Square" },
                    constraints: new { width = maxImageSizeConstraint, path = "(.*).(jpg|png|gif|webp)", originalExtension = "jpg|png|gif|webp" }
                );

                routes.MapRoute(
                   name: "Crop Square",
                   url: "derived/square/{quality}/{width}/{*path}",
                   defaults: new { controller = "ImageServe", action = "Square" },
                   constraints: new { width = maxImageSizeConstraint, path = "(.*).(jpg|png|gif|webp)" }
               );

                routes.MapRoute(
                    name: "Resize Width With Format Change",
                    url: "derived/resize-w/{quality}/{originalExtension}/{width}/{*path}",
                    defaults: new { controller = "ImageServe", action = "ResizeToWidth" },
                    constraints: new { width = maxImageSizeConstraint, path = "(.*).(jpg|png|gif|webp)", originalExtension = "jpg|png|gif|webp" }
                );

                routes.MapRoute(
                    name: "Resize Width",
                    url: "derived/resize-w/{quality}/{width}/{*path}",
                    defaults: new { controller = "ImageServe", action = "ResizeToWidth" },
                    constraints: new { width = maxImageSizeConstraint, path = "(.*).(jpg|png|gif|webp)" }
                );

                routes.MapRoute(
                     name: "Centre Crop with no filling",
                     url: "derived/crop/{quality}/{originalExtension}/{width}/{height}/{*path}",
                     defaults: new { controller = "ImageServe", action = "Crop" },
                     constraints: new { width = maxImageSizeConstraint, height = maxImageSizeConstraint, path = "(.*).(jpg|png|gif|webp)" }
                );

                //Note: bg color should be a hex value eg: FF2D00
                routes.MapRoute(
                     name: "Fill Width, Quality, Height and BgColor",
                     url: "derived/fill/{quality}/{originalExtension}/{width}/{height}/{bgColor}/{*path}",
                     defaults: new { controller = "ImageServe", action = "Fill" },
                     constraints: new { width = maxImageSizeConstraint, height = maxImageSizeConstraint, bgColor = systemColorConstraint, path = "(.*).(jpg|png|gif|webp)" }
                );
            }

            routes.MapRoute(
               name: "Plain",
               url: "{*path}",
               defaults: new { controller = "ImageServe", action = "Plain" }
           );
        }
    }
}

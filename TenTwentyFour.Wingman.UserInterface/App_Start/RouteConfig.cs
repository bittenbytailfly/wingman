using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing.Constraints;
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

            var customRouteSection = ConfigurationManager.GetSection("wingmanCustomRoutes") as WingmanRoutesConfigurationSection;
            var maxImageSizeConstraint = new MaximumImageSizeConstraint(customRouteSection.MaxImageDimension);
            var systemColorConstraint = new SystemColorConstraint();

            foreach (WingmanCustomRouteElement route in customRouteSection.CustomRoutes)
            {
                var bgConstraint = route.UriRoot.Contains("{bgColor}") || !String.IsNullOrWhiteSpace(route.BackgroundColour)
                    ? (IRouteConstraint)new SystemColorConstraint()
                    : new RegexRouteConstraint("(.*)?");

                var originalExtensionConstraint = route.UriRoot.Contains("{originalExtension}") || !String.IsNullOrWhiteSpace(route.OriginalExtension)
                    ? (IRouteConstraint)new RegexRouteConstraint("jpg|png|gif|webp")
                    : new RegexRouteConstraint("(.*)?");

                var widthConstraint = !String.IsNullOrWhiteSpace(route.AllowedWidths)
                    ? (IRouteConstraint)new RegexRouteConstraint(route.AllowedWidths.Replace(",", "|"))
                    : maxImageSizeConstraint;

                var heightConstraint = !String.IsNullOrWhiteSpace(route.AllowedHeights)
                    ? (IRouteConstraint)new RegexRouteConstraint(route.AllowedHeights.Replace(",", "|"))
                    : maxImageSizeConstraint;

                routes.MapRoute(
                    name: route.Name,
                    url: $"{route.UriRoot}/{{*path}}",
                    defaults: new { controller = "ImageServe", action = route.Manipulation, quality = route.Quality, width = route.Width, originalExtension = route.OriginalExtension, bgColor = route.BackgroundColour, height = route.Height, rotationDegrees = route.RotationDegrees, pathPrefix = route.PathPrefix },
                    constraints: new { path = "(.*).(jpg|png|gif|webp)", originalExtension = originalExtensionConstraint, rotationDegrees = "0|90|180|270", width = widthConstraint, height = heightConstraint }
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

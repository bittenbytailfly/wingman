using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TenTwentyFour.Wingman.UserInterface.RouteConstraints;

namespace TenTwentyFour.Wingman.UserInterface.App_Start
{
    public class RegisterFillManipulationRoutes
    {
        public static void RegisterRoutes(int maxDimensionInt, RouteCollection routes)
        {

            var maxImageSizeConstraint = new MaximumImageSizeConstraint(maxDimensionInt);
            var systemColorConstraint = new SystemColorConstraint();


            //Note: bg color should be a hexa value eg: FF2D00
            routes.MapRoute(
                 name: "Fill Width, Quality, Height and BgColor",
                 url: "derived/fill_h/{quality}/{width}/{height}/{bgColor}/{*path}",
                 defaults: new { controller = "ImageServe", action = "Fill" },
                 constraints: new { width = maxImageSizeConstraint, height = @"\d+", bgColor = systemColorConstraint, path = "(.*).(jpg|png|gif|webp)" }
                );

            routes.MapRoute(
                  name: "Fill Width, Quality and Height",
                  url: "derived/fill_h/{quality}/{width}/{height}/{*path}",
                  defaults: new { controller = "ImageServe", action = "Fill" },
                  constraints: new { width = maxImageSizeConstraint, height= @"\d+", path = "(.*).(jpg|png|gif|webp)" }
                );

            //Note: bg color should be a hexa value eg: FF2D00
            routes.MapRoute(
                name: "Fill Width, Quality and BgColor",
                url: "derived/fill/{quality}/{width}/{bgColor}/{*path}",
                defaults: new { controller = "ImageServe", action = "Fill" },
                constraints: new { width = maxImageSizeConstraint, bgColor = systemColorConstraint, path = "(.*).(jpg|png|gif|webp)" }
              );

            routes.MapRoute(
                 name: "Fill Width, Quality",
                 url: "derived/fill/{quality}/{width}/{*path}",
                 defaults: new { controller = "ImageServe", action = "Fill" },
                 constraints: new { width = maxImageSizeConstraint, path = "(.*).(jpg|png|gif|webp)" }
               );
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace TenTwentyFour.Wingman.UserInterface.RouteConstraints
{
    public class MaximumImageSizeConstraint : IRouteConstraint
    {
        public int MaximumImageSize { get; set; }

        public MaximumImageSizeConstraint(int maximumImageSize)
        {
            MaximumImageSize = maximumImageSize;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values[parameterName] == null)
            {
                return false;
            }

            int requestedImageSize;
            if (!Int32.TryParse(values[parameterName].ToString(), out requestedImageSize))
            {
                return false;
            }

            return requestedImageSize > 0 && requestedImageSize <= this.MaximumImageSize;
        }
    }
}
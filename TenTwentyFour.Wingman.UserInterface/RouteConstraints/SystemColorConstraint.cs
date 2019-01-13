using System;
using System.Drawing;
using System.Web;
using System.Web.Routing;

namespace TenTwentyFour.Wingman.UserInterface.RouteConstraints
{
    public class SystemColorConstraint : IRouteConstraint
    {
        public SystemColorConstraint() {}

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values[parameterName] == null)
            {
                return false;
            }
            try
            {
                var systemColor = ColorTranslator.FromHtml($"#{values[parameterName]}");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
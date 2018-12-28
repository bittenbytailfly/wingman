using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TenTwentyFour.Wingman.UserInterface.ApplicationSettings
{
    public class WingmanCustomRouteElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new WingmanCustomRouteElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WingmanCustomRouteElement)element).Name;
        }
    }
}
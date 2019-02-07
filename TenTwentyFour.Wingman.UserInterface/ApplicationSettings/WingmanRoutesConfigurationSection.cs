using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TenTwentyFour.Wingman.UserInterface.ApplicationSettings
{
    public class WingmanRoutesConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("maxImageDimension", IsRequired = true)]
        public int MaxImageDimension
        {
            get { return (int)base["maxImageDimension"]; }
            set { base["maxImageDimension"] = value; }
        }

        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public WingmanCustomRouteElementCollection CustomRoutes
        {
            get { return ((WingmanCustomRouteElementCollection)(this[""])); }
            set { this[""] = value; }
        }
    }
}
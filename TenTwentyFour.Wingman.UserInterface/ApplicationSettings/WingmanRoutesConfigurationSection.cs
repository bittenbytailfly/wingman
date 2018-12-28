using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TenTwentyFour.Wingman.UserInterface.ApplicationSettings
{
    public class WingmanRoutesConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("disableDefaultRouting", IsRequired = true)]
        public bool DisableDefaultRouting
        {
            get { return (bool)base["disableDefaultRouting"]; }
            set { base["disableDefaultRouting"] = value; }
        }

        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public WingmanCustomRouteElementCollection CustomRoutes
        {
            get { return ((WingmanCustomRouteElementCollection)(this[""])); }
            set { this[""] = value; }
        }
    }
}
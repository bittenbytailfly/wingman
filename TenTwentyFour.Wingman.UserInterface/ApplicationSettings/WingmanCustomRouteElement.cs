using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TenTwentyFour.Wingman.UserInterface.ApplicationSettings
{
    public class WingmanCustomRouteElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return base["name"] as string; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("uriRoot", IsRequired = true)]
        public string UriRoot
        {
            get { return base["uriRoot"] as string; }
            set { base["uriRoot"] = value; }
        }

        [ConfigurationProperty("manipulation", IsRequired = true)]
        public string Manipulation
        {
            get { return base["manipulation"] as string; }
            set { base["manipulation"] = value; }
        }

        [ConfigurationProperty("quality", IsRequired = true)]
        public int Quality
        {
            get { return Convert.ToInt32(base["quality"]); }
            set { base["quality"] = value; }
        }

        [ConfigurationProperty("width", IsRequired = true)]
        public int Width
        {
            get { return Convert.ToInt32(base["width"]); }
            set { base["width"] = value; }
        }

        [ConfigurationProperty("originalExtension", IsRequired = false)]
        public string OriginalExtension
        {
            get { return base["originalExtension"] as string; }
            set { base["originalExtension"] = value; }
        }
    }
}
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

        [ConfigurationProperty("quality", IsRequired = false)]
        public int Quality
        {
            get { return Convert.ToInt32(base["quality"]); }
            set { base["quality"] = value; }
        }

        [ConfigurationProperty("width", IsRequired = false)]
        public int Width
        {
            get { return Convert.ToInt32(base["width"]); }
            set { base["width"] = value; }
        }

        [ConfigurationProperty("height", IsRequired = false)]
        public int Height
        {
            get { return base["height"] != null ? Convert.ToInt32(base["height"]) : 0; }
            set { base["height"] = value; }
        }

        [ConfigurationProperty("originalExtension", IsRequired = false)]
        public string OriginalExtension
        {
            get { return base["originalExtension"] as string; }
            set { base["originalExtension"] = value; }
        }

        [ConfigurationProperty("bgColor", IsRequired = false)]
        public string BackgroundColour
        {
            get { return base["bgColor"] as string; }
            set { base["bgColor"] = value; }
        }

        [ConfigurationProperty("rotationDegrees", IsRequired = false)]
        public int RotationDegrees
        {
            get { return base["rotationDegrees"] != null ? Convert.ToInt32(base["rotationDegrees"]) : 0; }
            set { base["rotationDegrees"] = value; }
        }

        [ConfigurationProperty("allowedWidths", IsRequired = false)]
        public string AllowedWidths
        {
            get { return base["allowedWidths"] as string; }
            set { base["allowedWidths"] = value; }
        }

        [ConfigurationProperty("allowedHeights", IsRequired = false)]
        public string AllowedHeights
        {
            get { return base["allowedHeights"] as string; }
            set { base["allowedHeights"] = value; }
        }

        [ConfigurationProperty("pathPrefix", IsRequired = false)]
        public string PathPrefix
        {
            get { return base["pathPrefix"] as string; }
            set { base["pathPrefix"] = value; }
        }
    }
}
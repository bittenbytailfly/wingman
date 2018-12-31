using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using TenTwentyFour.Wingman.ImageManipulator.Models;

namespace TenTwentyFour.Wingman.ImageManipulator.Services
{
    public class HttpCacheService
    {
        private Cache Cache { get; set; }

        public HttpCacheService(HttpContextBase httpContext)
        {
            this.Cache = httpContext.ApplicationInstance.Context.Cache;
        }

        public ImageDetail GetCachedImageDetail(string filePath)
        {
            if (Cache[filePath] == null)
            {
                return null;
            }

            return new ImageDetail(filePath, Cache[filePath].ToString());
        }

        public ImageDetail CacheAndReturnImageDetail(string filePath, string mimeType)
        {
            var dependency = new CacheDependency(filePath);
            Cache.Insert(filePath, mimeType, dependency);

            return new ImageDetail(filePath, mimeType);
        }
    }
}

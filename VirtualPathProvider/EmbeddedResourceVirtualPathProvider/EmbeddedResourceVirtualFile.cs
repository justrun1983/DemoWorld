using System.IO;
using System.Web;
using System.Web.Hosting;

namespace EmbeddedResourceVirtualPathProvider
{
    class EmbeddedResourceVirtualFile : VirtualFile
    {
        readonly EmbeddedResource _embedded;
        readonly EmbeddedResourceCacheControl _cacheControl;

        public EmbeddedResourceVirtualFile(string virtualPath, EmbeddedResource embedded, EmbeddedResourceCacheControl cacheControl)
            : base(virtualPath)
        {
            this._embedded = embedded;
            this._cacheControl = cacheControl;
        }

        public override Stream Open()
        {
            if (_cacheControl != null)
            {
                HttpContext.Current.Response.Cache.SetCacheability(_cacheControl.Cacheability);
                HttpContext.Current.Response.Cache.AppendCacheExtension("max-age=" + _cacheControl.MaxAge);
            }
            return _embedded.GetStream();
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace SimpleVirtualPathProvider
{
    public class SimpleVirtualPathProvider : VirtualPathProvider
    {
        private readonly Assembly _assembly;
        private readonly IDictionary<string, string> _resources = new Dictionary<string, string>();

        public SimpleVirtualPathProvider(Assembly assembly)
        {
            _assembly = assembly;
            var assemblyName = assembly.GetName().Name;
            foreach (var resourcePath in assembly.GetManifestResourceNames().Where(r => r.StartsWith(assemblyName)))
            {
                var key = resourcePath.ToUpperInvariant().Substring(assemblyName.Length).TrimStart('.');
                _resources[key] = resourcePath;
            }
        }

        private string GetResourceFromVirtualPath(string virtualPath)
        {
            var path = VirtualPathUtility.ToAppRelative(virtualPath).TrimStart('~', '/');
            var index = path.LastIndexOf("/");
            if (index != -1)
            {
                var folder = path.Substring(0, index).Replace("-", "_"); //embedded resources with "-"in their folder names are stored as "_".
                path = folder + path.Substring(index);
            }
            var cleanedPath = path.Replace('/', '.');
            var key = (cleanedPath).ToUpperInvariant();
            if (_resources.ContainsKey(key))
            {
                var resource = _resources[key];
                if (resource != null)
                {
                    return resource;
                }
            }
            return null;
        }

        public override bool FileExists(string virtualPath)
        {
            return (base.FileExists(virtualPath) || GetResourceFromVirtualPath(virtualPath) != null);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            var resourceName = GetResourceFromVirtualPath(virtualPath);
            if (resourceName != null)
            {
                return new SimpleVirtualFile(_assembly, GetResourceFromVirtualPath(virtualPath), virtualPath);
            }
            return base.GetFile(virtualPath);
        }

        public override string GetCacheKey(string virtualPath)
        {
            var resource = GetResourceFromVirtualPath(virtualPath);
            if (resource != null)
            {
                var fileInfo = new FileInfo(_assembly.Location);
                return (virtualPath + fileInfo.LastWriteTime.Ticks).GetHashCode().ToString();
            }
            return base.GetCacheKey(virtualPath);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            var resource = GetResourceFromVirtualPath(virtualPath);
            if (resource != null)
            {
                return new CacheDependency(_assembly.Location);
            }

            if (DirectoryExists(virtualPath) || FileExists(virtualPath))
            {
                return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
            }

            return null;
        }

    }

    public class SimpleVirtualFile : VirtualFile
    {
        private readonly Assembly _assembly;
        private string _resourcePath;
        public SimpleVirtualFile(Assembly assembly, string resourcePath, string virtualPath) : base(virtualPath)
        {
            _assembly = assembly;
            _resourcePath = resourcePath;
        }

        public override Stream Open()
        {
            return _assembly.GetManifestResourceStream(_resourcePath);
        }
    }
}

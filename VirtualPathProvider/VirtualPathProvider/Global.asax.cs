using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using EmbeddedResourceVirtualPathProvider;

namespace VirtualPathProvider
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //get all the reference assembly that contain the .cshtml resources
            var assemblies = System.Web.Compilation.BuildManager.GetReferencedAssemblies()
             .Cast<Assembly>()
             .Where(a => !a.GetName().Name.StartsWith("System"))
             .Where(a => a.GetManifestResourceNames().Any(n => n.EndsWith(".cshtml")))
             .ToList();


            /********************************* Standard demo ********************************/
            var virtualPathProvider = new Vpp();

            foreach (var assembly in assemblies)
                virtualPathProvider.Add(assembly, assembly == Assembly.GetExecutingAssembly() ? "" : $"..\\{assembly.GetName().Name}");

            System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(virtualPathProvider);
            /********************************* Simple demo *********************************/
            //var assembly = Assembly.Load("WebPart");
            //var provider = new SimpleVirtualPathProvider.SimpleVirtualPathProvider(assembly);
            //System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(provider);
        }
    }
}

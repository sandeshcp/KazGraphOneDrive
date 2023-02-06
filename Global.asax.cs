using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
//using Microsoft.AspNet.WebFormsDependencyInjection.Unity;
//using Unity;

namespace KazGraph
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            //var container = this.AddUnity();
            //container.RegisterType<IAuthorRepository, AuthorRepository>();
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
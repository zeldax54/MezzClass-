using System;
using System.Web;
using System.Web.Security;
using Microsoft.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Owin;

[assembly: OwinStartupAttribute(typeof(MezzClass0._1.Startup))]
namespace MezzClass0._1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
         
            ConfigureAuth(app);
        }
    }
}

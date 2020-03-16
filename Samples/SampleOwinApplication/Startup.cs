using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SampleOwinApplication.Startup))]
namespace SampleOwinApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/foo", a =>
            {
                a.Use((ctx, next) =>
                {
                    return next.Invoke();
                });
            });

            ConfigureAuth(app);
        }
    }
}

using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(EldredBrown.ProFootball.AspNetCore.MvcWebApp.Areas.Identity.IdentityHostingStartup))]
namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}
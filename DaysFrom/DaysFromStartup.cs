using DaysFrom.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Shiny;
using Xamarin.Forms.Internals;

namespace DaysFrom
{
    public class DaysFromStartup : ShinyStartup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.UseNotifications();

        }
    }
}

using IceCreams.Ratings.Managers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(IceCreams.Ratings.Startup))]
namespace IceCreams.Ratings
{
    public class Startup : FunctionsStartup
    {

        public Startup()
        {

        }
        public override void Configure(IFunctionsHostBuilder builder)
        {

            builder.Services.AddScoped<IRatingManager, RatingManager>();
        }
    }
}

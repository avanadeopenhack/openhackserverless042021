using IceCreams.Ratings.Managers;
using IceCreams.Ratings.Options;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

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

            //builder.Services.Configure<ExternalApiOptions>(Configuration);
            builder.Services.AddScoped<IRatingManager, RatingManager>();
        }
    }
}

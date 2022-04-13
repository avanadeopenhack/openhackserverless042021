using IceCreams.Ratings.Managers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(IceCreams.Ratings.Startup))]
namespace IceCreams.Ratings
{
    public class Startup : FunctionsStartup
    {

        public Startup()
        {

        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            base.ConfigureAppConfiguration(builder);

            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .SetBasePath(context.ApplicationRootPath)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.ConfigurationBuilder.Build();
            if (string.IsNullOrEmpty(config["cosmosDBAccount"]))
            {
                builder.ConfigurationBuilder
                    .AddInMemoryCollection(GetCosmosDb(builder.ConfigurationBuilder.Build()).Result)
                    .Build();
            }
            else
            {
                builder.ConfigurationBuilder
                    .Build();

            }


        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IRatingManager, RatingManager>();
        }

        private async Task<IEnumerable<KeyValuePair<string, string>>> GetCosmosDb(IConfigurationRoot config)
        {

            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            // Authenticate to the Azure Resource Manager to get the Service Managed token.
            string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://management.azure.com/");


            var cosmosDbAccount = config["cosmosDBAccount"];
            // Setup the List Keys API to get the Azure Cosmos DB keys.
            string endpoint = $"https://management.azure.com/subscriptions/{config["subscriptionId"]}/resourceGroups/{config["resourceGroupName"]}/providers/Microsoft.DocumentDB/databaseAccounts/{cosmosDbAccount}/listKeys?api-version=2019-12-12";

            var httpClient = new HttpClient();
            // Add the access token to request headers.
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Post to the endpoint to get the keys result.
            var result = await httpClient.PostAsync(endpoint, new StringContent(""));

            // Get the result back as a DatabaseAccountListKeysResult.
            var keys = await result.Content.ReadFromJsonAsync<DatabaseAccountListKeysResult>();
            

            var res = new List<KeyValuePair<string, string>>();
            res.Add(new KeyValuePair<string, string>("CosmosDBConnection", $"AccountEndpoint=https://{cosmosDbAccount}.documents.azure.com:443/;AccountKey={keys.primaryMasterKey};"));
            return res;
        }

        public class DatabaseAccountListKeysResult
        {
            public string primaryMasterKey { get; set; }
            public string primaryReadonlyMasterKey { get; set; }
            public string secondaryMasterKey { get; set; }
            public string secondaryReadonlyMasterKey { get; set; }
        }
    }
}

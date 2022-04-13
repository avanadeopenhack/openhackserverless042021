using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace IceCreams.Ratings
{
    public class GetConfig
    {
        private readonly IConfiguration configuration;
        public GetConfig(IConfiguration config)
        {
            configuration = config;
        }

        [FunctionName("GetConfig")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";

            LogConfig(configuration.GetChildren(), log);


            return new OkResult();
        }

        private void LogConfig(IEnumerable<IConfigurationSection> entries, ILogger log)
        {

            foreach (var entry in entries)
            {
                log.LogInformation($"{entry.Key} == {entry.Value}");
                var children = entry.GetChildren();
                if (children.Any())
                {
                    LogConfig(children, log);
                }
            }

        }
    }
}

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ode.Function
{
    public static class PostHttpTrigger
    {
        [FunctionName(nameof(GetHttpTrigger))]
        public static async Task<IActionResult> GetHttpTrigger(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/{id:int?}")] HttpRequest req, int? id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            int cleanId = 0;
            var found = true;
            if (id != null)
            {
                cleanId = id.Value;
            }
            else
            {
                if (req.Query.ContainsKey("id"))
                {
                    found = int.TryParse(req.Query["id"][0], out cleanId);
                }
                else
                {
                    found = false;
                }
            }

            var responseMessage = found ? $"The product name for your product id {cleanId} is Starfruit Explosion" : "No product ID";

            return new OkObjectResult(responseMessage);
        }
    }
}

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionChallenge1
{
    public static class azfchallenge1
    {
        [FunctionName("azfchallenge1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string id = req.Query["productId"];
            

            if(string.IsNullOrWhiteSpace(id))
    {
                return new NotFoundResult();
            }
            
            log.LogInformation("C# HTTP trigger function processed the product name for your product id = " + id.ToString());
            

            string responseMessage = string.IsNullOrEmpty(id)
                ? "the description is This starfruit ice cream is out of this world!"
                : $"The product name for your product id {id} is Starfruit Explosion and the description is This starfruit ice cream is out of this world!";

            return new OkObjectResult(responseMessage);
        }


    }
}

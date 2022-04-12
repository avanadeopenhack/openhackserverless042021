using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Bfyoc.FirstFunction
{
    public static class AvaEhr3FirstFunction
    {
        [FunctionName("AvaEhr3FirstFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest request,
            ILogger log)
        {
            log.LogInformation("Get information for a specific product id.");

            string productId = request.Query["productid"];

            string responseMessage = string.IsNullOrEmpty(productId)
                ? "The productid is required."
                : $"The product name for your product id {productId} is Starfruit Explosion";

            return new OkObjectResult(responseMessage);
        }

        // TODO Route
        [FunctionName("AvaEhr3FirstFunctionWithRoute")]
        public static async Task<IActionResult> RunWithRoute(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "AvaEhr3FirstFunction/products/{productId:alpha}")] HttpRequest request,
            string productIdFromRoute,
            ILogger log)
        {
            log.LogInformation("Get information for a specific product id.");

            string productId = string.IsNullOrEmpty(productIdFromRoute) ? (string)request.Query["productid"] : productIdFromRoute;

            string responseMessage = string.IsNullOrEmpty(productId)
                ? "The productid is required."
                : $"The product name for your product id {productId} is Starfruit Explosion";

            return new OkObjectResult(responseMessage);
        }
    }
}

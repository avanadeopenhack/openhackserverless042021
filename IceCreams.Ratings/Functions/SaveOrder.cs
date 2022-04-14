using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using IceCreams.Ratings.Managers;
using IceCreams.Ratings.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace IceCreams.Ratings.Functions
{
    public class SaveOrder
    {
        public IRatingManager _ratingManager { get; }

        public SaveOrder(IRatingManager ratingManager)
        {
            this._ratingManager = ratingManager;
        }

        [FunctionName("SaveOrder")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            IList<Order> model = await _ratingManager.ExtractModelFromHttpRequestAsync<List<Order>>(req);
            var key = req.Query["key"];
            try
            {
                await _ratingManager.SaveOrderAsync(model, key);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            return new OkResult();
        }
    }
}


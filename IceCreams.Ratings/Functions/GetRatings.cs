using IceCreams.Ratings.Infra.Dto;
using IceCreams.Ratings.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace IceCreams.Ratings.Functions
{
    public class GetRatings
    {
        public IRatingManager _ratingManager { get; }

        public GetRatings(IRatingManager ratingManager)
        {
            this._ratingManager = ratingManager;
        }

        [FunctionName("GetRatings")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **User Id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The ratings")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetRatings/{userId:alpha}")] HttpRequest request,
            string userId,
            [CosmosDB(
                databaseName: "IceCreamDb",
                collectionName: "Ratings",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM c WHERE c.userId = {userId} ORDER BY c._ts DESC")]
                IEnumerable<Rating> ratings,
            ILogger log)
        {
            log.LogInformation("Get the ratings from the Cosmos DB database.");

            // 

            // TODO gestion des userId (paramètre)
            // TODO conversion

            string responseMessage = JsonConvert.SerializeObject(ratings);

            return new OkObjectResult(responseMessage);
        }
    }
}


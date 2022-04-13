using IceCreams.Ratings.Managers;
using IceCreams.Ratings.Models;
using IceCreams.Ratings.Models.Dto;
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
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IceCreams.Ratings.Functions
{
    public class GetRating
    {
        public IRatingManager _ratingManager { get; }

        public GetRating(IRatingManager ratingManager)
        {
            _ratingManager = ratingManager;
        }

        [FunctionName("GetRating")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "ratingId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **Rating Id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The matching rating")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetRating/{ratingId}")] HttpRequest request,
            string ratingId,
            [CosmosDB(
                databaseName: "IceCreamDb",
                collectionName: "Ratings",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM c WHERE c.id = {ratingId} ORDER BY c._ts DESC")]
                IEnumerable<Rating> ratings,
            ILogger log)
        {
            log.LogInformation($"Get the rating {ratingId} from the Cosmos DB database.");

            if (ratings.Any())
            {
                RatingModel ratingModel = _ratingManager.ConvertRatingToModel(ratings.First());

                string responseMessage = JsonConvert.SerializeObject(ratingModel);

                return new OkObjectResult(responseMessage);
            }

            return new NotFoundResult();
        }
    }
}


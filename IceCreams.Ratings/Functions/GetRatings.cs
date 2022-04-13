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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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
        [OpenApiParameter(name: "userId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The **User Id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The ratings")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetRatings/{userId:guid}")] HttpRequest request,
            Guid userId,
            [CosmosDB(
                databaseName: "IceCreamDb",
                collectionName: "Ratings",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM c WHERE c.userId = {userId} ORDER BY c._ts DESC")]
                IEnumerable<Rating> ratings,
            ILogger log)
        {
            log.LogInformation($"Get the ratings from the Cosmos DB database for the user {userId}.");

            if (ratings.Any())
            {
                IEnumerable<RatingModel> ratingModelCollection = _ratingManager.ConvertRatingCollectionToModel(ratings);

                string responseMessage = JsonConvert.SerializeObject(ratingModelCollection.ToList());
                return new OkObjectResult(responseMessage);
            }

            return new NotFoundResult();
        }
    }
}


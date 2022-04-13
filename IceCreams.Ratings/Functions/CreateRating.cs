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
using System;
using System.Net;
using System.Threading.Tasks;

namespace IceCreams.Ratings.Functions
{
    public class CreateRating
    {
        private readonly IRatingManager _ratingManager;

        public CreateRating(IRatingManager ratingManager)
        {
            _ratingManager = ratingManager;
        }

        [FunctionName("CreateRating")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(RatingModel), Description = "The **rating** to create", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest request,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            RatingModel model = await _ratingManager.ExtractModelFromHttpRequestAsync(request);
            try
            {
                await _ratingManager.CreateAsync(model);
            }
            catch (ArgumentException e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            return new OkResult();
        }
    }
}


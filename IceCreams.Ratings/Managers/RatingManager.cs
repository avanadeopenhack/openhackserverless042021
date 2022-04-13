using IceCreams.Ratings.Infra.Dto;
using IceCreams.Ratings.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace IceCreams.Ratings.Managers
{
    public class RatingManager : IRatingManager
    {
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        private readonly string _cosmosDbConnectionString;

        public RatingManager(IConfiguration configuration)
        {
            _configuration = configuration;
            _baseUrl = configuration.GetValue<string>("BaseUrl");
            _cosmosDbConnectionString = configuration.GetValue<string>("CosmosDBConnection");
        }

        public async Task CreateAsync(RatingModel model)
        {
            var getUserUrl = $"{ _baseUrl}/GetUser?userId={model.UserId}";
            var getProductUrl = $"{ _baseUrl}/GetProduct?productId={model.ProductId}";

            var user = await GetAsync<UserModel>(getUserUrl);
            var product = await GetAsync<ProductModel>(getProductUrl);

            if (model.Rating > 5 || model.Rating < 0)
            {
                throw new ArgumentException("Rating should be between 0 and 5");
            }
            if (user == null)
            {
                throw new ArgumentException($"User {model.UserId} does not exist");
            }
            if (product == null)
            {
                throw new ArgumentException($"Product {model.ProductId} does not exist");
            }

            var ratingItem = new Rating
            {
                ProductId = model.ProductId.ToString(),
                UserId = model.UserId.ToString(),
                UserNotes = model.UserNotes,
                RatingScore = model.Rating,
                Id = Guid.NewGuid().ToString(),
                LocationName = "Sample ice cream shop"

            };
            var cosmosClientProvider = new CosmosClientProvider(_cosmosDbConnectionString, "IceCreamDb", "Ratings");
            await cosmosClientProvider.InsertAsync(ratingItem, "/id");


            await Task.CompletedTask;
        }

        private async Task<T> GetAsync<T>(string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(url);

                var model = await response.Content.ReadAsAsync<T>();
                return model;
            }
            catch { }

            return default(T);
        }
    }
}

using IceCreams.Ratings.Models;
using IceCreams.Ratings.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task<T> ExtractModelFromHttpRequestAsync<T>(HttpRequest request)
        {
            T model;
            using StreamReader streamReader = new StreamReader(request.Body);

            string requestBody = await streamReader.ReadToEndAsync();

            model = JsonConvert.DeserializeObject<T>(requestBody);

            return model;
        }

        public async Task CreateAsync(RatingModel model)
        {
            string getUserUrl = $"{ _baseUrl}/GetUser?userId={model.UserId}";
            string getProductUrl = $"{ _baseUrl}/GetProduct?productId={model.ProductId}";

            UserModel user = await GetAsync<UserModel>(getUserUrl);
            ProductModel product = await GetAsync<ProductModel>(getProductUrl);

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

            var ratingItem = ConvertRatingToDto(model, true); //new Rating
            //{
            //    ProductId = model.ProductId.ToString(),
            //    UserId = model.UserId.ToString(),
            //    UserNotes = model.UserNotes,
            //    RatingScore = model.Rating,
            //    Id = Guid.NewGuid().ToString(),
            //    LocationName = "Sample ice cream shop"

            //};
            var cosmosClientProvider = new CosmosClientProvider(_cosmosDbConnectionString, "IceCreamDb", "Ratings");
            await cosmosClientProvider.InsertAsync(ratingItem, ratingItem.Id);


            await Task.CompletedTask;
        }

        private async Task<T> GetAsync<T>(string url)
        {
            try
            {
                using HttpClient client = new HttpClient();
                var response = await client.GetAsync(url);

                var model = await response.Content.ReadAsAsync<T>();
                return model;
            }
            catch { }

            return default(T);
        }

        public IEnumerable<RatingModel> ConvertRatingCollectionToModel(IEnumerable<Rating> ratingCollection)
        {
            foreach (Rating rating in ratingCollection)
            {
                yield return ConvertRatingToModel(rating);
            }
        }

        public RatingModel ConvertRatingToModel(Rating rating)
        {
            return new RatingModel
            {
                RatingId = Guid.Parse(rating.Id),
                UserId = Guid.Parse(rating.UserId),
                ProductId = Guid.Parse(rating.ProductId),
                LocationName = rating.LocationName,
                Rating = rating.RatingScore,
                UserNotes = rating.UserNotes
            };
        }

        public IEnumerable<Rating> ConvertRatingCollectionToDto(IEnumerable<RatingModel> ratingCollection)
        {
            foreach (RatingModel rating in ratingCollection)
            {
                yield return ConvertRatingToDto(rating, false);
            }
        }

        public Rating ConvertRatingToDto(RatingModel rating, bool forceNewId)
        {
            return new Rating
            {
                Id = (forceNewId ? Guid.NewGuid() : rating.RatingId).ToString("D"),
                UserId = rating.UserId.ToString("D"),
                ProductId = rating.ProductId.ToString("D"),
                LocationName = rating.LocationName,
                RatingScore = rating.Rating,
                UserNotes = rating.UserNotes
            };
        }

        public async Task SaveOrderAsync(IEnumerable<Order> orders, string key)
        {
            var cosmosClientProvider = new CosmosClientProvider(_cosmosDbConnectionString, "IceCreamDb", "Orders");
            foreach (var order in orders)
            {
                order.id = Guid.NewGuid().ToString();
                order.key = order.headers.salesNumber;
                await cosmosClientProvider.InsertAsync(order, order.headers.salesNumber);
            }
        }
    }
}

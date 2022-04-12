using IceCreams.Ratings.Models.Dto;
using IceCreams.Ratings.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using Newtonsoft.Json;

namespace IceCreams.Ratings.Managers
{
    public class RatingManager : IRatingManager
    {
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public RatingManager(IConfiguration configuration)
        {
            _configuration = configuration;
            _baseUrl = configuration.GetValue<string>("BaseUrl");
        }

        public async Task<RatingModel> ExtractModelFromHttpRequestAsync(HttpRequest request)
        {
            RatingModel ratingModel;
            using StreamReader streamReader = new StreamReader(request.Body);

            string requestBody = await streamReader.ReadToEndAsync();

            ratingModel = JsonConvert.DeserializeObject<RatingModel>(requestBody);

            return ratingModel;
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
                yield return ConvertRatingToDto(rating);
            }
        }

        public Rating ConvertRatingToDto(RatingModel rating)
        {
            return new Rating
            {
                Id = rating.RatingId.ToString("D"),
                UserId = rating.UserId.ToString("D"),
                ProductId = rating.ProductId.ToString("D"),
                LocationName = rating.LocationName,
                RatingScore = rating.Rating,
                UserNotes = rating.UserNotes
            };
        }
    }
}

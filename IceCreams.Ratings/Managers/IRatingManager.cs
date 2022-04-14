using IceCreams.Ratings.Models;
using IceCreams.Ratings.Models.Dto;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IceCreams.Ratings.Managers
{
    public interface IRatingManager
    {
        Task<T> ExtractModelFromHttpRequestAsync<T>(HttpRequest request);

        Task CreateAsync(RatingModel model);

        IEnumerable<RatingModel> ConvertRatingCollectionToModel(IEnumerable<Rating> ratingCollection);

        RatingModel ConvertRatingToModel(Rating rating);

        IEnumerable<Rating> ConvertRatingCollectionToDto(IEnumerable<RatingModel> ratingCollection);

        Rating ConvertRatingToDto(RatingModel rating, bool forceNewId);

        Task SaveOrderAsync(IEnumerable<Order> content, string key);
    }
}
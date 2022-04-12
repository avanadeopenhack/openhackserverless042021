using IceCreams.Ratings.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IceCreams.Ratings.Managers
{
    public interface IRatingManager
    {
        Task CreateAsync(RatingModel model);
        Task<IEnumerable<RatingModel>> GetAllAsync();
        Task<RatingModel> GetAsync(Guid id);
    }
}
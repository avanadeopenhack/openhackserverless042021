using IceCreams.Ratings.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IceCreams.Ratings.Managers
{
    public class RatingManager : IRatingManager
    {
        public RatingManager()
        {

        }

        public async Task CreateAsync(RatingModel model)
        {

            await Task.CompletedTask;
        }

        public async Task<RatingModel> GetAsync(Guid id)
        {
            var response = new RatingModel();
            return response;
        }

        public async Task<IEnumerable<RatingModel>> GetAllAsync()
        {
            var response = new List<RatingModel>();
            return response;
        }
    }
}

using System;
using System.Text.Json.Serialization;

namespace IceCreams.Ratings.Models
{
    public class RatingModel
    {
        [JsonIgnore]
        public Guid RatingId { get; set; }

        public Guid UserId { get; set; }

        public Guid ProductId { get; set; }

        public string LocationName { get; set; }

        public int Rating { get; set; }

        public string UserNotes { get; set; }
    }
}
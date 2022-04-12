using Newtonsoft.Json;
using System;

namespace IceCreams.Ratings.Models.Dto
{
    public class Rating
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty("locationName")]
        public string LocationName { get; set; }

        [JsonProperty("rating")]
        public int RatingScore { get; set; }

        [JsonProperty("userNotes")]
        public string UserNotes { get; set; }
    }
}

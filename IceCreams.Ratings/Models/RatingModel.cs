using System;

namespace IceCreams.Ratings.Models
{
    public class RatingModel
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public string UserlocationNameerId { get; set; }
        public int Rating { get; set; }
        public string UserNotes { get; set; }
    }
}
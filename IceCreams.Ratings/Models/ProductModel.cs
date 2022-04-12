using System;

namespace IceCreams.Ratings.Models
{
    public class ProductModel
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductDescription { get; set; }
    }
}
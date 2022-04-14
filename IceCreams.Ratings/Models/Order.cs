using System;
using System.Collections.Generic;
using System.Text;

namespace IceCreams.Ratings.Models
{
    public class Headers
    {
        public string salesNumber { get; set; }
        public string dateTime { get; set; }
        public string locationId { get; set; }
        public string locationName { get; set; }
        public string locationAddress { get; set; }
        public string locationPostcode { get; set; }
        public string totalCost { get; set; }
        public string totalTax { get; set; }
        public string receiptUrl { get; set; }
    }

    public class Detail
    {
        public string productId { get; set; }
        public string quantity { get; set; }
        public string unitCost { get; set; }
        public string totalCost { get; set; }
        public string totalTax { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
    }

    public class Order
    {
        public string id { get; set; }
        public string key { get; set; }
        public Headers headers { get; set; }
        public List<Detail> details { get; set; }
    }


}

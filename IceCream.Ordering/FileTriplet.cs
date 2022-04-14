using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IceCream.Ordering
{
    public class FileTriplet
    {
        public void Add(string type, string fileUrl)
        {
            switch (type)
            {
                case "OrderHeaderDetails.csv":
                    orderHeaderDetailsCSVUrl = fileUrl;
                    break;
                case "OrderLineItems.csv":
                    orderLineItemsCSVUrl = fileUrl;
                    break;
                case "ProductInformation.csv":
                    productInformationCSVUrl = fileUrl;
                    break;

            }
        }

        public bool IsFull()
        {
            return !string.IsNullOrEmpty(productInformationCSVUrl) && !string.IsNullOrEmpty(orderHeaderDetailsCSVUrl) && !string.IsNullOrEmpty(orderLineItemsCSVUrl);
        }

        public string orderHeaderDetailsCSVUrl { get; set; }
        public string orderLineItemsCSVUrl { get; set; }
        public string productInformationCSVUrl { get; set; }
    }
}

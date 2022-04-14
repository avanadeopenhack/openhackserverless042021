using System;
using System.Collections.Generic;
using System.Text;

namespace IceCream.Ordering
{
    public class FileTriplet
    {
        public int Count = 0;
        public int Add(string type, string fileUrl)
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
            Count++;
            return Count;
        }

        public string orderHeaderDetailsCSVUrl { get; set; }
        public string orderLineItemsCSVUrl { get; set; }
        public string productInformationCSVUrl { get; set; }
    }
}

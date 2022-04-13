using System;
using System.Collections.Generic;
using System.Text;

namespace IceCreams.Ratings.Models
{
    public class UserModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
    }
}

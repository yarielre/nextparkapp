using System;
using System.Collections.Generic;
using System.Text;

namespace NextPark.Models
{
    public class UserModel : BaseModel
    {
        public double Coins { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }

        public string Name { get; set; }
        public string Lastname { get; set; }

        public string Address { get; set; }
        public string State { get; set; }
        public string CarPlate { get; set; }
        
    }
}

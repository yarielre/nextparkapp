using System;
using NextPark.Domain.Core;

namespace NextPark.Domain.Entities
{
    public class CarPlate : BaseEntity
    {
        public string Plate { get; set; }
        public ApplicationUser User { get; set; }
        public int UserId { get; set; }

    }
}

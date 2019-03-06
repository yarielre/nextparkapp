using NextPark.Domain.Core;

namespace NextPark.Domain.Entities
{
    public class Feed : BaseEntity
    {
        public string Name { get; set; }
        public double Tax { get; set; }
    }
}
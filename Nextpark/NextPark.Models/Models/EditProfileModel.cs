namespace NextPark.Models
{
    public class EditProfileModel : BaseModel
    {
        public string Name { get; set; }
        public string Lastname { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }

        public string Address { get; set; }
        public int Cap { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public string Phone { get; set; }

        public string CarPlate { get; set; }

        public string ImageUrl { get; set; }
        public byte[] ImageBinary { get; set; }

        public double Balance { get; set; }
        public double Profit { get; set; }
    }
}

namespace NextPark.Models
{
    public class RegisterModel : BaseModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string Name { get; set; }
        public string Lastname { get; set; }

        public string Address { get; set; }
        public string State { get; set; }
        public string CarPlate { get; set; }
    }
}

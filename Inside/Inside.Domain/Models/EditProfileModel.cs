namespace Inside.Domain.Models
{
    public class EditProfileModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

        public string Name { get; set; }
        public string Lastname { get; set; }

        public string Address { get; set; }
        public string State { get; set; }
        public string CarPlate { get; set; }
    }
}

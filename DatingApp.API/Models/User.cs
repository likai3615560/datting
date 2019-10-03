namespace DatingApp.API.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public byte[] UserPwd { get; set; }

        public byte[] UserPwdSalt { get; set; }

        public string Gender { get; set; }
    }
}
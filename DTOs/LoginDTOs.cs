using netCorePlayground.Models;

namespace netCorePlayground.DTOs
{
    public class LoginInputDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }


    public class LoginOutputDTO
    {
        public string Token { get; set; }
        public User User { get; set; }
    }
}
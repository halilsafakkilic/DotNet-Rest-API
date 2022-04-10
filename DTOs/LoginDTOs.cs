using netCorePlayground.Models;

namespace netCorePlayground.DTOs
{
    public class LoginInputDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


    public class LoginOutputDTO
    {
        public string AccessToken { get; set; }
    }
}
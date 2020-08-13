using System.Collections.Generic;

namespace netCorePlayground.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public IReadOnlyList<string> UserRoles { get; set; }
    }
}
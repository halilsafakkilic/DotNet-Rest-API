using Microsoft.AspNetCore.Mvc;

namespace netCorePlayground.Controllers.User
{
    [ApiController]
    [Area("api")]
    [Route("[area]")]
    public abstract class UserController : ControllerBase
    {

    }
}
using Microsoft.AspNetCore.Mvc;

namespace netCorePlayground.Controllers.Admin
{
    [ApiController]
    [Area("admin")]
    [Route("[area]")]
    public abstract class AdminController : ControllerBase
    {

    }
}
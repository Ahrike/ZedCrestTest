using Application.DTOS;
using Application.UserHandler;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static Application.UserHandler.Retrieve;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {

        [HttpPost("register")]
        public async Task<ActionResult<RegisterUserReturnDto>> Register([FromForm]Register.Command request)
        {
            return await Mediator.Send(request);
        }

        [HttpGet("{emailAddress}/{reference}")]
        public async Task<ActionResult<UserDetailsDto>> Retrieve(string emailAddress,string reference)
        {
            return await Mediator.Send(new Query { EmailAddress = emailAddress, Reference=reference});
        }
    }
}

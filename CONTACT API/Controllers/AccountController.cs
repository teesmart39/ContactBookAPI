using Core.API.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Model.API.Entity;

namespace CONTACT_API.Controllers
{

    [Route("users/")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;


        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] Register register,  string role)
        {
            try
            {
                var result = await _accountRepository.SignUpAsync(register, role);
                if (result.Succeeded)
                {
                    return Ok(result.Succeeded);
                }
                return Unauthorized(result);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] SignInModel signInModel)
        {
            var result = await _accountRepository.LoginAsync(signInModel);
            if (string.IsNullOrEmpty(result))
            {
                return Unauthorized(result);
            }
            return Ok (result);
        }

    }
}

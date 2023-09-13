using Mango.Services.AuthAPI.Models.DTOs;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseType _responseType;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
            _responseType = new ResponseType();
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO registration)
        {
           var errorMessage = await _authService.SignUp(registration);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _responseType.IsSuccess = false;
                _responseType.Message = errorMessage;
                return BadRequest(_responseType);
            }
            return Ok(_responseType);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO login)
        {
            var loginResponse = await _authService.Login(login);
            if(loginResponse.User == null)
            {
                _responseType.IsSuccess = false;
                _responseType.Message = "Username or Password is incorrect";
                return BadRequest(_responseType);
            }
            _responseType.Result = loginResponse;
            return Ok(_responseType);
        }

        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
            var user = await _authService.AssignRole(registrationRequestDTO.Email, registrationRequestDTO.Role);
            if (!user)
            {
                _responseType.IsSuccess = false;
                _responseType.Message = "Error encountered";
                return BadRequest(_responseType);
            }
            return Ok(_responseType);
        }

    }
}

using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class AuthController: Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }
        #region Login
        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO loginRequestDTO = new();
            return View(loginRequestDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            ResponseType responseDto = await _authService.LoginAsync(loginRequestDTO);

            if (responseDto?.Result != null && responseDto.IsSuccess)
            {
                LoginResponseDTO loginResponseDto =
                    JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(responseDto?.Result));

                await SignInUser(loginResponseDto);
                _tokenProvider.SetToken(loginResponseDto.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = responseDto?.Message;
                return View(loginRequestDTO);
            }
        }
        #endregion

        #region Register

        [HttpGet]
        public IActionResult Register()
        {
            PopulateDropDown();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDTO registrationRequest)
        {
            ResponseType result = await _authService.RegisterAsync(registrationRequest);
            ResponseType assingRole;

            if (result != null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(registrationRequest.Role))
                {
                    registrationRequest.Role = StaticDetails.RoleCustomer;
                }
                assingRole = await _authService.AssignRole(registrationRequest);
                if (assingRole != null && assingRole.IsSuccess)
                {
                    TempData["success"] = "Registration Successful";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = result?.Message;
            }
            PopulateDropDown();
            return View(registrationRequest);
        }

        #endregion

        #region Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");

        }
        #endregion

        private void PopulateDropDown()
        {
            var rolelist = new List<SelectListItem>()
            {
                new SelectListItem{Text=StaticDetails.RoleAdmin, Value=StaticDetails.RoleAdmin},
                new SelectListItem{Text=StaticDetails.RoleCustomer, Value=StaticDetails.RoleCustomer},
                new SelectListItem{Text=StaticDetails.RoleFrontDesk, Value=StaticDetails.RoleFrontDesk},
                new SelectListItem{Text=StaticDetails.RoleKitchen, Value=StaticDetails.RoleKitchen},
            };

            ViewBag.RoleList = rolelist;
        }

        // This method is used for setting Idenity as signed in user
        private async Task SignInUser(LoginResponseDTO loginResponseDTO)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(loginResponseDTO.Token);

            if (jwt != null)
            {
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                    jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
                identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                    jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
                identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                    jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));


                identity.AddClaim(new Claim(ClaimTypes.Name,
                    jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

                identity.AddClaim(new Claim(ClaimTypes.Role,
                    jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }

        }

    }
}

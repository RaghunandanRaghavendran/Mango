using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseType?> AssignRole(RegistrationRequestDTO registrationRequestDTO)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = registrationRequestDTO,
                Url = StaticDetails.AuthAPIBase + "/api/auth/assignrole"
            });
        }

        public async Task<ResponseType?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = loginRequestDTO,
                Url = StaticDetails.AuthAPIBase + "/api/auth/login"
            }, withBearer:false);
        }

        public async Task<ResponseType?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = registrationRequestDTO,
                Url = StaticDetails.AuthAPIBase + "/api/auth/register"
            }, withBearer:false);
        }
    }
}

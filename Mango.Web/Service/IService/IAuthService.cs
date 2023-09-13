using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseType?> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<ResponseType?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO);
        Task<ResponseType?> AssignRole(RegistrationRequestDTO registrationRequestDTO);
    }
}

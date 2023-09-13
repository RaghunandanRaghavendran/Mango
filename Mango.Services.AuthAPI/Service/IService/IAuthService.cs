using Mango.Services.AuthAPI.Models.DTOs;

namespace Mango.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        // This is not being used here, But i wanted this way of implementation rather than SignUp which i didnt like it
        Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);

        Task<string> SignUp(RegistrationRequestDTO registrationRequestDTO);

        Task<bool> AssignRole(string email, string roleName);

    }
}

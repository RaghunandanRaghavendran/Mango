using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IBaseService
    {
        Task<ResponseType?> SendAsync(RequestType requestType, bool withBearer = true);
    }
}

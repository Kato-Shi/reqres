using System.Threading.Tasks;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Entities;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Manager
{
    public interface IUserManager : IGenericManager<User>
    {
        Task<CreateUserResponse?> CreateUserAsync(CreateUserRequest requestBody);
    }
}

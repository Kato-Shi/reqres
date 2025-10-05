using System;
using System.Threading.Tasks;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Entities;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Manager;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Services
{
    public class UserServices
    {
        private readonly IUserManager _userManager;

        public UserServices(IUserManager userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public Task<User?> GetUsers(int page = 1, int perPage = 6) =>
            _userManager.GetAll(page, perPage);

        public Task<CreateUserResponse?> PostUser(CreateUserRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return _userManager.CreateUserAsync(request);
        }
    }
}

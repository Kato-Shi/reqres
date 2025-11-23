using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Entities;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Services;

namespace ReqresIntegratedApplication.WebAPI.Services
{
    /// <summary>
    /// Coordinates ReqRes user calls and maintains a local cache to simulate persistence for updates.
    /// </summary>
    public class EmployeeService
    {
        private readonly ReqResClient _client;
        private readonly Dictionary<int, UserData> _localUsers = new();

        public EmployeeService(ReqResClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<User?> GetUsersAsync(int page, int perPage)
        {
            var response = await _client.GetUsersAsync(page, perPage);
            if (response?.Data is { Count: > 0 })
            {
                foreach (var user in response.Data)
                {
                    _localUsers[user.Id] = user;
                }
            }

            return response;
        }

        public async Task<UserData?> GetUserAsync(int id)
        {
            if (_localUsers.TryGetValue(id, out var cached))
            {
                return cached;
            }

            var response = await _client.GetUserAsync(id);
            if (response?.Data is not null)
            {
                _localUsers[id] = response.Data;
            }

            return response?.Data;
        }

        public async Task<CreateUserResponse?> CreateUserAsync(CreateUserRequest request)
        {
            var created = await _client.CreateUserAsync(request);
            if (created is not null)
            {
                var (firstName, lastName) = SplitName(request.Name);
                var synthetic = new UserData
                {
                    Id = int.TryParse(created.Id, out var newId) ? newId : _localUsers.Keys.DefaultIfEmpty().Max() + 1,
                    Email = $"{request.Name.Replace(" ", ".").ToLower()}@reqres.in",
                    FirstName = firstName,
                    LastName = lastName,
                    Avatar = null
                };

                _localUsers[synthetic.Id] = synthetic;
            }

            return created;
        }

        public async Task<UpdateUserResponse?> UpdateUserAsync(int id, UpdateUserRequest request, bool isPatch)
        {
            var response = isPatch
                ? await _client.PatchUserAsync(id, request)
                : await _client.PutUserAsync(id, request);

            if (response is not null)
            {
                if (!_localUsers.TryGetValue(id, out var existing))
                {
                    existing = new UserData { Id = id };
                }

                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    var (firstName, lastName) = SplitName(request.Name);
                    existing.FirstName = firstName;
                    existing.LastName = lastName;
                }

                existing.FirstName ??= string.Empty;
                existing.LastName ??= string.Empty;
                existing.Email ??= string.Empty;
                existing.Avatar = existing.Avatar;
                _localUsers[id] = existing;
            }

            return response;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var deleted = await _client.DeleteUserAsync(id);
            if (deleted)
            {
                _localUsers.Remove(id);
            }

            return deleted;
        }

        public IReadOnlyCollection<UserData> GetLocalUsers() => _localUsers.Values.ToList();

        private static (string FirstName, string LastName) SplitName(string name)
        {
            var parts = name?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            if (parts.Length == 0)
            {
                return (string.Empty, string.Empty);
            }

            var firstName = parts[0];
            var lastName = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;
            return (firstName, lastName);
        }
    }
}

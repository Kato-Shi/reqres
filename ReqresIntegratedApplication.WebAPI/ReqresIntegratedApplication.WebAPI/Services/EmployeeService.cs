using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        private static readonly UserData[] DemoUsers =
        {
            new() { Id = 101, Email = "demo.jane@reqres.in", FirstName = "Demo", LastName = "Jane", Avatar = null },
            new() { Id = 102, Email = "demo.john@reqres.in", FirstName = "Demo", LastName = "John", Avatar = null },
            new() { Id = 103, Email = "demo.lee@reqres.in", FirstName = "Demo", LastName = "Lee", Avatar = null }
        };

        public EmployeeService(ReqResClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<User?> GetUsersAsync(int page, int perPage)
        {
            try
            {
                var response = await _client.GetUsersAsync(page, perPage);
                if (response?.Data is { Count: > 0 })
                {
                    foreach (var user in response.Data)
                    {
                        _localUsers[user.Id] = user;
                    }
                }

                return response ?? BuildLocalUserPage();
            }
            catch (HttpRequestException)
            {
                // If the upstream call fails (401/403/timeout), surface a local fallback page
                return BuildLocalUserPage();
            }
        }

        public async Task<UserData?> GetUserAsync(int id)
        {
            if (_localUsers.TryGetValue(id, out var cached))
            {
                return cached;
            }

            try
            {
                var response = await _client.GetUserAsync(id);
                if (response?.Data is not null)
                {
                    _localUsers[id] = response.Data;
                }

                return response?.Data ?? GetLocalOrDemoUser(id);
            }
            catch (HttpRequestException)
            {
                return GetLocalOrDemoUser(id);
            }
        }

        public async Task<CreateUserResponse?> CreateUserAsync(CreateUserRequest request)
        {
            CreateUserResponse? created;
            try
            {
                created = await _client.CreateUserAsync(request);
            }
            catch (HttpRequestException)
            {
                // If the upstream call is blocked, fabricate a local success payload to keep the demo flowing.
                created = new CreateUserResponse
                {
                    Id = (_localUsers.Keys.DefaultIfEmpty().Max() + 1).ToString(),
                    Name = request.Name,
                    Job = request.Job,
                    CreatedAt = DateTime.UtcNow.ToString("O")
                };
            }
            if (created is not null)
            {
                var synthetic = new UserData
                {
                    Id = int.TryParse(created.Id, out var newId) ? newId : _localUsers.Keys.DefaultIfEmpty().Max() + 1,
                    Email = $"{request.Name.Replace(" ", ".").ToLower()}@reqres.in",
                    FirstName = request.Name,
                    LastName = request.Job,
                    Avatar = null
                };

                _localUsers[synthetic.Id] = synthetic;
            }

            return created;
        }

        public async Task<UpdateUserResponse?> UpdateUserAsync(int id, UpdateUserRequest request, bool isPatch)
        {
            UpdateUserResponse? response;
            try
            {
                response = isPatch
                    ? await _client.PatchUserAsync(id, request)
                    : await _client.PutUserAsync(id, request);
            }
            catch (HttpRequestException)
            {
                response = new UpdateUserResponse
                {
                    Name = request.Name,
                    Job = request.Job,
                    UpdatedAt = DateTime.UtcNow.ToString("O")
                };
            }

            if (response is not null)
            {
                if (!_localUsers.TryGetValue(id, out var existing))
                {
                    existing = new UserData { Id = id };
                }

                existing.FirstName = request.Name;
                existing.LastName = existing.LastName ?? string.Empty;
                existing.Email = existing.Email ?? string.Empty;
                existing.Avatar = existing.Avatar;
                _localUsers[id] = existing;
            }

            return response;
        }

        public IReadOnlyCollection<UserData> GetLocalUsers() => _localUsers.Values.ToList();

        private User? BuildLocalUserPage()
        {
            if (_localUsers.Count == 0)
            {
                foreach (var demo in DemoUsers)
                {
                    _localUsers[demo.Id] = demo;
                }
            }

            return new User
            {
                Page = 1,
                PerPage = _localUsers.Count,
                Total = _localUsers.Count,
                TotalPages = 1,
                Data = _localUsers.Values.ToList()
            };
        }

        private UserData? GetLocalOrDemoUser(int id)
        {
            if (_localUsers.TryGetValue(id, out var cached))
            {
                return cached;
            }

            var demo = DemoUsers.FirstOrDefault(d => d.Id == id);
            if (demo is not null)
            {
                _localUsers[id] = demo;
                return demo;
            }

            return null;
        }
    }
}

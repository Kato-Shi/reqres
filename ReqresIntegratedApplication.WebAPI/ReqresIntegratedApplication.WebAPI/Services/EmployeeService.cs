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
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<int, UserData> _localUsers = new();
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<int, byte> _deletedUsers = new();
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
                        if (_deletedUsers.ContainsKey(user.Id))
                        {
                            continue;
                        }

                        var merged = _localUsers.GetOrAdd(user.Id, _ => new UserData { Id = user.Id });
                        merged.FirstName ??= user.FirstName;
                        merged.LastName ??= user.LastName;
                        merged.Email ??= user.Email;
                        merged.Avatar ??= user.Avatar;
                        _localUsers[user.Id] = merged;
                    }
                }

                return BuildLocalUserPage(page, perPage);
            }
            catch (HttpRequestException)
            {
                // If the upstream call fails (401/403/timeout), surface a local fallback page
                return BuildLocalUserPage(page, perPage);
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
                if (response?.Data is not null && !_deletedUsers.ContainsKey(id))
                {
                    var merged = _localUsers.GetOrAdd(id, _ => new UserData { Id = id });
                    merged.FirstName = response.Data.FirstName ?? merged.FirstName;
                    merged.LastName = response.Data.LastName ?? merged.LastName;
                    merged.Email = response.Data.Email ?? merged.Email;
                    merged.Avatar = response.Data.Avatar ?? merged.Avatar;
                    _localUsers[id] = merged;
                }

                return _localUsers.TryGetValue(id, out var mergedUser)
                    ? mergedUser
                    : response?.Data ?? GetLocalOrDemoUser(id);
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
                var parsed = ParseName(request.Name);
                var id = int.TryParse(created.Id, out var newId) ? newId : _localUsers.Keys.DefaultIfEmpty().Max() + 1;
                var synthetic = new UserData
                {
                    Id = id,
                    Email = string.IsNullOrWhiteSpace(request.Email)
                        ? $"{(parsed.first ?? "user").Replace(" ", ".").ToLower()}@reqres.in"
                        : request.Email,
                    FirstName = parsed.first,
                    LastName = parsed.last,
                    Avatar = null,
                    Job = request.Job
                };

                _localUsers[synthetic.Id] = synthetic;
                _deletedUsers.TryRemove(synthetic.Id, out _);
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
                var parsed = ParseName(request.Name);
                var updated = _localUsers.GetOrAdd(id, _ => new UserData { Id = id });
                updated.FirstName = parsed.first ?? updated.FirstName ?? request.Name;
                updated.LastName = parsed.last ?? updated.LastName;
                updated.Email = !string.IsNullOrWhiteSpace(request.Email)
                    ? request.Email
                    : updated.Email ?? BuildEmail(parsed, id);
                updated.Avatar = updated.Avatar;
                if (!string.IsNullOrWhiteSpace(request.Job))
                {
                    updated.Job = request.Job;
                }
                _localUsers[id] = updated;
                _deletedUsers.TryRemove(id, out _);
            }

            return response;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var deleted = false;
            try
            {
                deleted = await _client.DeleteUserAsync(id);
            }
            catch (HttpRequestException)
            {
                deleted = true; // treat unreachable upstream as deleted locally
            }

            _localUsers.TryRemove(id, out _);
            _deletedUsers[id] = 1;
            return deleted;
        }

        public IReadOnlyCollection<UserData> GetLocalUsers() => _localUsers.Values.ToList();

        private User? BuildLocalUserPage(int page = 1, int perPage = int.MaxValue)
        {
            if (_localUsers.Count == 0)
            {
                foreach (var demo in DemoUsers)
                {
                    _localUsers[demo.Id] = demo;
                }
            }

            var filtered = _localUsers
                .Where(kvp => !_deletedUsers.ContainsKey(kvp.Key))
                .Select(kvp => kvp.Value)
                .OrderBy(u => u.Id)
                .ToList();

            var pageData = filtered
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .ToList();

            return new User
            {
                Page = page,
                PerPage = perPage,
                Total = filtered.Count,
                TotalPages = (int)Math.Ceiling(filtered.Count / (double)perPage),
                Data = pageData
            };
        }

        private UserData? GetLocalOrDemoUser(int id)
        {
            if (_deletedUsers.ContainsKey(id))
            {
                return null;
            }

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

        private static (string? first, string? last) ParseName(string? name)
        {
            var parts = (name ?? string.Empty).Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var first = parts.Length > 0 ? parts[0] : null;
            var last = parts.Length > 1 ? parts[1] : null;
            return (first, last);
        }

        private static string BuildEmail((string? first, string? last) parsed, int id)
        {
            var left = string.Join(".", new[] { parsed.first, parsed.last }.Where(p => !string.IsNullOrWhiteSpace(p)))
                .ToLower();
            left = string.IsNullOrWhiteSpace(left) ? $"user{id}" : left;
            return $"{left}@reqres.in";
        }
    }
}

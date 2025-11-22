using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReqresIntegratedApplication.WebAPI.Models;

namespace ReqresIntegratedApplication.WebAPI.Services
{
    /// <summary>
    /// Maintains clerks locally, layering roles and resource assignments on top of ReqRes users.
    /// </summary>
    public class ClerkService
    {
        private readonly EmployeeService _employees;
        private readonly ResourceService _resources;
        private readonly Dictionary<int, ClerkModel> _clerks = new();

        public ClerkService(EmployeeService employees, ResourceService resources)
        {
            _employees = employees ?? throw new ArgumentNullException(nameof(employees));
            _resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        public async Task<ClerkModel?> PromoteAsync(int userId, string role)
        {
            var user = await _employees.GetUserAsync(userId);
            if (user is null)
            {
                return null;
            }

            var clerk = new ClerkModel
            {
                UserId = user.Id,
                FullName = string.Join(" ", new[] { user.FirstName, user.LastName }.Where(x => !string.IsNullOrWhiteSpace(x))),
                Role = role,
                AssignedResourceIds = _clerks.TryGetValue(user.Id, out var existing)
                    ? new List<int>(existing.AssignedResourceIds)
                    : new List<int>()
            };

            _clerks[user.Id] = clerk;
            return clerk;
        }

        public ClerkModel? GetClerk(int userId)
        {
            _clerks.TryGetValue(userId, out var clerk);
            return clerk;
        }

        public IEnumerable<ClerkModel> GetClerks() => _clerks.Values;

        public bool AssignResources(int userId, IEnumerable<int> resourceIds)
        {
            if (!_clerks.TryGetValue(userId, out var clerk))
            {
                return false;
            }

            var validResources = _resources.GetCachedResources().Select(r => r.Id).ToHashSet();
            var filtered = resourceIds.Where(validResources.Contains).Distinct().ToList();
            clerk.AssignedResourceIds = filtered;
            _clerks[userId] = clerk;
            return true;
        }
    }
}

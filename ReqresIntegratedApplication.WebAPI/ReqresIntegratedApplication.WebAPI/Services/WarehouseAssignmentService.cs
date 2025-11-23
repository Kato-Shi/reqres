using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReqresIntegratedApplication.WebAPI.Models;

namespace ReqresIntegratedApplication.WebAPI.Services
{
    /// <summary>
    /// Manages warehouse associates locally, layering roles and in-memory item assignments on top of ReqRes users.
    /// </summary>
    public class WarehouseAssignmentService
    {
        private readonly EmployeeService _employees;
        private readonly ResourceService _resources;
        private readonly ConcurrentDictionary<int, WarehouseAssociate> _associates = new();

        public WarehouseAssignmentService(EmployeeService employees, ResourceService resources)
        {
            _employees = employees ?? throw new ArgumentNullException(nameof(employees));
            _resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        public async Task<WarehouseAssociate?> PromoteAsync(int userId, string role)
        {
            var user = await _employees.GetUserAsync(userId);
            if (user is null)
            {
                return null;
            }

            var associate = new WarehouseAssociate
            {
                UserId = user.Id,
                FullName = string.Join(" ", new[] { user.FirstName, user.LastName }.Where(x => !string.IsNullOrWhiteSpace(x))),
                Role = role,
                AssignedItemIds = _associates.TryGetValue(user.Id, out var existing)
                    ? new List<int>(existing.AssignedItemIds)
                    : new List<int>()
            };

            _associates[user.Id] = associate;
            return associate;
        }

        public WarehouseAssociate? GetAssociate(int userId)
        {
            _associates.TryGetValue(userId, out var associate);
            return associate;
        }

        public IEnumerable<WarehouseAssociate> GetAssociates() => _associates.Values;

        public bool AssignItems(int userId, IEnumerable<int> itemIds)
        {
            if (!_associates.TryGetValue(userId, out var associate))
            {
                return false;
            }

            var validItems = _resources.GetCachedResources().Select(r => r.Id).ToHashSet();
            var filtered = itemIds.Where(validItems.Contains).Distinct().ToList();
            associate.AssignedItemIds = filtered;
            _associates[userId] = associate;
            return true;
        }
    }
}

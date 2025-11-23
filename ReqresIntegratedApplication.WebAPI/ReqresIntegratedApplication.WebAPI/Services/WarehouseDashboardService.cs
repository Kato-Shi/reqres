using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Entities;
using ReqresIntegratedApplication.WebAPI.Models;

namespace ReqresIntegratedApplication.WebAPI.Services
{
    public class WarehouseDashboardService
    {
        private const string UsersBaseUrl = "https://reqres.in/api/users";
        private readonly EmployeeService _employeeService;
        private readonly ResourceService _resourceService;

        public WarehouseDashboardService(EmployeeService employeeService, ResourceService resourceService)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _resourceService = resourceService ?? throw new ArgumentNullException(nameof(resourceService));
        }

        public async Task<EmployeePageDto?> GetEmployeesAsync(int page = 1, int perPage = 6)
        {
            var response = await _employeeService.GetUsersAsync(page, perPage);
            if (response is null)
            {
                return null;
            }

            var members = MapMembers(response.Data);
            var resources = await _resourceService.GetResourcesAsync(page, perPage) ?? new Resource();
            return new EmployeePageDto
            {
                Page = response.Page,
                PerPage = response.PerPage,
                Total = response.Total,
                TotalPages = response.TotalPages,
                Members = members,
                Summary = new WorkforceSummaryDto
                {
                    Page = response.Page,
                    PerPage = response.PerPage,
                    TotalMembers = response.Total,
                    TotalPages = response.TotalPages,
                    CountOnPage = members.Count,
                    ResourceCountOnPage = resources.Data?.Count ?? 0,
                    TotalResources = resources.Total
                }
            };
        }

        public async Task<WorkforceSummaryDto?> GetWorkforceSummaryAsync(int page = 1, int perPage = 6)
        {
            var roster = await GetEmployeesAsync(page, perPage);
            return roster?.Summary;
        }

        public async Task<HiredEmployeeResponse?> HireEmployeeAsync(HireEmployeeRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var createUserRequest = new CreateUserRequest(request.Name, request.Role);
            var created = await _employeeService.CreateUserAsync(createUserRequest);
            if (created is null)
            {
                return null;
            }

            return new HiredEmployeeResponse
            {
                Id = created.Id ?? string.Empty,
                Name = created.Name ?? request.Name,
                Role = created.Job ?? request.Role,
                CreatedAt = created.CreatedAt,
                ResourceUrl = !string.IsNullOrWhiteSpace(created.Id) ? $"{UsersBaseUrl}/{created.Id}" : UsersBaseUrl,
                Message = $"Welcome to the warehouse team, {created.Name ?? request.Name}!"
            };
        }

        private static List<EmployeeListItemDto> MapMembers(IReadOnlyCollection<UserData>? data)
        {
            if (data is null || data.Count == 0)
            {
                return new List<EmployeeListItemDto>();
            }

            return data
                .Select(user => new EmployeeListItemDto
                {
                    Id = user.Id,
                    FullName = string.Join(" ", new[] { user.FirstName, user.LastName }.Where(x => !string.IsNullOrWhiteSpace(x))).Trim(),
                    Email = user.Email ?? string.Empty,
                    Avatar = user.Avatar
                })
                .ToList();
        }
    }
}

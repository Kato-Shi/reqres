using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Entities;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Services;
using ReqresIntegratedApplication.WebAPI.Models;

namespace ReqresIntegratedApplication.WebAPI.Services
{
    public class TeamShiftDashboardService
    {
        private const string UsersBaseUrl = "https://reqres.in/api/users";
        private readonly UserServices _userServices;

        public TeamShiftDashboardService(UserServices userServices)
        {
            _userServices = userServices ?? throw new ArgumentNullException(nameof(userServices));
        }

        public async Task<RosterPageDto?> GetRosterAsync(int page = 1, int perPage = 6)
        {
            var response = await _userServices.GetUsers(page, perPage);
            if (response is null)
            {
                return null;
            }

            var members = MapMembers(response.Data);
            return new RosterPageDto
            {
                Page = response.Page,
                PerPage = response.PerPage,
                Total = response.Total,
                TotalPages = response.TotalPages,
                Members = members,
                Summary = new HeadcountSummaryDto
                {
                    Page = response.Page,
                    PerPage = response.PerPage,
                    TotalMembers = response.Total,
                    TotalPages = response.TotalPages,
                    CountOnPage = members.Count
                }
            };
        }

        public async Task<HeadcountSummaryDto?> GetHeadcountAsync(int page = 1, int perPage = 6)
        {
            var roster = await GetRosterAsync(page, perPage);
            return roster?.Summary;
        }

        public async Task<OnboardedMemberResponse?> OnboardMemberAsync(OnboardMemberRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var createUserRequest = new CreateUserRequest(request.Name, request.Role);
            var created = await _userServices.PostUser(createUserRequest);
            if (created is null)
            {
                return null;
            }

            return new OnboardedMemberResponse
            {
                Id = created.Id ?? string.Empty,
                Name = created.Name ?? request.Name,
                Role = created.Job ?? request.Role,
                CreatedAt = created.CreatedAt,
                ResourceUrl = !string.IsNullOrWhiteSpace(created.Id) ? $"{UsersBaseUrl}/{created.Id}" : UsersBaseUrl,
                Message = $"Welcome to TeamShift Lite, {created.Name ?? request.Name}!"
            };
        }

        private static List<RosterMemberDto> MapMembers(IReadOnlyCollection<UserData>? data)
        {
            if (data is null || data.Count == 0)
            {
                return new List<RosterMemberDto>();
            }

            return data
                .Select(user => new RosterMemberDto
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

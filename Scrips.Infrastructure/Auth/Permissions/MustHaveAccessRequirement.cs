using MediatR.Behaviors.Authorization;
using Scrips.Core.Application.Common.Interfaces;

namespace Scrips.Infrastructure.Auth.Permissions;

public class MustHaveAccessRequirement : IAuthorizationRequirement
{
    public string Permission { get; set; }
    public string UserId { get; set; }
    public string ResourceUri { get; set; }

    private class MustHaveAccessRequirementHandler : IAuthorizationHandler<MustHaveAccessRequirement>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IPermissionProvider _permissionProvider;

        public MustHaveAccessRequirementHandler(ICurrentUser currentUser)//, IPermissionProvider permissionProvider)
        {
            _currentUser = currentUser;
            //_permissionProvider = permissionProvider;
        }

        public async Task<AuthorizationResult> Handle(
            MustHaveAccessRequirement request,
            CancellationToken cancellationToken)
        {
            // Skip if it's an scrips super admin
            if (_currentUser.IsSSA())
                return AuthorizationResult.Succeed();

            // 1- Get the roles assignments
            //var roles = GetRoleAssignmentsForResource(request.ResourceUri, request.UserId);

            // 2- Evaluate the permissions
            return MediatR.Behaviors.Authorization.AuthorizationResult.Fail("Access Denied.");
        }

        private List<string> GetRoleAssignmentsForResource(string uri, string userId)
        {
            string[] split = uri.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var splitted = new List<string>();
            string concat = string.Empty;
            foreach (string item in split)
            {
                concat += "/" + item;
                splitted.Add(concat);
            }

            return RoleAssignments.Where(ra => splitted.Contains(ra.Item1) && ra.Item2 == userId).Select(ra => ra.Item3)
                .ToList();
        }

        static List<(string, string, string)> RoleAssignments = new()
        {
            new("/valley/goldenvalley/east", "EastMan", "Admin"),
            new("/valley/goldenvalley/west", "WestMan", "Admin"),
            new("/valley", "aa7f0a18-c8ea-4b72-976e-5b8810c4b7aa", "GlobalAdmin"),

            new("/t(id)/p(id)", "u1", "Player3"),
            new("/t(id)", "u1", "Player4"),

            new("/t(id)/p(id)/p(id)/pt(id)", "u2", "Owner"),
            new("/t(id)", "u2", "Reader"),
        };

        static List<string> AllPermissions = new List<string>()
        {
            "Tenants.View", "Tenants.Get"
        };
    }
}

public interface IPermissionProvider
{
}
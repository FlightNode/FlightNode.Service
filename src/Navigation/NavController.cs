using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Http;
using FligthNode.Common.Api.Controllers;
using System.Linq;

namespace FlightNode.Service.Navigation
{
    public class NavController : LoggingController
    {
        public IHttpActionResult Get()
        {
            var reqContext = this.RequestContext;

            var parent = new NavigationNode();
            parent.AddChild(new NavigationNode("Login", "#/login"));

            var claimsPrincipal = reqContext.Principal as ClaimsPrincipal;
            if (claimsPrincipal == null)
            {
                return Ok(parent);
            }

            var claims = (claimsPrincipal.Claims ?? new List<Claim>()).ToList();
            if (claims.Any(x => HasRole(x, "Administrator")))
            {
                parent = AddAdministrativeTree(parent);
            }
            if (claims.Any(x => HasRole(x, "Reporter")))
            {
                parent = AddReporterTree(parent);
            }

            return Ok(parent);
        }

        private static bool HasRole(Claim x, string roleName)
        {
            return x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && 
                   x.Value == roleName;
        }

        private NavigationNode AddAdministrativeTree(NavigationNode parent)
        {
            parent = BuildUserMenu(parent);



            return parent;
        }

        private static NavigationNode BuildUserMenu(NavigationNode parent)
        {
            var users = new NavigationNode("Manage", "#/users");

            users.AddChild(new NavigationNode("Users", "#/users"));
            users.AddChild(new NavigationNode("Work Types", "#/worktypes"));
            users.AddChild(new NavigationNode("Locations", "#/locations"));

            parent.AddChild(users);
            return parent;
        }

        private NavigationNode AddReporterTree(NavigationNode parent)
        {
            var collection = new NavigationNode("Reporting", "");

            collection.AddChild(new NavigationNode("Work Log", "#/workday/create"));

            return parent;
        }
    }
}
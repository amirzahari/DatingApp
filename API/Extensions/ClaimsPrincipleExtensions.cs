using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            // [aznote] Get user username from the token the api users to authenticate this user.
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
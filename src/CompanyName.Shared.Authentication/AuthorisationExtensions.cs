using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace CompanyName.Shared.Authentication
{
    public static class AuthorisationExtensions
    {
        public static bool HasPermission(this SecurityToken token, string permission)
        {
            if (token is JwtSecurityToken)
            {
                return HasPermission((JwtSecurityToken)token, permission);
            }
            return false;
        }
        public static bool HasPermission(this SecurityToken token, string permission, string entity)
        {
            return HasPermission(token, permission + ':' + entity);
        }

        public static bool HasPermission(this JwtSecurityToken token, string permission, string entity)
        {
            return HasPermission(token, permission + ':' + entity);
        }
        public static bool HasPermission(this JwtSecurityToken token, string permission)
        {
            var scope = token.Claims.FirstOrDefault(n => n.Type.Equals("scope", StringComparison.OrdinalIgnoreCase))?.Value;
            if (!string.IsNullOrWhiteSpace(scope))
            {
                var scopes = scope.Split(' ');
                return scopes.Any(n => n.Equals(permission, StringComparison.OrdinalIgnoreCase));
            }
            //return token.Claims.Any(n => n.Type.StartsWith("permission", StringComparison.OrdinalIgnoreCase) && n.Value.Equals(permission, StringComparison.OrdinalIgnoreCase));
            return false;
        }

        public static string GetIdentity(this ClaimsPrincipal user)
        {
            return user.FindFirst(n => n.Type == ClaimTypes.NameIdentifier).Value;
        }

        public static Guid? GetId(this ClaimsPrincipal user)
        {
            var id = user.FindFirst(n => n.Type == "https://CompanyName.net/uuid")?.Value;
            return !string.IsNullOrEmpty(id) ? new Guid(id) : default(Guid?);
        }
    }
}

using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompanyName.Shared.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CompanyName.Shared.Tests
{
    [TestClass]
    public class TestAuthentication
    {
        [TestMethod]
        public void TestHasPermission()
        {
            JwtSecurityToken token = new JwtSecurityToken(null, null, new[] { new Claim("scope", "read:users delete:users create:users") }, null, null, null);
            Assert.IsTrue(token.HasPermission("read:users"));
            Assert.IsTrue(token.HasPermission("delete:users"));
            Assert.IsTrue(token.HasPermission("create:users"));
            Assert.IsFalse(token.HasPermission("update:users"));
        }
    }
}

using Microsoft.AspNetCore.Http;
using Moq;
using PrimeCollaborationManager.Helpers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Xunit;

namespace PrimeCollaborationManager.Tests.Helpers
{
    public class UserHelperTest
    {
        [Fact]
        public void GetCurrentUser_Returns_User()
        {
            var user = GetFakePrincipal("test", 117);
            var ctx = new Mock<HttpContext>();
            ctx.Setup(h => h.TraceIdentifier).Returns("trace");
            ctx.Setup(h => h.User).Returns(user);
            var rslt = UserHelper.GetCurrentUser(ctx.Object);

            Assert.Equal("test", rslt.UserEmail);
            Assert.Equal(117, rslt.UserID);
            Assert.Equal("trace", rslt.RequestID);
        }

        [Fact]
        public void GetCurrentUser_ThrowsError_NullCtx()
        {
            Assert.Throws<ArgumentException>(() => UserHelper.GetCurrentUser(null));
        }

        [Fact]
        public void GetCurrentUser_GivesDefaults_MissingUser()
        {
            var ctx = new Mock<HttpContext>();
            ctx.Setup(h => h.TraceIdentifier).Returns("trace");
            ctx.Setup(h => h.User).Returns((ClaimsPrincipal)null);
            var rslt = UserHelper.GetCurrentUser(ctx.Object);

            Assert.Equal(UserHelper.EmailNotFound, rslt.UserEmail);
            Assert.Equal(-1, rslt.UserID);
            Assert.Equal("trace", rslt.RequestID);
        }

        [Fact]
        public void GetCurrentUser_GivesDefaultValues_MissingClaims()
        {
            var ctx = new Mock<HttpContext>();
            ctx.Setup(h => h.TraceIdentifier).Returns("trace");
            ctx.Setup(h => h.User).Returns(new ClaimsPrincipal(new GenericIdentity("test")));
            var rslt = UserHelper.GetCurrentUser(ctx.Object);

            Assert.Equal(UserHelper.EmailNotFound, rslt.UserEmail);
            Assert.Equal(-1, rslt.UserID);
            Assert.Equal("trace", rslt.RequestID);
        }

        [Fact]
        public void GetCurrentUser_GivesDefaultValues_EmptyEmail()
        {
            var user = GetFakePrincipal("    ", 117);
            var ctx = new Mock<HttpContext>();
            ctx.Setup(h => h.TraceIdentifier).Returns("trace");
            ctx.Setup(h => h.User).Returns(user);
            var rslt = UserHelper.GetCurrentUser(ctx.Object);

            Assert.Equal(UserHelper.EmailNotFound, rslt.UserEmail);
            Assert.Equal(117, rslt.UserID);
            Assert.Equal("trace", rslt.RequestID);
        }

        private ClaimsPrincipal GetFakePrincipal(string email, int id)
        {
            var ident = new GenericIdentity("test user");
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, email));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, id.ToString()));
            ident.AddClaims(claims);
            return new ClaimsPrincipal(ident);
        }
    }
}

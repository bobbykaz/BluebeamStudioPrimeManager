using PrimeCollaborationManager.Models;
using System.Collections.Generic;
using Xunit;

namespace PrimeCollaborationManager.Tests.Models
{
    public class ErrorViewModelTest
    {
        [Theory]
        [InlineData(false, "")]
        [InlineData(false, null)]
        [InlineData(true, "asdasd")]
        public void ShowRequestIdTest(bool expectedResult, string traceId)
        {
            var em = new ErrorViewModel
            {
                RequestId = traceId,
                StudioError = null
            };

            Assert.Equal(expectedResult, em.ShowRequestId);
        }

        [Fact]
        public void ShowStudioErrorTest()
        {
            var em = new ErrorViewModel
            {
                RequestId = "123",
                StudioError = null
            };

            Assert.False(em.ShowStudioError);
            em.StudioError = new Studio.Api.Client.StudioApiException();
            Assert.True(em.ShowStudioError);
        }
    }
}

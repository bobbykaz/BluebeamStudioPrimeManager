using PrimeCollaborationManager.Models;
using System.Collections.Generic;
using Xunit;
namespace PrimeCollaborationManager.Tests.Models
{
    public class PagedResultTest
    {
        [Fact]
        public void GetModelForDisplayTest()
        {
            var pr = new PagedResult<int>()
            {
                CurrentPage = 7,
                Items = new List<int>(),
                ItemsPerPage = 100,
                TotalItems = 750
            };

            var prd = pr.GetModelForDisplay("test");

            Assert.Equal("test", prd.ControllerAction);
            Assert.Empty(prd.ActionParams);
            Assert.Equal(7, prd.CurrentPage);
            Assert.Equal(100, prd.ItemsPerPage);
            Assert.Equal(750, prd.TotalItems);
        }

    }
}

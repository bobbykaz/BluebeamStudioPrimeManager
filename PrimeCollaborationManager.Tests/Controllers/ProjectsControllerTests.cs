using Xunit;
using System;
using System.Collections.Generic;
using System.Text;
using Studio.Api.Client;
using System.Threading.Tasks;
using PrimeCollaborationManager.Services;
using Moq;
using PrimeCollaborationManager.Models;
using PrimeCollaborationManager.Tests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace PrimeCollaborationManager.Controllers.Tests
{
    public class ProjectsControllerTests
    {
        public const string TestTraceID = "TestID";
        [Fact]
        public async Task IndexTest()
        {
            var mockSvc = new Mock<ICollaborationService>();
            var result = new CollaborationList();
            mockSvc.Setup(s => s.GetListAsync(91)).ReturnsAsync(result).Verifiable();
            var ctrlr = GetTestController(mockSvc.Object);

            var viewResult = await ctrlr.Index(91);

            Assert.True(ctrlr.IsInitialized);
            mockSvc.Verify(s => s.GetListAsync(91));
            Assert.True(viewResult is ViewResult);
            Assert.Equal(result, (viewResult as ViewResult).Model);
        }

        [Fact]
        public async Task Index_DefaultsToPage1()
        {
            var mockSvc = new Mock<ICollaborationService>();
            var result = new CollaborationList();
            mockSvc.Setup(s => s.GetListAsync(1)).ReturnsAsync(result).Verifiable();
            var ctrlr = GetTestController(mockSvc.Object);

            var viewResult = await ctrlr.Index();

            Assert.True(ctrlr.IsInitialized);
            mockSvc.Verify(s => s.GetListAsync(1));
            Assert.True(viewResult is ViewResult);
            Assert.Equal(result, (viewResult as ViewResult).Model);
        }

        [Fact]
        public async Task Index_HandlesErrors()
        {
            var mockSvc = new Mock<ICollaborationService>();
            mockSvc.Setup(s => s.GetListAsync(1)).Throws(DataGenerator.GetStudioApiException("1", "test", null));
            var ctrlr = GetTestController(mockSvc.Object);

            var view = await ctrlr.Index();

            Assert.True(ctrlr.IsInitialized);
            mockSvc.Verify(s => s.GetListAsync(1));
            Assert.True(view is ViewResult);
            Assert.True((view as ViewResult).Model is ErrorViewModel);
            var e = (view as ViewResult).Model as ErrorViewModel;
            Assert.Equal("1", e.StudioError.StudioErrorCode);
            Assert.Equal("test", e.StudioError.StudioErrorMessage);
            Assert.Equal(TestTraceID, e.RequestId);
        }

        public FakeProjectsController GetTestController(ICollaborationService mockSvc)
        {
            var httpCtx = new Mock<HttpContext>();
            httpCtx.Setup(s => s.TraceIdentifier).Returns(TestTraceID);
            return new FakeProjectsController(DataGenerator.GetStudioAppConfig(), mockSvc)
            {
                ControllerContext = new ControllerContext() { HttpContext = httpCtx.Object}
            };
        }
    }

    public class FakeProjectsController : ProjectsController
    {
        public ICollaborationService MockService { get; set; }
        public bool IsInitialized { get; set; }
        public FakeProjectsController(StudioApplicationConfig config, ICollaborationService svc) : base(config)
        {
            MockService = svc;
            IsInitialized = false;
        }

        protected override Task InitClient()
        {
            IsInitialized = true;
            _CollaborationService = MockService;
            return Task.CompletedTask;
        }
    }
}
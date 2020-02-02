using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PrimeCollaborationManager.Models;
using PrimeCollaborationManager.Services;
using PrimeCollaborationManager.Tests;
using Studio.Api.Client;
using System.Threading.Tasks;
using Xunit;

namespace PrimeCollaborationManager.Controllers.Tests
{
    public class ProjectsControllerTests
    {
        public const string TestTraceID = "TestID";
        public const int TestUserID = 117;
        public const string TestProjectID = "472-386-790";
        public const int TestPageNumber = 35;

        public const string TestStudioErrorCode = "-117";
        public const string TestStudioErrorMsg = "This is a test message";

        #region Index
        [Fact]
        public async Task IndexTest()
        {
            var mockSvc = new Mock<ICollaborationService>();
            var result = new CollaborationList();
            mockSvc.Setup(s => s.GetListAsync(TestPageNumber)).ReturnsAsync(result).Verifiable();
            var ctrlr = GetTestController(mockSvc.Object);

            var viewResult = await ctrlr.Index(TestPageNumber);

            Assert.True(ctrlr.IsInitialized);
            mockSvc.Verify(s => s.GetListAsync(TestPageNumber));
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
            mockSvc.Setup(s => s.GetListAsync(1)).Throws(DataGenerator.GetStudioApiException(TestStudioErrorCode, TestStudioErrorMsg, null)).Verifiable();
            var ctrlr = GetTestController(mockSvc.Object);

            var view = await ctrlr.Index();

            Assert.True(ctrlr.IsInitialized);
            mockSvc.Verify(s => s.GetListAsync(1));
            CheckErrors(view);
        }
        #endregion

        #region Details
        [Fact]
        public async Task DetailsTest()
        {
            var mockSvc = new Mock<ICollaborationService>();
            var svcResult = new Collaboration();
            mockSvc.Setup(s => s.GetDetailsAsync(TestProjectID)).ReturnsAsync(svcResult).Verifiable();
            var ctrlr = GetTestController(mockSvc.Object);

            var viewResult = await ctrlr.Details(TestProjectID);

            Assert.True(ctrlr.IsInitialized);
            mockSvc.VerifyAll();
            Assert.True(viewResult is ViewResult);
            Assert.True((viewResult as ViewResult).Model is CollaborationDetails);
            Assert.Equal(svcResult, ((viewResult as ViewResult).Model as CollaborationDetails).Collab);
        }

        [Fact]
        public async Task Details_HandlesErrors()
        {
            var mockSvc = new Mock<ICollaborationService>();
            mockSvc.Setup(s => s.GetDetailsAsync(TestProjectID)).Throws(DataGenerator.GetStudioApiException(TestStudioErrorCode, TestStudioErrorMsg, null)).Verifiable();
            var ctrlr = GetTestController(mockSvc.Object);

            var view = await ctrlr.Details(TestProjectID);

            Assert.True(ctrlr.IsInitialized);
            mockSvc.VerifyAll();
            CheckErrors(view);
        }
        #endregion

        #region PermissionDetails
        #endregion

        #region UpdateAccess
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UpdateAccessTest(bool inputBool)
        {
            var mockSvc = new Mock<ICollaborationService>();
            mockSvc.Setup(s => s.UpdateCollaborationAccessAsync(TestProjectID ,inputBool)).Returns(Task.CompletedTask).Verifiable();
            var ctrlr = GetTestController(mockSvc.Object);

            var viewResult = await ctrlr.UpdateAccess(TestProjectID, inputBool);

            Assert.True(ctrlr.IsInitialized);
            mockSvc.VerifyAll();
            Assert.True(viewResult is RedirectToActionResult);
            var result = viewResult as RedirectToActionResult;
            Assert.Equal("Details", result.ActionName);
            Assert.True(result.RouteValues.ContainsKey("collabId"));
            Assert.Equal(TestProjectID, result.RouteValues["collabId"]);
        }

        [Fact]
        public async Task UpdateAccess_HandlesErrors()
        {
            var mockSvc = new Mock<ICollaborationService>();
            mockSvc.Setup(s => s.UpdateCollaborationAccessAsync(TestProjectID, false))
                    .Throws(DataGenerator.GetStudioApiException(TestStudioErrorCode, TestStudioErrorMsg, null)).Verifiable();

            var ctrlr = GetTestController(mockSvc.Object);

            var viewResult = await ctrlr.UpdateAccess(TestProjectID, false);

            Assert.True(ctrlr.IsInitialized);
            mockSvc.VerifyAll();
            CheckErrors(viewResult);
        }
        #endregion

        #region UserList
        #endregion

        #region UpdateUserPermission
        [Theory]
        [InlineData(true, "Allow")]
        [InlineData(false, "Deny")]
        public async Task UpdateUserPermissionTest(bool inputBool, string expectedPermSetting)
        {
            var mockSvc = new Mock<ICollaborationService>();
            mockSvc.Setup(s => s.UpdateUserRestrictedStatusAsync(TestProjectID, TestUserID, expectedPermSetting)).Returns(Task.CompletedTask).Verifiable();
            var ctrlr = GetTestController(mockSvc.Object);

            var viewResult = await ctrlr.UpdateUserPermission(TestProjectID, TestPageNumber, TestUserID, inputBool);

            Assert.True(ctrlr.IsInitialized);
            mockSvc.VerifyAll();
            Assert.True(viewResult is RedirectToActionResult);
            var result = viewResult as RedirectToActionResult;
            Assert.Equal("UserList", result.ActionName);
            Assert.True(result.RouteValues.ContainsKey("collabId"));
            Assert.Equal(TestProjectID, result.RouteValues["collabId"]);
            Assert.True(result.RouteValues.ContainsKey("page"));
            Assert.Equal(TestPageNumber.ToString(), result.RouteValues["page"]);
        }

        [Fact]
        public async Task UpdateUserPermission_HandlesErrors()
        {
            var mockSvc = new Mock<ICollaborationService>();
            mockSvc.Setup(s => s.UpdateUserRestrictedStatusAsync(TestProjectID, TestUserID, It.IsAny<string>()))
                    .Throws(DataGenerator.GetStudioApiException(TestStudioErrorCode, TestStudioErrorMsg, null)).Verifiable();
            
            var ctrlr = GetTestController(mockSvc.Object);

            var viewResult = await ctrlr.UpdateUserPermission(TestProjectID, TestPageNumber, TestUserID, false);

            Assert.True(ctrlr.IsInitialized);
            mockSvc.VerifyAll();
            CheckErrors(viewResult);
        }
        #endregion

        public FakeProjectsController GetTestController(ICollaborationService mockSvc)
        {
            var httpCtx = new Mock<HttpContext>();
            httpCtx.Setup(s => s.TraceIdentifier).Returns(TestTraceID);
            return new FakeProjectsController(DataGenerator.GetStudioAppConfig(), mockSvc)
            {
                ControllerContext = new ControllerContext() { HttpContext = httpCtx.Object}
            };
        }

        private void CheckErrors(IActionResult result)
        {
            Assert.True(result is ViewResult);
            Assert.True((result as ViewResult).Model is ErrorViewModel);
            var e = (result as ViewResult).Model as ErrorViewModel;
            Assert.Equal(TestStudioErrorCode, e.StudioError.StudioErrorCode);
            Assert.Equal(TestStudioErrorMsg, e.StudioError.StudioErrorMessage);
            Assert.Equal(TestTraceID, e.RequestId);
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
            CollaborationService = MockService;
            return Task.CompletedTask;
        }
    }
}
using System;
using System.Threading.Tasks;
using Acme.IdentityServer.WebApi.Assemblers;
using Acme.IdentityServer.WebApi.Controllers;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Exceptions;
using Acme.IdentityServer.WebApi.Models.Input;
using Acme.IdentityServer.WebApi.Models.Output;
using Acme.IdentityServer.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests.Controllers {
    public class UserControllerTest : BaseTestFixture {

        private readonly UserController target;
        private readonly Mock<ILogger<UserController>> loggerMock;
        private readonly Mock<IUserService> userServiceMock;
        private readonly Mock<IUserModelAssembler> userModelAssemblerMock;
        private readonly Mock<IClientService> clientServiceMock;

        public UserControllerTest() {
            loggerMock = new Mock<ILogger<UserController>>();
            userServiceMock = new Mock<IUserService>();
            userModelAssemblerMock = new Mock<IUserModelAssembler>();
            clientServiceMock = new Mock<IClientService>();
            target = new UserController(loggerMock.Object, userServiceMock.Object, userModelAssemblerMock.Object, clientServiceMock.Object) {
                ControllerContext = new ControllerContext()
            };
        }

        [Fact]
        public void ShouldUpdatePassword() {
            // arrange
            Guid subjectId = Guid.NewGuid();
            UpdatePasswordModel model = new UpdatePasswordModel() {
                Password = "myawesomenewpassword"
            };
            userServiceMock.Setup(s => s.UpdatePassword(subjectId, model));

            // act
            var result = target.UpdatePassword(subjectId, model);

            // assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void ShouldNotUpdatePasswordWithInvalidPasswordModel() {
            // arrange
            Guid subjectId = Guid.NewGuid();
            UpdatePasswordModel model = new UpdatePasswordModel();
            target.ModelState.AddModelError("test", "test"); // stimulate invalid model state

            // act
            var result = target.UpdatePassword(subjectId, model);

            // assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void ShouldNotUpdatePasswordForUnfoundUser() {
            // arrange
            Guid subjectId = Guid.NewGuid();
            UpdatePasswordModel model = new UpdatePasswordModel();
            userServiceMock.Setup(s => s.UpdatePassword(subjectId, model)).Throws(new ResourceNotFoundMessage());

            // act
            var result = target.UpdatePassword(subjectId, model);

            // assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ShouldDeactivateUser() {
            // arrange
            Guid subjectId = Guid.NewGuid();
            userServiceMock.Setup(s => s.DeactivateUser(subjectId));

            // act
            var result = target.DeactivateUser(subjectId);

            // assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void ShouldNotDeactivateUserForUnfoundUser() {
            // arrange
            Guid subjectId = Guid.NewGuid();
            userServiceMock.Setup(s => s.DeactivateUser(subjectId)).Throws(new ResourceNotFoundMessage());

            // act
            var result = target.DeactivateUser(subjectId);

            // assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ShouldUpdateUser() {
            // arrange
            Guid subjectId = Guid.NewGuid();
            UpdateUserModel model = new UpdateUserModel() {
                Username = "username"
            };
            UserOutputModel output = new UserOutputModel();
            User updatedUser = new User();
            userServiceMock.Setup(s => s.UpdateUser(subjectId, model)).ReturnsAsync(updatedUser);
            userModelAssemblerMock.Setup(a => a.ToUserOutputModel(updatedUser)).Returns(output);
            target.ControllerContext.HttpContext = new DefaultHttpContext();
            target.ControllerContext.HttpContext.Request.Scheme = "https";
            target.ControllerContext.HttpContext.Request.Host = new HostString("unittest.local");

            // act
            var result = await target.UpdateUser(subjectId, model) as OkObjectResult;

            // assert
            Assert.Same(output, result.Value);
            Assert.Equal($"https://unittest.local/api/users/{subjectId:d}", target.ControllerContext.HttpContext.Response.Headers["Location"]);
        }

        [Fact]
        public async Task ShouldNotUpdateUserOfUnfoundUser() {
            // arrange
            Guid subjectId = Guid.NewGuid();
            UpdateUserModel model = new UpdateUserModel() {
                Username = "username"
            };
            userServiceMock.Setup(s => s.UpdateUser(subjectId, model)).Throws(new ResourceNotFoundMessage());

            // act
            var result = await target.UpdateUser(subjectId, model);

            // assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ShouldAddUser() {
            // arrange
            var model = new CreateUserModel() {
                Username = "username",
                Password = "password"
            };
            User created = new User();
            UserOutputModel output = new UserOutputModel();
            userServiceMock.Setup(s => s.CreateUser(model)).ReturnsAsync(created);
            userModelAssemblerMock.Setup(s => s.ToUserOutputModel(created)).Returns(output);

            // act
            var result = await target.AddUser(model) as CreatedAtRouteResult;

            // assert
            Assert.Same(output, result.Value);
        }

        [Fact]
        public void ShouldLockUser() {
            // arrange
            UserOutputModel output = new UserOutputModel();
            Guid subjectId = Guid.NewGuid();
            UpdateLockModel model = new UpdateLockModel() {
                IsLocked = true
            };
            target.ControllerContext.HttpContext = new DefaultHttpContext();
            target.ControllerContext.HttpContext.Request.Scheme = "https";
            target.ControllerContext.HttpContext.Request.Host = new HostString("unittest.local");
            userServiceMock.Setup(s => s.UpdateUserLock(subjectId, model));
            userModelAssemblerMock.Setup(a => a.ToUserOutputModel(It.IsAny<User>())).Returns(output);


            // act
            var result = target.UpdateUserLock(subjectId, model) as OkObjectResult;

            // assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Same(output, result.Value);
            Assert.Equal($"https://unittest.local/api/users/{subjectId:d}", target.ControllerContext.HttpContext.Response.Headers["Location"]);

        }

        [Fact]
        public void ShouldNotLockUserWithInvalidLockModel() {
            // arrange
            Guid subjectId = Guid.NewGuid();
            UpdateLockModel model = new UpdateLockModel();
            target.ModelState.AddModelError("test", "test"); // stimulating invalid model state

            // act
            var result = target.UpdateUserLock(subjectId, model);

            // assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void ShouldNotLockUnfoundUser() {
            // arrange
            Guid subjectId = Guid.NewGuid();
            UpdateLockModel model = new UpdateLockModel();
            userServiceMock.Setup(s => s.UpdateUserLock(subjectId, model)).Throws(new ResourceNotFoundMessage());
            // act
            var result = target.UpdateUserLock(subjectId, model);

            // assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ShouldGetUserAsync() {
            // arrange
            var userId = Guid.NewGuid();
            User created = new User();
            UserOutputModel output = new UserOutputModel();
            output.UserId = userId;
            userServiceMock.Setup(s => s.FindBySubjectIdAsync(It.IsAny<Guid>())).ReturnsAsync(created);
            userModelAssemblerMock.Setup(s => s.ToUserOutputModel(created)).Returns(output);

            // act
            var result = await target.GetUserAsync(userId) as OkObjectResult;
            UserOutputModel resultUser = result.Value as UserOutputModel;
            Assert.Equal(resultUser.UserId, userId);
        }

        [Fact]
        public async Task ShouldFailToGetUserAsync() {
            // arrange
            var userId = Guid.NewGuid();
            userServiceMock.Setup(s => s.FindBySubjectIdAsync(userId)).ReturnsAsync(null as User);

            // act
            var result = await target.GetUserAsync(userId) as NotFoundResult;

            // assert
            Assert.Equal(new NotFoundResult().StatusCode, result.StatusCode);
        }
    }
}

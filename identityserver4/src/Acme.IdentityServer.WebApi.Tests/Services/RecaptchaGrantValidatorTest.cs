using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acme.IdentityServer.WebApi.Services.ExtensionGrantValidators;
using IdentityServer4.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests.Services {
    public class RecaptchaGrantValidatorTest {
        private readonly RecaptchaGrantValidator validator;
        private readonly GResponseModel response;
        private readonly ValidatedTokenRequest request;
        private readonly Mock<IGoogleRecaptchaV3Service> service;

        public RecaptchaGrantValidatorTest() {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>() { { "GoogleRecaptchaV3:ApiUrl", "http://somehost.com" } })
                .Build();
            var raw = new System.Collections.Specialized.NameValueCollection()
            {
                { "site_secret", "secret" },
                { "recaptcha_token", "token" },
                { "remote_ip", "123.123.123.123" },
                { "client", "test-client" },
                { "version", "1.0" }
            };
            request = new ValidatedTokenRequest() { Raw = raw };
            service = new Mock<IGoogleRecaptchaV3Service>();
            service.Setup(x => x.Execute()).Returns(Task.FromResult(true));
            response = new GResponseModel() { action = "action", hostname = "hostname", score = 0.5M };
            service.Setup(x => x.Response).Returns(response);
            validator = new RecaptchaGrantValidator(new NullLogger<RecaptchaGrantValidator>(), service.Object, configuration);
        }

        [Fact]
        public async Task ShouldCreateGrantForValidToken() {
            // arrange
            var context = new ExtensionGrantValidationContext() { Request = request };

            // act
            await validator.ValidateAsync(context);

            // assert
            Assert.Null(context.Result.Error);
            Assert.NotNull(context.Result.Subject);
            Assert.NotEmpty(context.Result.Subject.Claims);
            Assert.Equal("00000000-0000-0000-0000-000000000001", context.Result.Subject.Claims.First(x => x.Type == "sub").Value);
            Assert.Equal(response.action, context.Result.Subject.Claims.First(x => x.Type == "action").Value);
            Assert.Equal(response.hostname, context.Result.Subject.Claims.First(x => x.Type == "hostname").Value);
            Assert.Equal(response.score.ToString(), context.Result.Subject.Claims.First(x => x.Type == "score").Value);
            Assert.Null(context.Result.Subject.Claims.FirstOrDefault(x => x.Type == "resource_id"));
        }

        [Fact]
        public async Task ShouldCreateGrantForValidTokenWithReferenceId() {
            // arrange
            request.Raw.Add("resource_id", "foo");
            var context = new ExtensionGrantValidationContext() { Request = request };

            // act
            await validator.ValidateAsync(context);

            // assert
            Assert.Null(context.Result.Error);
            Assert.NotNull(context.Result.Subject);
            Assert.NotEmpty(context.Result.Subject.Claims);
            Assert.Equal("00000000-0000-0000-0000-000000000001", context.Result.Subject.Claims.First(x => x.Type == "sub").Value);
            Assert.Equal(response.action, context.Result.Subject.Claims.First(x => x.Type == "action").Value);
            Assert.Equal(response.hostname, context.Result.Subject.Claims.First(x => x.Type == "hostname").Value);
            Assert.Equal(response.score.ToString(), context.Result.Subject.Claims.First(x => x.Type == "score").Value);
            Assert.Equal("foo", context.Result.Subject.Claims.First(x => x.Type == "resource_id").Value);
        }

        [Fact]
        public async Task ShouldFailToCreateGrantForInValidToken() {
            // arrange
            var context = new ExtensionGrantValidationContext() { Request = request };
            response.error_codes = new string[] { "invalid-input-response" };
            service.Setup(x => x.Execute()).Returns(Task.FromResult(false));

            // act
            await validator.ValidateAsync(context);

            // assert
            Assert.Equal("invalid_grant", context.Result.Error);
        }

        [Theory]
        [InlineData("site_secret")]
        [InlineData("recaptcha_token")]
        public async Task ShouldFailToCreateGrantForMissingValue(string key) {
            // arrange
            request.Raw.Remove(key);
            var context = new ExtensionGrantValidationContext() { Request = request };

            // act
            await validator.ValidateAsync(context);

            // assert
            Assert.Equal("invalid_grant", context.Result.Error);
        }

        [Fact]
        public async Task ShouldCreateGrantWithClientClaimWhenClientIsPresent() {
            // Arrange
            request.Raw.Add("client", "test-client");
            var context = new ExtensionGrantValidationContext() { Request = request };

            // Act
            await validator.ValidateAsync(context);

            // Assert
            Assert.Null(context.Result.Error);
            Assert.NotNull(context.Result.Subject);
            Assert.NotEmpty(context.Result.Subject.Claims);
            Assert.Contains(context.Result.Subject.Claims, c => c.Type == "client");
        }

        [Fact]
        public async Task ShouldCreateGrantWithVersionClaimWhenVersionIsPresent() {
            // Arrange
            request.Raw.Add("version", "1.0");
            var context = new ExtensionGrantValidationContext() { Request = request };

            // Act
            await validator.ValidateAsync(context);

            // Assert
            Assert.Null(context.Result.Error);
            Assert.NotNull(context.Result.Subject);
            Assert.NotEmpty(context.Result.Subject.Claims);
            Assert.Contains(context.Result.Subject.Claims, c => c.Type == "version");
        }
    }
}

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acme.DomainEvent.Events;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Models.Input;
using Acme.IdentityServer.WebApi.Services;
using Cortside.Common.Messages.MessageExceptions;
using Cortside.DomainEvent.EntityFramework;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static IdentityServer4.IdentityServerConstants;

namespace Acme.IdentityServer.WebApi.Tests.Services {

    public class ClientSecretServiceTest : BaseClientTest {
        private ClientSecretService sut;

        private Mock<IServiceProvider> mockServiceProvider;
        private Mock<ILogger<ClientSecretService>> mockLogger;
        private Mock<IDomainEventOutboxPublisher> mockPublisher;
        private Mock<IConfiguration> mockConfig;
        private IHashProvider hashProvider;

        public ClientSecretServiceTest() {
            mockServiceProvider = new Mock<IServiceProvider>();
            Mock<IServiceScopeFactory> scopeFactory = new Mock<IServiceScopeFactory>();
            mockServiceProvider.Setup(s => s.GetService(typeof(IServiceScopeFactory))).Returns(scopeFactory.Object);
            Mock<IServiceScope> scope = new Mock<IServiceScope>();
            scopeFactory.Setup(s => s.CreateScope()).Returns(scope.Object);
            Mock<IServiceProvider> scopeServiceProvider = new Mock<IServiceProvider>();
            scope.Setup(s => s.ServiceProvider).Returns(scopeServiceProvider.Object);
            scopeServiceProvider.Setup(s => s.GetService(typeof(IIdentityServerDbContext))).Returns(IdentityServerDbContext);
            scopeServiceProvider.Setup(s => s.GetService(typeof(IdentityServerDbContext))).Returns(IdentityServerDbContext);
            mockLogger = new Mock<ILogger<ClientSecretService>>();
            mockPublisher = new Mock<IDomainEventOutboxPublisher>();
            scopeServiceProvider.Setup(s => s.GetService(typeof(IDomainEventOutboxPublisher))).Returns(mockPublisher.Object);

            hashProvider = new HashProvider();

            mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["traefikHostname"]).Returns("https://www.identityserver.Acme.com");
            mockConfig.Setup(x => x["ClientSecretRequest:VerificationExpirationInMinutes"]).Returns("30");
            mockConfig.Setup(x => x["ClientSecretRequest:ExpirationInHours"]).Returns("24");
            mockConfig.Setup(x => x["ClientSecretRequest:VerificationAttempts"]).Returns("3");

            sut = new ClientSecretService(mockServiceProvider.Object, mockLogger.Object, mockConfig.Object, hashProvider);
        }

        [Fact]
        public void ShouldResetClientSecret() {
            // Arrange
            var clientId = InsertTestClientIntoDbContext("clientId", Guid.NewGuid());
            InsertTestClientSecretRequestsIntoDbContext(clientId);

            // Act
            var client = sut.ResetSecret(clientId);

            // Assert
            client.Result.Should().NotBeNull();
            client.Result.ClientSecret.Value.Should().Be("invalid-hash=");
        }

        [Fact]
        public async Task ShouldNotResetClientSecretIfClientOrSecretDoesntExist() {
            // Arrange
            var clientId = 1;

            // Act invalid client
            await Assert.ThrowsAsync<BadRequestResponseException>(() => sut.ResetSecret(clientId));

            // Act invalid client secret
            InsertTestClientIntoDbContext("clientId1", Guid.NewGuid(), false);
            await Assert.ThrowsAsync<BadRequestResponseException>(() => sut.ResetSecret(clientId));
        }

        [Fact]
        public async Task ShouldPublishResetClientSecretNotificationEvent() {
            // Arrange
            var clientId = 1;
            var subClaimId = Guid.NewGuid();

            InsertTestClientIntoDbContext(clientId.ToString(), subClaimId);
            InsertTestClientSecretRequestsIntoDbContext(clientId);

            // Act
            await sut.SendClientSecretEmail(clientId);

            // Assert
            mockPublisher.Verify(x => x.PublishAsync(It.IsAny<ResetClientSecretNotificationEvent>()), Times.Once);
        }

        [Fact]
        public async Task SendVerificationCode_ShouldErrorIfRequestDoesntExist() {
            // Arrange
            var clientId = 1;
            var subClaimId = Guid.NewGuid();
            var clientSecretRequestId = Guid.NewGuid();
            var request = new SendVerificationCodeModel() {
                TokenHash = "testhash"
            };

            InsertTestClientIntoDbContext(clientId.ToString(), subClaimId);
            InsertTestClientSecretRequestsIntoDbContext(clientId);

            // Act
            var exception = await Assert.ThrowsAsync<BadRequestResponseException>(() => sut.SendVerificationCode(clientSecretRequestId, request));

            // Assert
            Assert.Equal($"Cannot send verification code for ClientSecretRequestId: {clientSecretRequestId} because the client secret request doesn't exist.", exception.Message);
        }

        [Fact]
        public async Task SendVerificationCode_ShouldErrorIfRequestIsExpired() {
            // Arrange
            var clientId = 1;
            var subClaimId = Guid.NewGuid();
            var clientSecretRequestId = Guid.NewGuid();
            var request = new SendVerificationCodeModel() {
                TokenHash = "testhash"
            };

            InsertTestClientIntoDbContext(clientId.ToString(), subClaimId);
            InsertTestClientSecretRequestsIntoDbContext(clientId, clientSecretRequestId, Guid.NewGuid(), DateTime.Now.AddHours(-1));

            // Act
            var exception = await Assert.ThrowsAsync<BadRequestResponseException>(() => sut.SendVerificationCode(clientSecretRequestId, request));

            // Assert
            Assert.Equal($"ClientId: {clientId} cannot send verification code because secret request is expired.", exception.Message);
        }

        [Fact]
        public async Task SendVerificationCode_ShouldErrorIfTokenDoesntMatch() {
            // Arrange
            var clientId = 1;
            var subClaimId = Guid.NewGuid();
            var clientSecretRequestId = Guid.NewGuid();
            var token = Guid.NewGuid();

            var request = new SendVerificationCodeModel() {
                TokenHash = "testhash"
            };

            InsertTestClientIntoDbContext(clientId.ToString(), subClaimId);
            InsertTestClientSecretRequestsIntoDbContext(clientId, clientSecretRequestId, token, DateTime.Now.AddDays(1));

            // Act
            var exception = await Assert.ThrowsAsync<BadRequestResponseException>(() => sut.SendVerificationCode(clientSecretRequestId, request));

            // Assert
            Assert.Equal($"ClientId: {clientId} cannot send verification code because token hash is invalid.", exception.Message);
        }

        [Fact]
        public async Task SendVerificationCode_ShouldErrorIfNoPhoneNumberClaim() {
            // Arrange
            var clientId = 1;
            var subClaimId = Guid.NewGuid();
            var clientSecretRequestId = Guid.NewGuid();
            var token = Guid.NewGuid();

            var tokenHash = hashProvider.ComputeHash256(token.ToString());
            var request = new SendVerificationCodeModel() {
                TokenHash = tokenHash
            };

            InsertTestClientIntoDbContext(clientId.ToString(), subClaimId, true, false);
            InsertTestClientSecretRequestsIntoDbContext(clientId, clientSecretRequestId, token, DateTime.Now.AddDays(1));

            // Act
            var exception = await Assert.ThrowsAsync<BadRequestResponseException>(() => sut.SendVerificationCode(clientSecretRequestId, request));

            // Assert
            Assert.Equal($"ClientId: {clientId} cannot send verification code because a phone number claim doesn't exist.", exception.Message);
        }

        [Fact]
        public async Task SendVerificationCode_ShouldSendSuccessfully() {
            // Arrange
            var clientId = 1;
            var subClaimId = Guid.NewGuid();
            var clientSecretRequestId = Guid.NewGuid();
            var token = Guid.NewGuid();

            var tokenHash = hashProvider.ComputeHash256(token.ToString());
            var request = new SendVerificationCodeModel() {
                TokenHash = tokenHash
            };

            InsertTestClientIntoDbContext(clientId.ToString(), subClaimId);
            InsertTestClientSecretRequestsIntoDbContext(clientId, clientSecretRequestId, token, DateTime.Now.AddDays(1));
            InsertTestClientClaimsIntoDbContext(clientId, "phone_number", "1234567890");

            // Act
            await sut.SendVerificationCode(clientSecretRequestId, request);

            // Assert
            mockPublisher.Verify(x => x.PublishAsync(It.IsAny<SmsRequestEvent>()), Times.Once);
        }

        [Fact]
        public void EncodeClientSecretRequest_ShouldEncode() {
            // Arrange
            var requestId = Guid.NewGuid();
            var token = Guid.NewGuid();

            // Act
            var result = sut.EncodeClientSecretRequest(requestId, token);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(result));
        }

        [Fact]
        public void EncodeAndDecode_ShouldWork() {
            // Arrange
            var requestId = Guid.NewGuid();
            var token = Guid.NewGuid();

            // Act
            var encodedResult = sut.EncodeClientSecretRequest(requestId, token);
            var decodedResult = sut.DecodeClientSecretRequest(encodedResult);

            var tokenCompare = hashProvider.ComputeHash256(token.ToString());

            // Assert
            Assert.Equal(requestId, decodedResult.RequestId);
            Assert.Equal(tokenCompare, decodedResult.TokenHash);
        }

        [Fact]
        public void ValidateClientSecretRequest_ShouldBeValid() {
            // Arrange
            var requestId = Guid.NewGuid();
            var token = Guid.NewGuid();
            var clientId = 1;
            var subClaimId = Guid.NewGuid();

            var encodedResult = sut.EncodeClientSecretRequest(requestId, token);

            InsertTestClientIntoDbContext(clientId.ToString(), subClaimId);
            InsertTestClientSecretRequestsIntoDbContext(clientId, requestId, token, DateTime.Now.AddDays(1));
            InsertTestClientClaimsIntoDbContext(clientId, "phone_number", "1234567890");

            // Act
            var result = sut.ValidateClientSecretRequest(encodedResult);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void GetClientSecretKey_ShouldReturnKey() {
            // Arrange
            var requestId = Guid.NewGuid();
            var token = Guid.NewGuid();
            var clientId = 1;
            var secret = "eagweahweagwe";

            InsertTestClientSecretRequestsIntoDbContext(clientId, requestId, token, DateTime.Now.AddDays(1));
            InsertTestClientSecretIntoDbContext(clientId, ParsedSecretTypes.SharedSecret, secret);

            // Act
            var result = sut.GetClientSecretKey(requestId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetClientSecretKey_ShouldAddClientSecretAndReturnKey() {
            // Arrange
            var requestId = Guid.NewGuid();
            var token = Guid.NewGuid();
            var clientId = 1;

            InsertTestClientSecretRequestsIntoDbContext(clientId, requestId, token, DateTime.Now.AddDays(1));

            // Act
            var result = sut.GetClientSecretKey(requestId);

            // Assert
            Assert.NotNull(result);
            IdentityServerDbContext.ClientSecrets.Where(x => x.ClientId == clientId).Count().Should().Be(1);
        }

        [Fact]
        public void GetClientPhoneNumberFromSecretRequestId_ShouldReturnPhoneNumber() {
            // Arrange
            var clientId = 1;
            var requestId = Guid.NewGuid();
            var token = Guid.NewGuid();
            var phoneNumber = "1234567890";

            InsertTestClientSecretRequestsIntoDbContext(clientId, requestId, token, DateTime.Now.AddDays(1));
            InsertTestClientClaimsIntoDbContext(clientId, "phone_number", phoneNumber);

            // Act
            var result = sut.GetClientPhoneNumberFromSecretRequestId(requestId);

            // Assert
            Assert.Equal(phoneNumber, result);
        }

        [Fact]
        public void IsVerificationCodeValid_ShouldReturnValid() {
            // Arrange
            var clientId = 1;
            var requestId = Guid.NewGuid();
            var token = Guid.NewGuid();
            var verificationCode = "123456";

            InsertTestClientSecretRequestsIntoDbContext(clientId, requestId, token, DateTime.Now.AddDays(1), 3, verificationCode);

            // Act
            var result = sut.IsVerificationCodeValid(requestId, verificationCode);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void IsVerificationCodeValid_ShouldReturnInvalid_TooManyAttempts() {
            // Arrange
            var clientId = 1;
            var requestId = Guid.NewGuid();
            var token = Guid.NewGuid();
            var verificationCode = "123456";

            InsertTestClientSecretRequestsIntoDbContext(clientId, requestId, token, DateTime.Now.AddDays(1), 0, verificationCode);

            // Act
            var result = sut.IsVerificationCodeValid(requestId, verificationCode);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("Too many attempts.", result.Reason);
        }

        [Fact]
        public void IsVerificationCodeValid_ShouldReturnInvalid_IncorrectVerificationCode() {
            // Arrange
            var clientId = 1;
            var requestId = Guid.NewGuid();
            var token = Guid.NewGuid();
            var verificationCode = "123456";

            InsertTestClientSecretRequestsIntoDbContext(clientId, requestId, token, DateTime.Now.AddDays(1), 3, "654321");

            // Act
            var result = sut.IsVerificationCodeValid(requestId, verificationCode);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("Incorrect verification code.", result.Reason);
        }

        [Fact]
        public void IsVerificationCodeValid_ShouldReturnInvalid_CodeIsExpired() {
            // Arrange
            var clientId = 1;
            var requestId = Guid.NewGuid();
            var token = Guid.NewGuid();
            var verificationCode = "123456";

            InsertTestClientSecretRequestsIntoDbContext(clientId, requestId, token, DateTime.Now.AddDays(-1), 3, verificationCode);

            // Act
            var result = sut.IsVerificationCodeValid(requestId, verificationCode);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("Code is expired.", result.Reason);
        }

        private string GetDefaultHtmlTemplate() {
            var sb = new StringBuilder();
            sb.AppendLine("<head>");
            sb.AppendLine(" <title>Test Html</title>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine(" <div>{{token}}</div>");
            sb.AppendLine(" <div>{{token2}}</div>");
            sb.AppendLine("</body>");

            return sb.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Security.Claims;
using Cortside.AspNetCore.Auditable;
using Cortside.Common.Security;
using Acme.IdentityServer.WebApi.Controllers.Client;
using Acme.IdentityServer.WebApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace Acme.IdentityServer.WebApi.Tests.Services {
    public abstract class BaseClientTest {
        protected IIdentityServerDbContext IdentityServerDbContext;
        protected Mock<ISubjectPrincipal> mockSubjectPrincipal;
        protected DbContextOptionsBuilder<IdentityServerDbContext> optionsBuilder;
        protected Mock<IHttpContextAccessor> httpContextAccessorMock;

        public BaseClientTest() {
            mockSubjectPrincipal = new Mock<ISubjectPrincipal>();
            httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            optionsBuilder = new DbContextOptionsBuilder<IdentityServerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            IdentityServerDbContext = new IdentityServerDbContext(optionsBuilder.Options, new SubjectPrincipal(new List<Claim>() { new Claim("sub", Guid.NewGuid().ToString()) }), httpContextAccessorMock.Object, new DefaultSubjectFactory());
        }

        protected int InsertTestClientIntoDbContext(string clientId, Guid subClaimId, bool include_secret = true, bool include_phone = true) {
            var client = new Client() {
                ClientId = clientId
            };

            client.ClientGrantType = new ClientGrantType() {
                GrantType = ClientConstants.GrantTypes.Implicit.ToString()
            };

            client.ClientPostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>()
            {
                new ClientPostLogoutRedirectUri() {
                    PostLogoutRedirectUri = "http://localhost:4001/logout"
                }
            };

            client.ClientRedirectUris = new List<ClientRedirectUri>()
            {
                new ClientRedirectUri()
                {
                    RedirectUri = "http://localhost"
                }
            };

            client.ClientCorsOrigins = new List<ClientCorsOrigin>() {
                new ClientCorsOrigin()
                {
                    Origin = "http://localhost"
                }
            };

            client.ClientScopes = new List<ClientScope>() {
                new ClientScope()
                {
                    Scope = "test"
                }
            };

            client.ClientClaims = new List<ClientClaim> {
                new ClientClaim {
                    Type = "sub",
                    Value = subClaimId.ToString()
                },
                new ClientClaim {
                    Type = "email",
                    Value = "testemail@email.com"
                }
            };

            if (include_phone) {
                client.ClientClaims.Add(new ClientClaim {
                    Type = "phone_number",
                    Value = "777777777"
                });
            }

            if (include_secret) {
                client.ClientSecret = new ClientSecret {
                    ClientId = client.Id,
                    Value = "valid-hash"
                };
            }

            IdentityServerDbContext.Clients.Add(client);
            IdentityServerDbContext.SaveChanges();

            return client.Id;
        }

        protected int InsertTestClientSecretRequestsIntoDbContext(int clientId) {
            var date = DateTime.Now;

            var request = new ClientSecretRequest() {
                ClientId = clientId,
                ClientSecretRequestId = new Guid(),
                CreateDate = date,
                LastModifiedDate = date,
                Token = new Guid(),
                RequestExpirationDate = date.AddDays(1)
            };

            IdentityServerDbContext.ClientSecretRequests.Add(request);
            IdentityServerDbContext.SaveChanges();

            return clientId;
        }

        protected int InsertTestClientSecretRequestsIntoDbContext(int clientId, Guid clientSecretRequestId, Guid token, DateTime expirationDate) {
            var date = DateTime.Now;

            var request = new ClientSecretRequest() {
                ClientId = clientId,
                ClientSecretRequestId = clientSecretRequestId,
                CreateDate = date,
                LastModifiedDate = date,
                Token = token,
                RequestExpirationDate = expirationDate,
                RemainingSmsVerificationAttempts = 3
            };

            IdentityServerDbContext.ClientSecretRequests.Add(request);
            IdentityServerDbContext.SaveChanges();

            return clientId;
        }

        protected int InsertTestClientSecretRequestsIntoDbContext(int clientId, Guid clientSecretRequestId, Guid token, DateTime expirationDate, int remainingAttempts, string verificationCode) {
            var date = DateTime.Now;

            var request = new ClientSecretRequest() {
                ClientId = clientId,
                ClientSecretRequestId = clientSecretRequestId,
                CreateDate = date,
                LastModifiedDate = date,
                Token = token,
                RequestExpirationDate = expirationDate,
                RemainingSmsVerificationAttempts = remainingAttempts,
                SmsVerificationCode = verificationCode,
                SmsVerificationExpiration = expirationDate
            };

            IdentityServerDbContext.ClientSecretRequests.Add(request);
            IdentityServerDbContext.SaveChanges();

            return clientId;
        }

        protected int InsertTestClientClaimsIntoDbContext(int clientId, string type, string value) {
            var clientClaim = new ClientClaim() {
                ClientId = clientId,
                Type = type,
                Value = value
            };

            IdentityServerDbContext.ClientClaims.Add(clientClaim);
            IdentityServerDbContext.SaveChanges();

            return clientId;
        }

        protected int InsertTestClientSecretIntoDbContext(int clientId, string type, string value) {
            var clientSecret = new ClientSecret() {
                ClientId = clientId,
                Type = type,
                Value = value
            };

            IdentityServerDbContext.ClientSecrets.Add(clientSecret);
            IdentityServerDbContext.SaveChanges();

            return clientId;
        }
    }
}

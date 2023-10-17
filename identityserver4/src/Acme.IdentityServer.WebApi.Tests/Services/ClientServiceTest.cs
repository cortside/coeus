using System;
using System.Collections.Generic;
using System.Linq;
using Cortside.AspNetCore.Auditable;
using Cortside.Common.Messages.MessageExceptions;
using Acme.IdentityServer.WebApi.Controllers.Client;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Helpers;
using Acme.IdentityServer.WebApi.Models;
using Acme.IdentityServer.WebApi.Models.Input;
using Acme.IdentityServer.WebApi.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests.Services {
    public class ClientServiceTest : BaseClientTest {
        private ClientService clientsService;
        private IPhoneNumberHelper phoneNumberHelper;

        private Mock<IServiceProvider> mockServiceProvider;
        private Mock<ILogger<ClientService>> mockLogger;
        private Mock<IHashProvider> mockHashProvider;
        private Mock<IClientSecretService> mockClientSecretService;

        public ClientServiceTest() {
            mockServiceProvider = new Mock<IServiceProvider>();
            Mock<IServiceScopeFactory> scopeFactory = new Mock<IServiceScopeFactory>();
            mockServiceProvider.Setup(s => s.GetService(typeof(IServiceScopeFactory))).Returns(scopeFactory.Object);
            Mock<IServiceScope> scope = new Mock<IServiceScope>();
            scopeFactory.Setup(s => s.CreateScope()).Returns(scope.Object);
            Mock<IServiceProvider> scopeServiceProvider = new Mock<IServiceProvider>();
            scope.Setup(s => s.ServiceProvider).Returns(scopeServiceProvider.Object);
            scopeServiceProvider.Setup(s => s.GetService(typeof(IIdentityServerDbContext))).Returns(IdentityServerDbContext);
            scopeServiceProvider.Setup(s => s.GetService(typeof(IdentityServerDbContext))).Returns(IdentityServerDbContext);
            mockLogger = new Mock<ILogger<ClientService>>();
            mockHashProvider = new Mock<IHashProvider>();
            mockClientSecretService = new Mock<IClientSecretService>();
            phoneNumberHelper = new PhoneNumberHelper();

            clientsService = new ClientService(mockServiceProvider.Object, mockLogger.Object, mockHashProvider.Object, mockClientSecretService.Object, phoneNumberHelper);
        }

        [Fact]
        [Trait("TestRail", "C296324")]
        public void UpdateClient_ShouldInsertNewClientIntoDatabase_WithSettingsBasedOnRequestObject() {

            var clientId = "testID";
            var accessTokenType = 0;
            var clientName = "testName";
            var enableLocalLogin = true;
            var grantType = ClientConstants.GrantTypes.Implicit.ToString();
            var postLogoutRedirectUris = new string[2] {
                "http://localhost:4001/logout",
                "http://localhost:5002/logout"
            };
            var redirectUris = new string[2] {
                "http://localhost:4001/login",
                "http://localhost:5002/login"
            };
            var corsOrigins = new string[2] {
                "http://localhost:4001",
                "http://localhost:5002"
            };
            var scopes = new string[2] {
                "openid",
                "profile"
            };

            var updateClientRequest = new UpdateClientRequest() {
                AccessTokenType = accessTokenType,
                ClientName = clientName,
                EnableLocalLogin = enableLocalLogin,
                GrantType = grantType,
                PostLogoutRedirectUris = postLogoutRedirectUris,
                RedirectUris = redirectUris,
                CorsOrigins = corsOrigins,
                Scopes = scopes
            };

            var newClient = clientsService.UpdateClient(clientId, updateClientRequest);

            Assert.Equal(accessTokenType, newClient.AccessTokenType);
            Assert.Equal(clientName, newClient.ClientName);
            Assert.Equal(enableLocalLogin, newClient.EnableLocalLogin);
            Assert.Equal(grantType, newClient.ClientGrantType.GrantType);
            Assert.Equal(postLogoutRedirectUris, newClient.ClientPostLogoutRedirectUris.Select(x => x.PostLogoutRedirectUri));
            Assert.Equal(redirectUris, newClient.ClientRedirectUris.Select(x => x.RedirectUri));
            Assert.Equal(corsOrigins, newClient.ClientCorsOrigins.Select(x => x.Origin));
            Assert.Equal(scopes, newClient.ClientScopes.Select(x => x.Scope));
        }

        [Fact]
        [Trait("TestRail", "C296324")]
        public void UpdateClient_ShouldUpdateClient_WithSettingsBasedOnRequestObject() {
            var clientId = "testId";
            var clientIdPrimaryKey = InsertTestClientIntoDbContext(clientId, Guid.NewGuid());
            var newClient = IdentityServerDbContext.Clients.Where(x => x.ClientId == clientId).FirstOrDefault();

            var updateClientRequest = new UpdateClientRequest();

            updateClientRequest.ClientName = "Altered";
            updateClientRequest.PostLogoutRedirectUris = new string[1] {
                "http://localhost:4001/logout"
            };
            updateClientRequest.RedirectUris = new string[3] {
                "http://localhost:6001/login",
                "http://localhost:7002/login",
                "http://localhost:7003/login"
            };
            updateClientRequest.CorsOrigins = new string[1] {
                "http://localhost:7777"
            };
            updateClientRequest.Scopes = new string[2] {
                "openid",
                "role"
            };

            var updatedClient = clientsService.UpdateClient(clientId, updateClientRequest);

            // intact from initial new client
            Assert.Equal(newClient.AccessTokenType, updatedClient.AccessTokenType);
            Assert.Equal(newClient.EnableLocalLogin, updatedClient.EnableLocalLogin);
            Assert.Equal(newClient.ClientGrantType.GrantType, updatedClient.ClientGrantType.GrantType);

            // updated since the new client's insertion
            Assert.Equal(updateClientRequest.ClientName, updatedClient.ClientName);
            Assert.Equal(updateClientRequest.PostLogoutRedirectUris, updatedClient.ClientPostLogoutRedirectUris.Select(x => x.PostLogoutRedirectUri));
            Assert.Equal(updateClientRequest.RedirectUris, updatedClient.ClientRedirectUris.Select(x => x.RedirectUri));
            Assert.Equal(updateClientRequest.CorsOrigins, updatedClient.ClientCorsOrigins.Select(x => x.Origin));
            Assert.Equal(updateClientRequest.Scopes, updatedClient.ClientScopes.Select(x => x.Scope));
        }

        [Fact]
        [Trait("TestRail", "C298673")]
        public void DeleteClient_ShouldDeleteClient_ByAuthClientId() {

            var clientId = "testId";
            var clientIdPrimaryKey = InsertTestClientIntoDbContext(clientId, Guid.NewGuid());

            clientsService.DeleteClient(clientId);

            // need to reconnect a db context since the previous one was disposed in the service under test
            IdentityServerDbContext = new IdentityServerDbContext(optionsBuilder.Options, mockSubjectPrincipal.Object, httpContextAccessorMock.Object, new DefaultSubjectFactory());

            Assert.False(IdentityServerDbContext.Clients.Where(x => x.ClientId == clientId).Any());
        }

        [Fact]
        [Trait("TestRail", "C298673")]
        public void DeleteClient_ShouldDeleteClient_ByAuthClientIdAndNoneOtherClients() {
            var clientId = "testId";
            var clientIdPrimaryKey = InsertTestClientIntoDbContext(clientId, Guid.NewGuid());

            var clientIdTwo = "testIdTwo";
            var clientIdPrimaryKeyTwo = InsertTestClientIntoDbContext(clientIdTwo, Guid.NewGuid());

            clientsService.DeleteClient(clientId);

            // need to reconnect a db context since the previous one was disposed in the service under test
            IdentityServerDbContext = new IdentityServerDbContext(optionsBuilder.Options, mockSubjectPrincipal.Object, httpContextAccessorMock.Object, new DefaultSubjectFactory());

            Assert.False(IdentityServerDbContext.Clients.Where(x => x.ClientId == clientId).Any());
            Assert.True(IdentityServerDbContext.Clients.Where(x => x.ClientId == clientIdTwo).Any());
        }

        [Fact]
        [Trait("TestRail", "C298673")]
        public void DeleteClient_ShouldSucceedDeleteClient_IfItDoesNotExist() {
            try {
                clientsService.DeleteClient("nonexistentId");
                Assert.True(true);
            } catch {
                Assert.True(false);
            }
        }

        [Fact]
        public void ShouldGetClient() {
            //arrange
            var clientId = "testId";
            var subClaimValue = Guid.NewGuid();
            var clientIdPrimaryKey = InsertTestClientIntoDbContext(clientId, subClaimValue);

            //act
            var client = clientsService.GetClient(subClaimValue.ToString());

            //assert
            client.Should().NotBeNull();
            client.ClientClaims.Should().Contain(c => c.Value == subClaimValue.ToString());
        }

        [Fact]
        public void ShouldGetClientById() {
            //arrange
            var clientId = "testId";
            var subClaimValue = Guid.NewGuid();
            var clientIdPrimaryKey = InsertTestClientIntoDbContext(clientId, subClaimValue);

            //act
            var client = clientsService.GetClient(clientIdPrimaryKey);

            //assert
            client.Should().NotBeNull();
            client.ClientClaims.Should().Contain(c => c.Value == subClaimValue.ToString());
        }

        [Fact]
        public void ShouldNotGetClientById() {
            //arrange
            var clientId = "testId";
            var subClaimValue = Guid.NewGuid();
            var clientIdPrimaryKey = InsertTestClientIntoDbContext(clientId, subClaimValue);

            //act
            var result = clientsService.GetClient(clientIdPrimaryKey + 1); // doesnt exist

            //assert
            Assert.Null(result);
        }

        [Fact]
        public void ShouldGetClientReturnNull() {
            //arrange

            //act
            var client = clientsService.GetClient(Guid.NewGuid().ToString());

            //assert
            client.Should().BeNull();
        }

        [Fact]
        public void ShouldCreateClient() {
            //arrange
            var request = new CreateClientModel {
                ClientId = "clientId",
                SubjectId = "subjectId",
                ClientName = "name",
                Email = "test@gmail.com",
                PhoneNumber = "555-55555",
                Scopes = new List<string> {
                    "scope1",
                    "scope2"
                },
                Claims = new List<UserClaimModel> {
                    new UserClaimModel {
                        Type = "customClaim1",
                        Value = "customValue1"
                    },
                    new UserClaimModel {
                        Type = "customClaim2",
                        Value = "customValue2"
                    }
                }
            };

            //act
            var client = clientsService.CreateClient(request);

            //assert
            client.Should().NotBeNull();
            Assert.Equal(5, client.ClientClaims.Count); // three custom claims + subject claim
            Assert.Equal(2, client.ClientScopes.Count); // custom scope
            Assert.Equal(request.ClientId, client.ClientId);
            Assert.Equal(request.ClientName, client.ClientName);
        }

        [Fact]
        public void ShouldNotCreateClient_DuplicateClientId() {
            //arrange
            var guid = Guid.NewGuid();
            InsertTestClientIntoDbContext("clientId", guid);
            var request = new CreateClientModel {
                ClientId = "clientId",
                SubjectId = guid.ToString(),
                ClientName = "name",
                Scopes = new List<string> {
                    "scope1",
                    "scope2"
                },
                Claims = new List<UserClaimModel> {
                    new UserClaimModel {
                        Type = "customClaim1",
                        Value = "customValue1"
                    },
                    new UserClaimModel {
                        Type = "customClaim2",
                        Value = "customValue2"
                    }
                }
            };

            //act/assert
            Assert.Throws<BadRequestResponseException>(() => clientsService.CreateClient(request));
        }

        [Fact]
        public void ShouldNotCreateClient_NullEmail() {
            //arrange
            var guid = Guid.NewGuid();
            InsertTestClientIntoDbContext("clientId", guid);
            var request = new CreateClientModel {
                ClientId = "clientId",
                SubjectId = guid.ToString(),
                ClientName = "name",
                Scopes = new List<string> {
                    "scope1",
                    "scope2"
                },
                Claims = new List<UserClaimModel> {
                    new UserClaimModel {
                        Type = "customClaim1",
                        Value = "customValue1"
                    },
                    new UserClaimModel {
                        Type = "customClaim2",
                        Value = "customValue2"
                    }
                }
            };

            //act/assert
            Assert.Throws<BadRequestResponseException>(() => clientsService.CreateClient(request));
        }

        [Fact]
        public void ShouldNotCreateClient_NullSubjectId() {
            //arrange
            var guid = Guid.NewGuid();
            InsertTestClientIntoDbContext("clientId", guid);
            var request = new CreateClientModel {
                ClientId = "clientId",
                ClientName = "name",
                Email = "test@gmail.com",
                Scopes = new List<string> {
                    "scope1",
                    "scope2"
                },
                Claims = new List<UserClaimModel> {
                    new UserClaimModel {
                        Type = "customClaim1",
                        Value = "customValue1"
                    },
                    new UserClaimModel {
                        Type = "customClaim2",
                        Value = "customValue2"
                    }
                }
            };

            //act/assert
            Assert.Throws<BadRequestResponseException>(() => clientsService.CreateClient(request));
        }

        [Fact]
        public void ShouldNotCreateClient_NullClientId() {
            //arrange
            var guid = Guid.NewGuid();
            InsertTestClientIntoDbContext("clientId", guid);
            var request = new CreateClientModel {
                ClientName = "name",
                SubjectId = "subjectId",
                Email = "test@gmail.com",
                Scopes = new List<string> {
                    "scope1",
                    "scope2"
                },
                Claims = new List<UserClaimModel> {
                    new UserClaimModel {
                        Type = "customClaim1",
                        Value = "customValue1"
                    },
                    new UserClaimModel {
                        Type = "customClaim2",
                        Value = "customValue2"
                    }
                }
            };

            //act/assert
            Assert.Throws<BadRequestResponseException>(() => clientsService.CreateClient(request));
        }

        [Fact]
        public void ShouldUpdateClient() {
            //arrange
            var id = InsertTestClientIntoDbContext("clientId", Guid.NewGuid());
            var request = new UpdateClientModel {
                ClientName = "newName",
                Email = "test@test.com",
                PhoneNumber = "888888888",
                Claims = new List<UserClaimModel> {
                    new UserClaimModel {
                        Type = "abc",
                        Value = "def"
                    }
                },
                Scopes = new List<string> {
                    "newScope"
                }
            };

            //act
            var client = clientsService.UpdateClient(id, request);

            //assert
            client.Should().NotBeNull();
            client.ClientName.Should().Be("newName");
            client.ClientScopes.Should().Contain(x => x.Scope == "newScope");
            client.ClientClaims.Should().Contain(x => x.Type == "abc");
            client.ClientClaims.Should().Contain(x => x.Type == "sub");// provided by the setup
            client.ClientScopes.Count.Should().Be(1);
            client.ClientClaims.Count.Should().Be(4);
        }

        [Fact]
        public void ShouldNotUpdateClientSubClaim() {
            //arrange
            var id = InsertTestClientIntoDbContext("clientId", Guid.NewGuid());
            var request = new UpdateClientModel {
                ClientName = "newName",
                Claims = new List<UserClaimModel> {
                    new UserClaimModel {
                        Type = "sub",
                        Value = "def"
                    }
                }
            };

            //act

            //assert
            Assert.Throws<BadRequestResponseException>(() => clientsService.UpdateClient(id, request));
        }

        [Fact]
        public void ShouldNotUpdateInvalidClient() {
            //arrange
            var id = InsertTestClientIntoDbContext("clientId", Guid.NewGuid());
            var request = new UpdateClientModel {
                ClientName = "newName",
                Claims = new List<UserClaimModel> {
                    new UserClaimModel {
                        Type = "sub",
                        Value = "def"
                    }
                }
            };

            //act

            //assert
            Assert.Throws<BadRequestResponseException>(() => clientsService.UpdateClient(id + 1, request)); //id+1 should not exist and throw
        }

        [Fact]
        public void ShouldUpdateClientScopes() {
            // Arrange
            var id = InsertTestClientIntoDbContext("clientId", Guid.NewGuid());
            var request = new UpdateClientScopesModel {
                Scopes = new List<string> {
                    "newScope"
                }
            };

            // Act
            var clientScopes = clientsService.UpdateClientScopes(id, request);

            // Assert
            clientScopes.Should().NotBeNull();
            clientScopes.Scopes.Should().Contain(x => x == "newScope");
            clientScopes.Scopes.Count.Should().Be(1);
        }

        [Fact]
        public void UpdateClientClaims_AddThreeFullControlClaims() {
            // Arrange
            var model = GetDefaultClientClaims();
            var id = InsertTestClientIntoDbContext("clientId", Guid.NewGuid());
            var updateOnlyClaimTypes = new List<string>() {
                "email",
                "email_verified",
                "phone_number",
                "phone_number_verified"
            };

            // Act
            var clientClaims = clientsService.UpdateClientClaims(id, model);

            // Assert
            var addedClaims = IdentityServerDbContext.ClientClaims.Where(x => x.Type != "sub").ToList();
            addedClaims.Count().Should().Be(5);
            var updateOnlyClaims = addedClaims.Where(x => updateOnlyClaimTypes.Contains(x.Type));
            updateOnlyClaims.Count().Should().Be(2);
        }

        [Fact]
        public void UpdateClientClaims_Update2UpdateOnlyClaims() {
            // Arrange
            var model = GetDefaultClientClaims();
            var id = InsertTestClientIntoDbContext("clientId", Guid.NewGuid());
            var updateOnlyClaimTypes = new List<string>() {
                "email",
                "email_verified",
                "phone_number",
                "phone_number_verified"
            };
            var clientClaims = new List<ClientClaim>() {
                new ClientClaim() {
                    ClientId = id,
                    Id = 0,
                    Type = "email",
                    Value = "StartValue"
                },

                new ClientClaim() {
                    ClientId = id,
                    Id = 0,
                    Type = "phone_number",
                    Value = "StartValue"
                },
            };

            clientClaims.ForEach(item => {
                IdentityServerDbContext.ClientClaims.Add(item);
            });

            IdentityServerDbContext.SaveChanges();


            // Act
            clientsService.UpdateClientClaims(id, model);

            // Assert
            var totalClaims = IdentityServerDbContext.ClientClaims.Where(x => x.Type != "sub").ToList();
            var updateOnlyClaims = totalClaims.Where(x => updateOnlyClaimTypes.Contains(x.Type));
            var emailClaim = updateOnlyClaims.FirstOrDefault(x => x.Type == "email");
            var phoneNumberClaim = updateOnlyClaims.FirstOrDefault(x => x.Type == "phone_number");

            totalClaims.Count().Should().Be(7);
            updateOnlyClaims.Count().Should().Be(4);
            Assert.Equal("test@email.com", emailClaim.Value);
            Assert.Equal("123456789", phoneNumberClaim.Value);
        }

        [Fact]
        public void UpdateClientClaims_UpdatingSub_ShouldThrowError() {
            // Arrange
            var model = new UpdateClientClaimsModel() {
                Claims = new List<UserClaimModel>() {
                    new UserClaimModel() {
                        Type = "sub",
                        Value = "test-sub"
                    }
                }
            };

            var id = InsertTestClientIntoDbContext("clientId", Guid.NewGuid());

            // Act / Assert
            Assert.Throws<BadRequestResponseException>(() => clientsService.UpdateClientClaims(id, model));
        }

        private UpdateClientClaimsModel GetDefaultClientClaims() {
            var model = new UpdateClientClaimsModel() {
                Claims = new List<UserClaimModel>() {
                    new UserClaimModel() {
                        Type = "role",
                        Value = "api"
                    },

                    new UserClaimModel() {
                        Type = "family_name",
                        Value = "test_family"
                    },

                    new UserClaimModel() {
                        Type = "test",
                        Value = "test"
                    },

                    new UserClaimModel() {
                        Type = "email",
                        Value = "test@email.com"
                    },

                    new UserClaimModel() {
                        Type = "phone_number",
                        Value = "123456789"
                    },

                    new UserClaimModel() {
                        Type = "email_verified",
                        Value = "test@email.com"
                    },

                    new UserClaimModel() {
                        Type = "phone_number_verified",
                        Value = "123456789"
                    }
                }
            };

            return model;
        }
    }
}

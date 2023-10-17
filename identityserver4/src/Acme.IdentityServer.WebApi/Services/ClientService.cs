using System;
using System.Collections.Generic;
using System.Linq;
using Cortside.Common.Messages.MessageExceptions;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Data.DbSetComparers;
using Acme.IdentityServer.WebApi.Helpers;
using Acme.IdentityServer.WebApi.Models;
using Acme.IdentityServer.WebApi.Models.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static IdentityServer4.IdentityServerConstants;

namespace Acme.IdentityServer.WebApi.Services {
    public class ClientService : IClientService {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<ClientService> logger;
        private readonly IHashProvider hashProvider;
        private readonly IClientSecretService clientSecretService;
        private readonly IPhoneNumberHelper phoneNumberHelper;

        public ClientService(IServiceProvider serviceProvider, ILogger<ClientService> logger, IHashProvider hashProvider, IClientSecretService clientSecretService, IPhoneNumberHelper phoneNumberHelper) {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.hashProvider = hashProvider;
            this.clientSecretService = clientSecretService;
            this.phoneNumberHelper = phoneNumberHelper;
        }

        /// <summary>
        /// Deletes the client ID and all affiliated settings.
        /// </summary>
        /// <param name="clientId">Client ID, a string name that corresponds with an auth client.</param>
        /// <returns></returns>
        public void DeleteClient(string clientId) {

            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                var client = dbContext.Clients.Where(x => x.ClientId == clientId).FirstOrDefault();

                if (client != null) {
                    dbContext.Clients.Remove(client);
                }

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Updates the settings pertaining to the client ID,
        /// applying to all affiliated entities in their entirety.
        /// </summary>
        /// <param name="clientId">Client ID, a string name that corresponds with an auth client.</param>
        /// <param name="updateClientRequest">The request object containing settings to update.</param>
        /// <returns>Client that was inserted or updated, containing all child entities.</returns>
        public Client UpdateClient(string clientId, UpdateClientRequest updateClientRequest) {

            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                // main aggregate
                var client = UpsertClient(dbContext, clientId, updateClientRequest);

                // single object
                UpsertClientGrantType(client, updateClientRequest);

                // collections
                UpsertClientPostLogoutRedirectUris(dbContext, client, updateClientRequest);
                UpsertClientRedirectUris(dbContext, client, updateClientRequest);
                UpsertClientCorsOrigins(dbContext, client, updateClientRequest);
                UpsertClientScopes(dbContext, client, updateClientRequest);

                dbContext.SaveChanges();

                return client;
            }
        }

        /// <summary>
        /// Updates a client given a user client update request
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <param name="request">Model containing the claims/scopes</param>
        /// <returns></returns>
        public Client UpdateClient(int id, UpdateClientModel request) {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();

            var client = db.Clients.FirstOrDefault(x => x.Id == id);
            if (client == null) {
                throw new BadRequestResponseException($"Updating a client that does not exist is not allowed.");
            }

            if (request.Claims != null && request.Claims.Any(x => x.Type == "sub")) {
                throw new BadRequestResponseException($"Updating client sub claim is not allowed");
            }

            if (string.IsNullOrEmpty(request.Email)) {
                throw new BadRequestResponseException($"Updating a client without providing an Email is not allowed.");
            }

            if (string.IsNullOrEmpty(request.PhoneNumber)) {
                throw new BadRequestResponseException($"Updating a client without providing a PhoneNumber is not allowed.");
            }

            // update client properties
            client.ClientName = request.ClientName;
            client.Enabled = request.Enabled;

            // update the scopes
            // remove any existing ones
            var current_scopes = db.ClientScopes.Where(x => x.ClientId == client.Id).ToList();
            db.ClientScopes.RemoveRange(current_scopes);

            // add the new scopes
            request.Scopes?.ForEach(item => {
                db.ClientScopes.Add(new ClientScope {
                    ClientId = client.Id,
                    Scope = item
                });
            });

            // update the claims
            // remove any existing ones
            var current_claims = db.ClientClaims.Where(x => x.ClientId == client.Id && x.Type != "sub" && x.Type != "email" && x.Type != "phone_number").ToList(); // exclude the sub and email claims
            db.ClientClaims.RemoveRange(current_claims);

            var phoneNumberClaim = request.Claims.FirstOrDefault(x => x.Type == "phone_number");
            if (phoneNumberClaim != null) {
                var newPhoneNumber = phoneNumberHelper.FormatPhoneNumber(phoneNumberClaim.Value);
                phoneNumberClaim.Value = newPhoneNumber;
            }

            // add the new claims
            request.Claims?.ForEach(item => {
                db.ClientClaims.Add(new ClientClaim {
                    ClientId = client.Id,
                    Type = item.Type,
                    Value = item.Value
                });
            });

            // updates the email claim
            var emailClaim = db.ClientClaims.FirstOrDefault(x => x.ClientId == client.Id && x.Type == "email");
            if (emailClaim != null) {
                emailClaim.Value = request.Email;
            }

            // updates the phone claim
            var phoneClaim = db.ClientClaims.FirstOrDefault(x => x.ClientId == client.Id && x.Type == "phone_number");
            if (phoneClaim != null) {
                phoneClaim.Value = request.PhoneNumber;
            } else {
                db.ClientClaims.Add(new ClientClaim {
                    ClientId = client.Id,
                    Type = "phone_number",
                    Value = request.PhoneNumber
                });
            }
            db.SaveChanges();
            return client;
        }

        /// <summary>
        /// Updates a client's scopes given a user client scope update request
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <param name="model">Model containing the scopes</param>
        /// <returns></returns>
        public UpdateClientScopesModel UpdateClientScopes(int id, UpdateClientScopesModel model) {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();

            var client = db.Clients.FirstOrDefault(x => x.Id == id);
            if (client == null) {
                throw new BadRequestResponseException($"Updating a client that does not exist is not allowed.");
            }

            // update the scopes
            // remove any existing ones
            var currentScopes = db.ClientScopes.Where(x => x.ClientId == client.Id).ToList();
            db.ClientScopes.RemoveRange(currentScopes);

            // add the new scopes
            model.Scopes?.ForEach(item => {
                db.ClientScopes.Add(new ClientScope {
                    ClientId = client.Id,
                    Scope = item
                });
            });

            db.SaveChanges();
            return model;
        }

        /// <summary>
        /// Updates a client's claims given a user client claims update request
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <param name="model">Model containing the claims</param>
        /// <returns></returns>
        public UpdateClientClaimsModel UpdateClientClaims(int id, UpdateClientClaimsModel model) {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();

            var client = db.Clients.FirstOrDefault(x => x.Id == id);
            if (client == null) {
                throw new BadRequestResponseException($"Updating a client that does not exist is not allowed.");
            }

            if (model.Claims != null && model.Claims.Any(x => x.Type == "sub")) {
                throw new BadRequestResponseException($"Updating client sub claim is not allowed");
            }

            var phoneNumber = model.Claims.FirstOrDefault(x => x.Type == "phone_number");
            if (phoneNumber != null) {
                var newPhoneNumber = phoneNumberHelper.FormatPhoneNumber(phoneNumber.Value);
                phoneNumber.Value = newPhoneNumber;
            }

            // These claims are updateable only
            var updateOnlyClaimTypes = new List<string>() {
                "email",
                "email_verified",
                "phone_number",
                "phone_number_verified"
            };

            // These claims can be added or deleted
            var fullControlClaims = model.Claims.Where(x => !updateOnlyClaimTypes.Contains(x.Type)).ToList();

            // These claims can be updated only
            var updateOnlyClaims = model.Claims.Where(x => updateOnlyClaimTypes.Contains(x.Type)).ToList();

            // Remove any existing ones, excluding Update Only Claims
            var currentClaims = db.ClientClaims.Where(x => x.ClientId == client.Id && !updateOnlyClaimTypes.Contains(x.Type) && x.Type != "sub").ToList();
            db.ClientClaims.RemoveRange(currentClaims);

            // Add the new claims
            fullControlClaims?.ForEach(item => {
                db.ClientClaims.Add(new ClientClaim {
                    ClientId = client.Id,
                    Type = item.Type,
                    Value = item.Value
                });
            });

            // Updates existing update only claims
            var existingUpdateOnlyClaims = db.ClientClaims.Where(x => x.ClientId == client.Id && updateOnlyClaimTypes.Contains(x.Type)).ToList();
            existingUpdateOnlyClaims?.ForEach(item => {
                var newValue = updateOnlyClaims.FirstOrDefault(x => x.Type == item.Type)?.Value;
                if (!string.IsNullOrWhiteSpace(newValue)) {
                    item.Value = newValue;
                }
            });

            db.SaveChanges();

            return model;
        }

        /// <summary>
        /// Retrieves client info by the client sub from the claims table
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public Client GetClient(string clientSubjectId) {
            using (var scope = serviceProvider.CreateScope()) {
                logger.LogInformation($"Looking up client with sub {clientSubjectId}");
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                var subClaim = dbContext.ClientClaims.FirstOrDefault(c => c.Type == "sub" && c.Value == clientSubjectId);
                if (subClaim == null) {
                    logger.LogInformation($"client with sub {clientSubjectId} not found");
                    return null;
                } else {
                    var client = dbContext.Clients.Where(c => c.Id == subClaim.ClientId).Include(x => x.ClientClaims).FirstOrDefault();
                    return client;
                }
            }
        }

        public Client GetClient(int id) {
            using (var scope = serviceProvider.CreateScope()) {
                logger.LogInformation($"Looking up client with id {id}");
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                var client = dbContext.Clients.FirstOrDefault(x => x.Id == id);

                if (client == null) {
                    logger.LogInformation($"client with id {id} not found");
                    return null;
                }

                client.ClientScopes = dbContext.ClientScopes.Where(x => x.ClientId == client.Id).ToList();
                client.ClientClaims = dbContext.ClientClaims.Where(x => x.ClientId == client.Id).ToList();

                return client;
            }
        }

        public Client CreateClient(CreateClientModel request) {
            using var scope = serviceProvider.CreateScope();
            logger.LogInformation($"Creating client with id {request.ClientId}");
            var db = scope.ServiceProvider.GetRequiredService<IdentityServerDbContext>();
            using var transaction = db.Database.BeginTransaction();

            try {
                // check
                if (string.IsNullOrEmpty(request.SubjectId)) {
                    throw new BadRequestResponseException($"Creating a new client without providing a SubjectId is not allowed.");
                }

                if (string.IsNullOrEmpty(request.Email)) {
                    throw new BadRequestResponseException($"Creating a new client without providing an Email is not allowed.");
                }

                if (string.IsNullOrEmpty(request.PhoneNumber)) {
                    throw new BadRequestResponseException($"Creating a new client without providing a PhoneNumber is not allowed.");
                }

                if (string.IsNullOrEmpty(request.ClientId)) {
                    throw new BadRequestResponseException($"Creating a new client without providing a ClientId is not allowed.");
                }

                // first check to make sure this client doesnt already exist
                if (db.Clients.Any(x => x.ClientId == request.ClientId) || db.ClientClaims.Any(x => x.Type == "sub" && x.Value == request.SubjectId)) {
                    throw new BadRequestResponseException($"ClientId/SubjectId {request.ClientId}/{request.SubjectId} already exists, cannot create");
                }

                // create the client
                var client = new Client() {
                    ClientId = request.ClientId,
                    ClientName = request.ClientName,
                    AccessTokenType = (int)IdentityServer4.Models.AccessTokenType.Jwt,
                    Enabled = request.Enabled
                };
                SetClientDefaultValues(client);

                db.Clients.Add(client);
                db.SaveChanges();

                // save the client grant type
                db.ClientGrantTypes.Add(new ClientGrantType {
                    ClientId = client.Id,
                    GrantType = "client_credentials"
                });
                db.SaveChanges();

                // save the client claims
                SaveNewClientClaims(db, request, client.Id);

                // save the scopes
                request.Scopes?.ForEach(item => {
                    db.ClientScopes.Add(new ClientScope {
                        ClientId = client.Id,
                        Scope = item
                    });
                });
                db.SaveChanges();

                // normally here we would then either generate/save the secret
                // and insert it, or email the user a one time code that redirects back to here
                // so that they can setup the secret themselves. TBD on this
                var input = "secret";
                var secret = hashProvider.ComputeHash256(input);
                ///////////////////////////////////////////////////////////////

                db.ClientSecrets.Add(new ClientSecret {
                    ClientId = client.Id,
                    Type = ParsedSecretTypes.SharedSecret,
                    Created = DateTime.UtcNow,
                    Value = secret
                });
                db.SaveChanges();

                clientSecretService.SaveNewClientSecretRequest(db, client.Id);

                // commit it
                transaction.Commit();

                clientSecretService.SendClientSecretEmail(client.Id);

                return client;
            } catch (Exception e) {
                logger.LogError($"Failed creating new client {request.ClientId}", e);
                transaction.Rollback();
                throw e;
            }
        }

        private void SaveNewClientClaims(IIdentityServerDbContext db, CreateClientModel request, int clientId) {
            request.Claims?.ForEach(claim => {
                db.ClientClaims.Add(new ClientClaim {
                    ClientId = clientId,
                    Type = claim.Type,
                    Value = claim.Value
                });
            });

            db.ClientClaims.Add(new ClientClaim {
                ClientId = clientId,
                Type = "sub",
                Value = request.SubjectId
            });

            db.ClientClaims.Add(new ClientClaim {
                ClientId = clientId,
                Type = "email",
                Value = request.Email
            });

            db.ClientClaims.Add(new ClientClaim {
                ClientId = clientId,
                Type = "phone_number",
                Value = phoneNumberHelper.FormatPhoneNumber(request.PhoneNumber)
            });

            db.SaveChanges();
        }

        private void DeleteClient<T>(DbSet<T> collectionToDeleteFrom, Func<T, bool> predicateForDeletion) where T : class {
            IEnumerable<T> deprecatedEntities = collectionToDeleteFrom.Where(predicateForDeletion);
            collectionToDeleteFrom.RemoveRange(deprecatedEntities);
        }

        // the main client entry, from which the others have a relationship with
        private Client UpsertClient(IIdentityServerDbContext dbContext, string clientId, UpdateClientRequest updateClientRequest) {
            var client = dbContext.Clients.Where(x => x.ClientId == clientId)
                            .Include(x => x.ClientGrantType)
                            .Include(x => x.ClientPostLogoutRedirectUris)
                            .Include(x => x.ClientRedirectUris)
                            .Include(x => x.ClientCorsOrigins)
                            .Include(x => x.ClientScopes)
                            .FirstOrDefault();

            if (client == null) {
                client = new Client() {
                    ClientId = clientId
                };

                client = dbContext.Clients.Add(client).Entity;
            }

            // user-provided value mappings
            client.AccessTokenType = updateClientRequest.AccessTokenType;
            client.ClientName = updateClientRequest.ClientName;
            client.EnableLocalLogin = updateClientRequest.EnableLocalLogin;

            // default value mappings
            SetClientDefaultValues(client);

            return client;
        }

        private void SetClientDefaultValues(Client client) {
            client.AbsoluteRefreshTokenLifetime = 2592000;
            client.AccessTokenLifetime = 3600;
            client.AllowAccessTokensViaBrowser = true;
            client.AllowRememberConsent = true;
            client.AlwaysIncludeUserClaimsInIdToken = true;
            client.AlwaysSendClientClaims = true;
            client.AuthorizationCodeLifetime = 300;
            client.BackChannelLogoutSessionRequired = true;
            client.FrontChannelLogoutSessionRequired = true;
            client.IdentityTokenLifetime = 300;
            client.RefreshTokenExpiration = 1;
            client.RefreshTokenUsage = 1;
            client.RequireClientSecret = true;
            client.SlidingRefreshTokenLifetime = 1296000;
            client.NonEditable = false;
            client.ProtocolType = "oidc";
            client.EnableLocalLogin = true;
        }

        private void UpsertClientGrantType(Client client, UpdateClientRequest updateClientRequest) {
            var clientGrantType = client.ClientGrantType;

            if (clientGrantType == null) {
                client.ClientGrantType = new ClientGrantType();
                clientGrantType = client.ClientGrantType;
            }

            clientGrantType.GrantType = updateClientRequest.GrantType;
        }

        private void UpsertClientPostLogoutRedirectUris(IIdentityServerDbContext dbContext, Client client, UpdateClientRequest updateClientRequest) {
            var currentClientPostLogoutRedirectUris = client.ClientPostLogoutRedirectUris;

            if (currentClientPostLogoutRedirectUris == null) {
                client.ClientPostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>();
                currentClientPostLogoutRedirectUris = client.ClientPostLogoutRedirectUris;
            }

            var updateClientPostLogoutRedirectUris = updateClientRequest.PostLogoutRedirectUris.Select(x =>
                new ClientPostLogoutRedirectUri() {
                    PostLogoutRedirectUri = x
                });

            AddNewItemsAndMarkOldItemsForDeletion(dbContext, currentClientPostLogoutRedirectUris, updateClientPostLogoutRedirectUris, new ClientPostLogoutRedirectUriComparer());
        }

        private void UpsertClientRedirectUris(IIdentityServerDbContext dbContext, Client client, UpdateClientRequest updateClientRequest) {
            var currentClientRedirectUris = client.ClientRedirectUris;

            if (currentClientRedirectUris == null) {
                client.ClientRedirectUris = new List<ClientRedirectUri>();
                currentClientRedirectUris = client.ClientRedirectUris;
            }

            var updateClientRedirectUris = updateClientRequest.RedirectUris.Select(x =>
                new ClientRedirectUri() {
                    RedirectUri = x
                });

            AddNewItemsAndMarkOldItemsForDeletion(dbContext, currentClientRedirectUris, updateClientRedirectUris, new ClientRedirectUriComparer());
        }

        private void UpsertClientCorsOrigins(IIdentityServerDbContext dbContext, Client client, UpdateClientRequest updateClientRequest) {
            var currentClientCorsOrigins = client.ClientCorsOrigins;

            if (currentClientCorsOrigins == null) {
                client.ClientCorsOrigins = new List<ClientCorsOrigin>();
                currentClientCorsOrigins = client.ClientCorsOrigins;
            }

            var updateClientCorsOrigins = updateClientRequest.CorsOrigins.Select(x =>
                new ClientCorsOrigin() {
                    Origin = x
                });

            AddNewItemsAndMarkOldItemsForDeletion(dbContext, currentClientCorsOrigins, updateClientCorsOrigins, new ClientCorsOriginComparer());
        }

        private void UpsertClientScopes(IIdentityServerDbContext dbContext, Client client, UpdateClientRequest updateClientRequest) {
            var currentClientScopes = client.ClientScopes;

            if (currentClientScopes == null) {
                client.ClientScopes = new List<ClientScope>();
                currentClientScopes = client.ClientScopes;
            }

            var updateClientScopes = updateClientRequest.Scopes.Select(x =>
                new ClientScope() {
                    Scope = x
                });

            AddNewItemsAndMarkOldItemsForDeletion(dbContext, currentClientScopes, updateClientScopes, new ClientScopeComparer());
        }

        /* 
         * Helper for updating collections that have a one to many relation with the Client
         * adds the new entries while marking the old ones for deletion based on the request body
         * that had been sent by the requestor
         */
        private void AddNewItemsAndMarkOldItemsForDeletion<T>(IIdentityServerDbContext dbContext, ICollection<T> currentCollection, IEnumerable<T> updateCollection,
                                         IEqualityComparer<T> comparer) where T : class {

            var newCollection =
                updateCollection.Except(currentCollection, comparer);

            var deprecatedCollection =
                currentCollection.Except(updateCollection, comparer);

            // need to remove a range, but without instantiating a new collection
            // marks for deletion
            dbContext.RemoveRange(deprecatedCollection);

            foreach (var newItem in newCollection) {
                currentCollection.Add(newItem);
            }
        }
    }
}

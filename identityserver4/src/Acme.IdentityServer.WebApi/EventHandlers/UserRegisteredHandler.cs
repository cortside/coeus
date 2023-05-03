using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cortside.DomainEvent;
using Cortside.DomainEvent.Handlers;
using Acme.IdentityServer.Data;
using Acme.IdentityServer.WebApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Acme.IdentityServer.EventHandlers {
    public class UserRegisteredHandler : IDomainEventHandler<UserRegisteredEvent> {
        private readonly ILogger<UserRegisteredHandler> logger;
        private readonly IServiceProvider serviceProvider;

        public UserRegisteredHandler(ILogger<UserRegisteredHandler> logger, IServiceProvider serviceProvider) {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public async Task Handle(UserRegisteredEvent @event) {
            logger.LogInformation("Received event: {Email} has registered.", @event.Email);

            var newUser = new User {
                EffectiveDate = DateTime.UtcNow,
                UserStatus = IdsDefinitions.Active,
                Username = @event.Email,
                Password = @event.Password,
                Salt = @event.Salt,
                TermsOfUseAcceptanceDate = @event.AgreeToTerms,
                CreateDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };

            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                //Probably not a great way of doing this, need a user for audit fields.
                var theFirstUser = dbContext.Users.OrderBy(x => x.UserId).First();
                var role = dbContext.Roles.FirstOrDefault(r => r.Name == @event.Role);

                if (role == null) {
                    role = new Role {
                        Name = @event.Role,
                        Description = @event.Role,
                        CreateDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow,
                        CreateUserId = theFirstUser.UserId,
                        LastModifiedUserId = theFirstUser.UserId
                    };
                    dbContext.AddRole(role);
                }

                newUser.CreateUserId = theFirstUser.UserId;
                newUser.LastModifiedUserId = theFirstUser.UserId;
                newUser.UserRoles = new List<UserRole> {
                    new UserRole {
                        User = newUser,
                        Role = role
                    }
                };

                dbContext.AddUser(newUser);

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<HandlerResult> HandleAsync(DomainEventMessage<UserRegisteredEvent> @event) {
            await Handle(@event.Data);
            return HandlerResult.Success;
        }
    }
}

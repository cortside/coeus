using System;
using System.Threading.Tasks;
using Acme.IdentityServer.WebApi.Services;
using IdentityServer4.Configuration;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Acme.IdentityServer.WebApi.Events {
    /// <summary>
    /// Enriching event service with user information
    /// </summary>
    public class UserEnrichingEventService : DefaultEventService {
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserEnrichingEventService"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="context">The context.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="clock">The clock.</param>
        public UserEnrichingEventService(IdentityServerOptions options, IHttpContextAccessor context, IEventSink sink, ISystemClock clock, IUserService userService) : base(options, context, sink, clock) {
            this.userService = userService;
        }

        /// <summary>
        /// Prepares the event.
        /// </summary>
        /// <param name="evt">The evt.</param>
        /// <returns></returns>
        protected override async Task PrepareEventAsync(Event evt) {
            await base.PrepareEventAsync(evt);

            if (evt is TokenIssuedSuccessEvent @event0) {
                //if (string.IsNullOrWhiteSpace(@event0.Username)) {
                @event0.Username = await GetUserPrincipalName(@event0.SubjectId);
                //}
            }
            if (evt is TokenIssuedFailureEvent @event1) {
                //if (string.IsNullOrWhiteSpace(@event1.Username)) {
                @event1.Username = await GetUserPrincipalName(@event1.SubjectId);
                //}
            }
            if (evt is UserLoginSuccessEvent @event2) {
                //if (string.IsNullOrWhiteSpace(@event2.Username)) {
                @event2.Username = await GetUserPrincipalName(@event2.SubjectId);
                //}
            }
            if (evt is UserLogoutSuccessEvent @event3) {
                //if (string.IsNullOrWhiteSpace(@event3.DisplayName)) {
                @event3.Username = await GetUserPrincipalName(@event3.SubjectId);
                //}
            }
        }

        private async Task<string> GetUserPrincipalName(string subjectId) {
            if (Guid.TryParse(subjectId, out var userId)) {
                try {
                    var user = await userService.FindBySubjectIdAsync(userId);
                    return user?.Username ?? string.Empty;
                } catch (Exception) {
                    return string.Empty;
                }
            }
            return string.Empty;
        }
    }
}

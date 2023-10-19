using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acme.IdentityServer.WebApi.Data {
    public class Client {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AbsoluteRefreshTokenLifetime { get; set; }
        public int AccessTokenLifetime { get; set; }
        public int AccessTokenType { get; set; }
        public bool AllowAccessTokensViaBrowser { get; set; }
        public bool AllowOfflineAccess { get; set; }
        public bool AllowPlainTextPkce { get; set; }
        public bool AllowRememberConsent { get; set; }
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        public string AllowedIdentityTokenSigningAlgorithms { get; set; }
        public bool AlwaysSendClientClaims { get; set; }
        public int AuthorizationCodeLifetime { get; set; }
        public bool BackChannelLogoutSessionRequired { get; set; }
        public string BackChannelLogoutUri { get; set; }
        public string ClientClaimsPrefix { get; set; }
        public string ClientId { get; set; } = ""; // default value not null
        public string ClientName { get; set; }
        public string ClientUri { get; set; }
        public int? ConsentLifetime { get; set; }
        public string Description { get; set; }
        public bool EnableLocalLogin { get; set; }
        public bool Enabled { get; set; } = true;
        public bool FrontChannelLogoutSessionRequired { get; set; }
        public string FrontChannelLogoutUri { get; set; }
        public int IdentityTokenLifetime { get; set; }
        public bool IncludeJwtId { get; set; }
        public string LogoUri { get; set; }
        public string PairWiseSubjectSalt { get; set; }
        public bool LogoutSessionRequired { get; set; }
        public string LogoutUri { get; set; }
        public bool NonEditable { get; set; }
        public string ProtocolType { get; set; }
        public int RefreshTokenExpiration { get; set; }
        public int RefreshTokenUsage { get; set; }
        public bool RequireClientSecret { get; set; }
        public bool RequireConsent { get; set; }
        public bool RequirePkce { get; set; }
        public bool RequireRequestObject { get; set; }
        public int SlidingRefreshTokenLifetime { get; set; }
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

        public ClientGrantType ClientGrantType { get; set; }
        public ClientSecret ClientSecret { get; set; }

        public ICollection<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris { get; set; }

        public ICollection<ClientRedirectUri> ClientRedirectUris { get; set; }

        public ICollection<ClientCorsOrigin> ClientCorsOrigins { get; set; }

        public ICollection<ClientScope> ClientScopes { get; set; }
        public ICollection<ClientClaim> ClientClaims { get; set; }
    }
}

using System;

namespace Acme.IdentityServer.WebApi.Data {
    public class ClientSecretRequest : AuditableEntity {

        public Guid ClientSecretRequestId { get; set; }
        public int ClientId { get; set; }
        public Guid Token { get; set; }
        public string SmsVerificationCode { get; set; }
        public DateTime RequestExpirationDate { get; set; }
        public DateTime SmsVerificationExpiration { get; set; }
        public int RemainingSmsVerificationAttempts { get; set; }
    }
}

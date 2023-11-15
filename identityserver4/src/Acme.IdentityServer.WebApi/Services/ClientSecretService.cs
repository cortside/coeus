using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cortside.Common.Messages.MessageExceptions;
using Cortside.DomainEvent.EntityFramework;
using Acme.DomainEvent.Events;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Models.Input;
using Acme.IdentityServer.WebApi.Models.Output;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static IdentityServer4.IdentityServerConstants;

namespace Acme.IdentityServer.WebApi.Services {
    public class ClientSecretService : IClientSecretService {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<ClientSecretService> logger;
        private readonly IConfiguration config;
        private readonly IHashProvider hashProvider;

        public ClientSecretService(IServiceProvider serviceProvider, ILogger<ClientSecretService> logger, IConfiguration config, IHashProvider hashProvider) {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.config = config;
            this.hashProvider = hashProvider;
        }

        /// <summary>
        /// Will send the client a secret request to their email claim
        /// </summary>
        /// <param name="clientId"></param>
        public async Task SendClientSecretEmail(int clientId) {
            using var scope = serviceProvider.CreateScope();
            logger.LogInformation($"Sending Client Secret Email for Client Id: {clientId}");
            var db = scope.ServiceProvider.GetRequiredService<IdentityServerDbContext>();

            var client = db.Clients.FirstOrDefault(x => x.Id == clientId);
            if (client == null) {
                throw new BadRequestResponseException($"ClientId: {clientId} cannot have a secret reset because it does not exist.");
            }

            var emailClaim = db.ClientClaims.FirstOrDefault(x => x.ClientId == client.Id && x.Type == "email");
            if (emailClaim == null) {
                throw new BadRequestResponseException($"ClientId: {clientId} cannot have it's secret reset because it doesn't have an email claim.");
            }

            var clientSecretRequest = db.ClientSecretRequests.Where(x => x.ClientId == clientId)
                                                             .OrderByDescending(o => o.CreateDate)
                                                             .FirstOrDefault();
            if (clientSecretRequest == null) {
                throw new BadRequestResponseException($"ClientId: {clientId} cannot have it's secret reset because it doesn't have a secret request.");
            }

            var urlParametersEncoded = EncodeClientSecretRequest(clientSecretRequest.ClientSecretRequestId, clientSecretRequest.Token);

            var urlBase = "https://" + config["traefikHostname"];

            var url = urlBase + "/resetclientsecret?requestId=" + urlParametersEncoded;

            ResetClientSecretNotificationEvent @event = new ResetClientSecretNotificationEvent {
                Url = url,
                Recipients = new List<string> { emailClaim.Value }
            };

            try {
                var publisher = scope.ServiceProvider.GetRequiredService<IDomainEventOutboxPublisher>();
                await publisher.PublishAsync(@event);
                await db.SaveChangesAsync();
            } catch (Exception e) {
                logger.LogError(e, $"Failed to publish message {@event}");
                throw;
            }
        }

        /// <summary>
        /// Will reset the client secret to an invalid hash, until the user provides a new secret
        /// via the one time code workflow
        /// </summary>
        /// <param name="clientId"></param>
        public async Task<Client> ResetSecret(int clientId) {
            using var scope = serviceProvider.CreateScope();
            logger.LogInformation($"Reseting Client Secret with Id: {clientId}");
            var db = scope.ServiceProvider.GetRequiredService<IdentityServerDbContext>();

            // first make sure the client exists
            var client = db.Clients.FirstOrDefault(x => x.Id == clientId);
            if (client == null) {
                throw new BadRequestResponseException($"ClientId: {clientId} cannot have a secret reset because it does not exist");
            }

            // reset the secret to invalid-hash=
            var clientSecret = db.ClientSecrets.FirstOrDefault(x => x.ClientId == client.Id);
            if (clientSecret == null) {
                throw new BadRequestResponseException($"ClientId: {clientId} cannot have a secret reset because a current secret does not exist");
            }

            clientSecret.Value = "invalid-hash=";
            db.SaveChanges();

            SaveNewClientSecretRequest(db, clientId);

            // Send an email to the client to generate a new secret
            await SendClientSecretEmail(client.Id);

            return client;
        }

        /// <summary>
        /// Will send the client a verification code to their phone number claim
        /// via the one time code workflow
        /// </summary>
        /// <param name="clientId"></param>
        public async Task SendVerificationCode(Guid clientSecretRequestId, SendVerificationCodeModel sendVerificationCodeModel) {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IdentityServerDbContext>();

            var clientSecretRequest = db.ClientSecretRequests.FirstOrDefault(x => x.ClientSecretRequestId == clientSecretRequestId);
            if (clientSecretRequest == null) {
                throw new BadRequestResponseException($"Cannot send verification code for ClientSecretRequestId: {clientSecretRequestId} because the client secret request doesn't exist.");
            }

            if (clientSecretRequest.RequestExpirationDate < DateTime.Now) {
                throw new BadRequestResponseException($"ClientId: {clientSecretRequest.ClientId} cannot send verification code because secret request is expired.");
            }

            var tokenHashed = hashProvider.ComputeHash256(clientSecretRequest.Token.ToString());

            if (sendVerificationCodeModel.TokenHash != tokenHashed) {
                throw new BadRequestResponseException($"ClientId: {clientSecretRequest.ClientId} cannot send verification code because token hash is invalid.");
            }

            logger.LogInformation($"Sending Client Secret Verification Code for Client Id: {clientSecretRequest.ClientId}");

            var client = db.Clients.FirstOrDefault(x => x.Id == clientSecretRequest.ClientId);
            if (client == null) {
                throw new BadRequestResponseException($"ClientId: {client.ClientId} cannot send verification code because it does not exist.");
            }

            var phoneNumberClaim = db.ClientClaims.FirstOrDefault(x => x.ClientId == client.Id && x.Type == "phone_number");
            if (phoneNumberClaim == null) {
                throw new BadRequestResponseException($"ClientId: {client.ClientId} cannot send verification code because a phone number claim doesn't exist.");
            }

            var code = GenerateCode();
            clientSecretRequest.SmsVerificationCode = code;

            var expirationInMinutes = config["ClientSecretRequest:VerificationExpirationInMinutes"];
            var expirationDate = DateTime.Now.AddMinutes(int.Parse(expirationInMinutes));
            clientSecretRequest.SmsVerificationExpiration = expirationDate;

            var verificationAttempts = config["ClientSecretRequest:VerificationAttempts"];
            clientSecretRequest.RemainingSmsVerificationAttempts = int.Parse(verificationAttempts);

            db.SaveChanges();

            var payload = $"Acme USA automated msg: Your verification code is: {code}. It’ll expire in {expirationInMinutes} min. Don’t share this with anyone.";

            var @event = new SmsRequestEvent() {
                Recipients = new[] { phoneNumberClaim.Value },
                MessagePayload = payload,
                InternalReferenceId = client.ClientId
            };

            try {
                var publisher = scope.ServiceProvider.GetRequiredService<IDomainEventOutboxPublisher>();
                await publisher.PublishAsync(@event);
                await db.SaveChangesAsync();
            } catch (Exception e) {
                logger.LogError(e, $"Failed to publish message {@event}");
                throw;
            }
        }

        /// <summary>
        /// Saves a new client secret request
        /// </summary>
        /// <param name="db"></param>
        /// <param name="clientId"></param>
        public void SaveNewClientSecretRequest(IIdentityServerDbContext db, int clientId) {
            var date = DateTime.Now;
            var expirationInHoursOffset = config["ClientSecretRequest:ExpirationInHours"];
            var verificationAttempts = config["ClientSecretRequest:VerificationAttempts"];

            var clientSecretRequest = new ClientSecretRequest() {
                ClientId = clientId,
                CreateDate = date,
                ClientSecretRequestId = Guid.NewGuid(),
                Token = Guid.NewGuid(),
                LastModifiedDate = date,
                RequestExpirationDate = date.AddHours(int.Parse(expirationInHoursOffset)),
                RemainingSmsVerificationAttempts = int.Parse(verificationAttempts)
            };

            db.ClientSecretRequests.Add(clientSecretRequest);
            db.SaveChanges();
        }

        /// <summary>
        /// Validates a client secret request
        /// </summary>
        /// <param name="encodedRequestId"></param>
        public ValidateClientSecretRequestOutput ValidateClientSecretRequest(string encodedRequestId) {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IdentityServerDbContext>();

            var output = DecodeClientSecretRequest(encodedRequestId);

            var response = new ValidateClientSecretRequestOutput {
                IsValid = false,
                DecodedOutput = output
            };

            var clientSecretRequest = db.ClientSecretRequests.FirstOrDefault(x => x.ClientSecretRequestId == output.RequestId);
            if (clientSecretRequest == null) {
                return response;
            }

            if (clientSecretRequest.RequestExpirationDate < DateTime.Now) {
                return response;
            }

            var phoneNumberClaim = db.ClientClaims.FirstOrDefault(x => x.ClientId == clientSecretRequest.ClientId && x.Type == "phone_number");
            if (phoneNumberClaim == null) {
                return response;
            }

            response.Last4PhoneNumber = phoneNumberClaim.Value.Substring(phoneNumberClaim.Value.Length - 4);

            var tokenHashCompare = hashProvider.ComputeHash256(clientSecretRequest.Token.ToString());
            if (output.TokenHash != tokenHashCompare) {
                return response;
            }

            response.IsValid = true;

            return response;
        }

        /// <summary>
        /// Validates verification code
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="verificationCode"></param>
        public IsVerificationCodeValidOutput IsVerificationCodeValid(Guid requestId, string verificationCode) {
            var output = new IsVerificationCodeValidOutput {
                IsValid = false
            };

            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IdentityServerDbContext>();

            var clientSecretRequest = db.ClientSecretRequests.FirstOrDefault(x => x.ClientSecretRequestId == requestId);
            if (clientSecretRequest == null) {
                output.Reason = $"Cannot find Client Secret Request for Id: {requestId}";
                return output;
            }

            if (clientSecretRequest.RemainingSmsVerificationAttempts <= 0) {
                output.Reason = $"Too many attempts.";
                return output;
            }

            if (clientSecretRequest.SmsVerificationExpiration < DateTime.Now) {
                output.Reason = $"Code is expired.";
                return output;
            }

            clientSecretRequest.RemainingSmsVerificationAttempts--;
            db.SaveChanges();

            if (clientSecretRequest.SmsVerificationCode != verificationCode) {
                output.Reason = $"Incorrect verification code.";
                return output;
            }

            output.IsValid = true;
            return output;
        }

        /// <summary>
        /// Decodes an encoded request string
        /// </summary>
        /// <param name="encodedRequestId"></param>
        public ClientSecretRequestDecodedOutput DecodeClientSecretRequest(string encodedRequestId) {
            var urlParametersDecoded = Convert.FromBase64String(encodedRequestId);
            var urlParametersString = Encoding.UTF8.GetString(urlParametersDecoded);
            var parameters = urlParametersString.Split(":");
            var requestId = parameters[0];
            var tokenHash = parameters[1];

            var response = new ClientSecretRequestDecodedOutput {
                RequestId = Guid.Parse(requestId),
                TokenHash = tokenHash
            };

            return response;
        }

        /// <summary>
        /// Encodes a request id and a token into a single string
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="token"></param>
        public string EncodeClientSecretRequest(Guid requestId, Guid token) {
            var tokenHash = hashProvider.ComputeHash256(token.ToString());
            var urlParameters = requestId + ":" + tokenHash;
            var urlParametersInBytes = Encoding.UTF8.GetBytes(urlParameters);
            var urlParametersEncoded = Convert.ToBase64String(urlParametersInBytes);

            return urlParametersEncoded;
        }

        /// <summary>
        /// Returns a clients phone number pulled from the client secret request id
        /// </summary>
        /// <param name="requestId"></param>
        public string GetClientPhoneNumberFromSecretRequestId(Guid requestId) {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IdentityServerDbContext>();

            var clientSecretRequest = db.ClientSecretRequests.FirstOrDefault(x => x.ClientSecretRequestId == requestId);
            if (clientSecretRequest == null) {
                throw new ArgumentException("Client Secret Request Id is invalid.");
            }

            var phoneNumberClaim = db.ClientClaims.FirstOrDefault(x => x.ClientId == clientSecretRequest.ClientId && x.Type == "phone_number");
            if (phoneNumberClaim == null) {
                throw new ArgumentException("Client Secret Request Id is invalid.");
            }

            return phoneNumberClaim.Value;
        }

        /// <summary>
        /// Returns a new secret key for a client
        /// </summary>
        /// <param name="requestId"></param>
        public string GetClientSecretKey(Guid requestId) {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IdentityServerDbContext>();

            var clientSecretRequest = db.ClientSecretRequests.FirstOrDefault(x => x.ClientSecretRequestId == requestId);
            if (clientSecretRequest == null) {
                throw new ArgumentException("Client Secret Request Id is invalid.");
            }

            var secretKey = GenerateSecretKey();
            var secretHash = hashProvider.ComputeHash256(secretKey);

            ClientSecret clientSecret = db.ClientSecrets.FirstOrDefault(x => x.ClientId == clientSecretRequest.ClientId);
            if (clientSecret == null) {
                db.ClientSecrets.Add(clientSecret = new ClientSecret {
                    ClientId = clientSecretRequest.ClientId,
                    Type = ParsedSecretTypes.SharedSecret,
                    Created = DateTime.UtcNow,
                    Value = secretHash
                });
            } else {
                clientSecret.Value = secretHash;
                clientSecret.Created = DateTime.UtcNow;
            }

            db.SaveChanges();

            return secretKey;
        }

        private string GenerateCode() {
            var random = new Random();
            return random.Next(0, 1000000).ToString("D6");
        }

        private string GenerateSecretKey() {
            var upperCaseChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            var lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
            var numberChars = "0123456789";
            var specialChars = "!@#$%^&*?_-";
            var random = new Random();

            var sb = new StringBuilder();

            int size = random.Next(16, 20);
            int i = 0;
            while (i < size) {
                sb.Append(upperCaseChars[random.Next(0, upperCaseChars.Length)]);
                i++;
                if (i >= size) {
                    break;
                }

                sb.Append(lowerCaseChars[random.Next(0, lowerCaseChars.Length)]);
                i++;
                if (i >= size) {
                    break;
                }

                sb.Append(numberChars[random.Next(0, numberChars.Length)]);
                i++;
                if (i >= size) {
                    break;
                }

                sb.Append(specialChars[random.Next(0, specialChars.Length)]);
                i++;
                if (i >= size) {
                    break;
                }
            }

            return new string(sb.ToString().ToCharArray().
                    OrderBy(s => (random.Next(2) % 2) == 0).ToArray());
        }
    }
}

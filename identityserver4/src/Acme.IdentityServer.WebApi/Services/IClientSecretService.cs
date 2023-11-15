using System;
using System.Threading.Tasks;
using Acme.IdentityServer.Data;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Models.Input;
using Acme.IdentityServer.WebApi.Models.Output;

namespace Acme.IdentityServer.WebApi.Services {
    public interface IClientSecretService {

        /// <summary>
        /// Will send the client a secret request to their email claim
        /// </summary>
        /// <param name="clientId"></param>
        Task SendClientSecretEmail(int clientId);

        /// <summary>
        /// Saves a new client secret request
        /// </summary>
        /// <param name="db"></param>
        /// <param name="clientId"></param>
        void SaveNewClientSecretRequest(IIdentityServerDbContext db, int clientId);

        /// <summary>
        /// Will reset the client secret to an invalid hash, until the user provides a new secret
        /// via the one time code workflow
        /// </summary>
        /// <param name="clientId"></param>
        Task<Client> ResetSecret(int clientId);

        /// <summary>
        /// Will send the client a verification code to their phone number claim
        /// </summary>
        /// <param name="clientSecretRequestId"></param>
        Task SendVerificationCode(Guid clientSecretRequestId, SendVerificationCodeModel sendVerificationCodeModel);

        /// <summary>
        /// Validates a client secret request
        /// </summary>
        /// <param name="encodedRequestId"></param>
        ValidateClientSecretRequestOutput ValidateClientSecretRequest(string encodedRequestId);

        /// <summary>
        /// Decodes an encoded request string
        /// </summary>
        /// <param name="encodedRequestId"></param>
        ClientSecretRequestDecodedOutput DecodeClientSecretRequest(string encodedRequestId);

        /// <summary>
        /// Encodes a request id and a token into a single string
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="token"></param>
        string EncodeClientSecretRequest(Guid requestId, Guid token);

        /// <summary>
        /// Validates verification code
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="verificationCode"></param>
        IsVerificationCodeValidOutput IsVerificationCodeValid(Guid requestId, string verificationCode);

        /// <summary>
        /// Returns a clients phone number pulled from the client secret request id
        /// </summary>
        /// <param name="requestId"></param>
        string GetClientPhoneNumberFromSecretRequestId(Guid requestId);

        /// <summary>
        /// Returns a new secret key for a client
        /// </summary>
        /// <param name="requestId"></param>
        string GetClientSecretKey(Guid requestId);
    }
}

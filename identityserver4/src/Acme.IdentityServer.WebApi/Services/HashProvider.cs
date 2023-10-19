using System;
using System.Security.Cryptography;
using System.Text;

namespace Acme.IdentityServer.WebApi.Services {
    public class HashProvider : IHashProvider {

        public string ComputeHash(string theString) {
            var x = SHA512.Create();
            var data = Encoding.ASCII.GetBytes(theString);
            data = x.ComputeHash(data);
            var hash = Convert.ToBase64String(data);

            return hash;
        }

        public string ComputeHash256(string input) {
            var hash = string.Empty;

            using (var shA256 = SHA256.Create()) {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                hash = Convert.ToBase64String((shA256).ComputeHash(bytes));
            }

            return hash;
        }

        public string GenerateSalt() {
            byte[] salt = new byte[32];
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider()) {
                provider.GetNonZeroBytes(salt);
                return Convert.ToBase64String(salt);
            }
        }
    }
}

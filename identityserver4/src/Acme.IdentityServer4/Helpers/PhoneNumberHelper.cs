using System.Text.RegularExpressions;

namespace EnerBank.IdentityServer.WebApi.Helpers {
    public class PhoneNumberHelper : IPhoneNumberHelper {

        public string FormatPhoneNumber(string phoneNumber) {
            return Regex.Replace(phoneNumber, @"^(\+)|\D", "$1");
        }
    }
}

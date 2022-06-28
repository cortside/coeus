﻿using System.Text.RegularExpressions;

namespace Cortside.IdentityServer.WebApi.Helpers {
    public class PhoneNumberHelper : IPhoneNumberHelper {

        public string FormatPhoneNumber(string phoneNumber) {
            return Regex.Replace(phoneNumber, @"^(\+)|\D", "$1");
        }
    }
}

// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.IdentityServer.Controllers.Account {
    public class LoginInputModel {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }
        public bool ValidatedLogin { get; set; } = false;
        public bool HasTwoFactorSetup { get; set; } = false;
        public string SetupCode { get; set; } = string.Empty;
        public string TwoFactorCode { get; set; } = "";
        public bool RememberMachine { get; set; } = false;
    }
}

// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;

namespace Acme.IdentityServer.WebApi.Controllers.Account {
    public static class AccountOptions {
        public readonly static bool AllowLocalLogin = true;

        public readonly static bool AllowRememberLogin = true;
        public readonly static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

        public readonly static bool ShowLogoutPrompt = false;
        public readonly static bool AutomaticRedirectAfterSignOut = true;

        // to enable windows authentication, the host (IIS or IIS Express) also must have 
        // windows auth enabled.
        public readonly static bool WindowsAuthenticationEnabled = false;
        public readonly static bool IncludeWindowsGroups = false;
        // specify the Windows authentication scheme and display name
        public readonly static string WindowsAuthenticationSchemeName = "Windows";

        public readonly static string InvalidCredentialsErrorMessage = "We do not recognize the username and/or password entered. Please try again.";
        public readonly static string EmptyFieldErrorMessage = "Please enter both a Username and Password";
        public readonly static string UserLockedErrorMessage = "Your account is locked. Please contact Contractor Support (888) 700-9610.";
    }
}

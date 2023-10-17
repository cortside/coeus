using System.IO.Abstractions;
using Cortside.Common.BootStrap;
using Acme.IdentityServer.WebApi.Assemblers;
using Acme.IdentityServer.WebApi.Assemblers.Implementors;
using Acme.IdentityServer.WebApi.Controllers.Account;
using Acme.IdentityServer.WebApi.Controllers.ResetClientSecretController;
using Acme.IdentityServer.WebApi.Helpers;
using Acme.IdentityServer.WebApi.Models;
using Acme.IdentityServer.WebApi.Services;
using Acme.IdentityServer.WebApi.Services.ExtensionGrantValidators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.IdentityServer.WebApi.Installers {
    public class DomainServiceInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfiguration configuration) {
            services.AddHttpClient<IGoogleRecaptchaV3Service, GoogleRecaptchaV3Service>();

            var build = configuration.GetSection("Build").Get<Build>();
            services.AddSingleton(build == null ? new Build() : build);

            services.AddSingleton<IHashProvider, HashProvider>();
            services.AddSingleton<IAuthenticator, Authenticator>();

            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IScopeService, ScopeService>();
            services.AddScoped<IClientSecretService, ClientSecretService>();
            services.AddScoped<IResetClientSecretService, ResetClientSecretService>();
            services.AddScoped<IUserModelAssembler, UserModelAssembler>();
            services.AddScoped<IFileSystem, FileSystem>();
            services.AddScoped<IPhoneNumberHelper, PhoneNumberHelper>();
            services.AddScoped<IAccountService, AccountService>();

            services.AddScoped<IUserService, UserService>();
        }
    }
}

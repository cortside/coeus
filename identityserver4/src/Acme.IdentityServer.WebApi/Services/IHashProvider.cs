namespace Acme.IdentityServer.WebApi.Services {
    public interface IHashProvider {
        string ComputeHash(string theString);
        string ComputeHash256(string input);
        string GenerateSalt();
    }
}

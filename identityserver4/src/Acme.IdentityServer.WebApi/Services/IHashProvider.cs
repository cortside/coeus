namespace Acme.IdentityServer.Services {
    public interface IHashProvider {
        string ComputeHash(string theString);
        string ComputeHash256(string input);
        string GenerateSalt();
    }
}

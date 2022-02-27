namespace Acme.ShoppingCart.Configuration {
    public class EncryptionConfiguration {
        public string Secret { get; set; }
        public string AesKey { get; set; }
        public string AesIv { get; set; }
    }
}

using System;
using System.IO;
using System.Security.Cryptography;
using System.Web;
using Acme.ShoppingCart.Configuration;
using Acme.ShoppingCart.Exceptions;
using Newtonsoft.Json;

namespace Acme.ShoppingCart.DomainService {
    public class EncryptionService : IEncryptionService {
        private byte[] AesKey;
        private byte[] AesIV;
        private readonly EncryptionConfiguration encryptionConfiguration;

        public EncryptionService(EncryptionConfiguration encryptionConfiguration) {
            this.encryptionConfiguration = encryptionConfiguration;
        }

        private AesManaged CryptoInit() {
            //Load the AES key and initialization vector from config
            var key = encryptionConfiguration.AesKey;
            if (key == null) {
                throw new ArgumentNullException("AesKey", "The system has not been configured for encryption.");
            }
            AesKey = Convert.FromBase64String(key);

            var iv = encryptionConfiguration.AesIv;
            if (iv == null) {
                throw new ArgumentNullException("AesIV", "Failed to initialize encryption.");
            }
            AesIV = Convert.FromBase64String(iv);

            var aesAlg = new AesManaged {
                Key = AesKey,
                IV = AesIV
            };

            return aesAlg;
        }

        public string EncryptString<T>(T objectToEncrypt) {
            string objectString = JsonConvert.SerializeObject(objectToEncrypt);
            string encryptedString = EncryptString(objectString);
            encryptedString = encryptedString.Replace("+", "%2B");
            return encryptedString;
        }

        private string EncryptString(string plainText) {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");

            byte[] encrypted;

            using (AesManaged aesAlg = CryptoInit()) {
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream()) {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        public T DecryptString<T>(string cipherText) {
            string decryptedString = DecryptString(cipherText);
            var response = JsonConvert.DeserializeObject<T>(decryptedString);
            if (response == null) {
                throw new BadRequestMessage("Unable to deserialize search string");
            }
            return response;
        }

        private string DecryptString(string cipherText) {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            string plaintext = null;
            if (cipherText.Contains("%")) {
                cipherText = HttpUtility.UrlDecode(cipherText);
            }
            //string cipherTextUrlDecoded =
            using (AesManaged aesAlg = CryptoInit()) {
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText))) {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt)) {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}

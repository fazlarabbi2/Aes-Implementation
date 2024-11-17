using System.Security.Cryptography;

namespace AESServerAPP.Models
{
    public class AesEncryptionService
    {
        // Method to encrypt a plaintext string using AES encryption with a client-specific key and IV.
        public static string EncryptString(string clientId, string plainText)
        {
            // Retrieve the AES key and IV using the clientId from a key management service.
            var client = KeyManagementService.GetKeyAndIV(clientId);

            // Throw an exception if no client configuration is found (invalid clientId).
            if (client == null)
            {
                throw new Exception("Invalid Client Id");
            }

            // Convert the base64-encoded key to a byte array.
            byte[] _key = Convert.FromBase64String(client.Key);

            // Convert the base64-encoded IV to a byte array.
            byte[] _iv = Convert.FromBase64String(client.IV);

            // Create a new instance of the AES algorithm.
            using (var aesAlg = Aes.Create())
            {
                // Set the key for the AES algorithm.
                aesAlg.Key = _key;

                // Set the IV for the AES algorithm.
                aesAlg.IV = _iv;

                // Create an encryptor from the AES instance to encrypt data.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                // MemoryStream is used to hold the encrypted bytes.
                using (var msEncrypt = new MemoryStream())
                {
                    // CryptoStream for cryptographic transformation of data.
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))

                    // StreamWriter to write the plaintext to the stream in a particular encoding.
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    // Convert the encrypted bytes from the memory stream to a base64 string.
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // Method to decrypt a ciphertext string using AES decryption with a client-specific key and IV.
        public static string DecryptString(string clientId, string cipherText)
        {
            // Retrieve the AES key and IV using the clientId from a key management service.
            var client = KeyManagementService.GetKeyAndIV(clientId);

            // Throw an exception if no client configuration is found (invalid clientId).
            if (client == null)
            {
                throw new Exception("Invalid Client id");
            }

            // Convert the base64-encoded key to a byte array.
            byte[] _key = Convert.FromBase64String(client.Key);
            // Convert the base64-encoded IV to a byte array.
            byte[] _iv = Convert.FromBase64String(client.IV);

            // Convert the base64-encoded ciphertext into a byte array.
            var buffer = Convert.FromBase64String(cipherText);

            // Create a new instance of the AES algorithm.
            using (var aesAlg = Aes.Create())
            {
                // Set the key for the AES algorithm.
                aesAlg.Key = _key;

                // Set the IV for the AES algorithm.
                aesAlg.IV = _iv;

                // Create a decryptor from the AES instance to decrypt data.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // MemoryStream is used to read the encrypted bytes.
                using (var msDecrypt = new MemoryStream(buffer))
                {
                    // CryptoStream for cryptographic transformation of data.
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))

                    // StreamReader to read the decrypted plaintext from the stream.
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted text from the crypto stream and return it.
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}

using System.Security.Cryptography;
using System.Text.Json;
using System.Text;

namespace AESClientAPP
{
    public class Program
    {
        // Base URL of the API server.
        private static readonly string BaseUrl = "https://localhost:7207/api/Employees"; // Base API URL
        private static readonly string ClientId = "Client1"; // Client identifier for AES encryption
        private static readonly string Key = "gi1D2eDd8Tg565ZbfRWc00j9xKtBka4ZHu0Sen+Drgc="; // Base64-encoded AES key
        private static readonly string IV = "Qb4nTgWS7UBo2YU7G/gJCg=="; // Base64-encoded AES IV

        // Asynchronous main function.
        static async Task Main(string[] args)
        {
            // HttpClient is used to make HTTP requests.
            using (HttpClient client = new HttpClient())
            {
                // Add the ClientId to the default request headers.
                client.DefaultRequestHeaders.Add("ClientId", ClientId);

                try
                {
                    // Fetching all employees data.
                    Console.WriteLine("Fetching all employees...");
                    HttpRequestMessage getAllRequest = new HttpRequestMessage(HttpMethod.Get, BaseUrl);
                    HttpResponseMessage getAllResponse = await client.SendAsync(getAllRequest);
                    string encryptedGetAllResponse = await getAllResponse.Content.ReadAsStringAsync();
                    string decryptedGetAllResponse = DecryptString(encryptedGetAllResponse);
                    Console.WriteLine("Decrypted response (All Employees):");
                    Console.WriteLine(decryptedGetAllResponse);
                    List<Employee> employees = JsonSerializer.Deserialize<List<Employee>>(decryptedGetAllResponse);
                    foreach (var emp in employees)
                    {
                        Console.WriteLine($"Employee Details - ID: {emp.Id}, Name: {emp.Name}, Salary: {emp.Salary}");
                    }

                    // Fetching a specific employee by ID.
                    Console.WriteLine("\nFetching employee with ID 1...");
                    HttpRequestMessage getOneRequest = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/1");
                    HttpResponseMessage getOneResponse = await client.SendAsync(getOneRequest);
                    string encryptedGetOneResponse = await getOneResponse.Content.ReadAsStringAsync();
                    string decryptedGetOneResponse = DecryptString(encryptedGetOneResponse);
                    Console.WriteLine("Decrypted response (Employee ID 1):");
                    Console.WriteLine(decryptedGetOneResponse);
                    Employee employee = JsonSerializer.Deserialize<Employee>(decryptedGetOneResponse);
                    Console.WriteLine($"Employee Details - ID: {employee.Id}, Name: {employee.Name}, Salary: {employee.Salary}");

                    // Adding a new employee.
                    Console.WriteLine("\nAdding a new employee...");
                    var newEmployee = new Employee { Name = "David Brown", Salary = 65000 };
                    var encryptedEmployee = EncryptString(JsonSerializer.Serialize(newEmployee));
                    string jsonPayload = $"\"{encryptedEmployee}\"";
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, BaseUrl) { Content = content };
                    HttpResponseMessage postResponse = await client.SendAsync(postRequest);
                    string encryptedPostResponse = await postResponse.Content.ReadAsStringAsync();
                    string decryptedPostResponse = DecryptString(encryptedPostResponse);
                    Console.WriteLine("Decrypted response (New Employee):");
                    Console.WriteLine(decryptedPostResponse);

                    // Updating an existing employee.
                    Console.WriteLine("\nUpdating employee with ID 1...");
                    var updatedEmployee = new Employee { Name = "David Brown Updated", Salary = 70000 };
                    encryptedEmployee = EncryptString(JsonSerializer.Serialize(updatedEmployee));
                    jsonPayload = $"\"{encryptedEmployee}\"";
                    content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    HttpRequestMessage putRequest = new HttpRequestMessage(HttpMethod.Put, $"{BaseUrl}/1") { Content = content };
                    HttpResponseMessage putResponse = await client.SendAsync(putRequest);
                    Console.WriteLine(putResponse.IsSuccessStatusCode ? "Update successful" : "Update failed");

                    // Deleting an employee.
                    Console.WriteLine("\nDeleting employee with ID 2...");
                    HttpRequestMessage deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"{BaseUrl}/2");
                    HttpResponseMessage deleteResponse = await client.SendAsync(deleteRequest);
                    Console.WriteLine(deleteResponse.IsSuccessStatusCode ? "Deletion successful" : "Deletion failed");
                }
                catch (Exception ex)
                {
                    // Handle and report any errors that occur during the HTTP requests.
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            // Wait for a key press before closing the console window.
            Console.ReadKey();
        }

        // Encrypts a plaintext string using AES and returns the base64-encoded cipher text.
        private static string EncryptString(string plainText)
        {
            // Convert the base64-encoded key into a byte array.
            byte[] _key = Convert.FromBase64String(Key);

            // Convert the base64-encoded IV into a byte array.
            byte[] _iv = Convert.FromBase64String(IV);

            // Create a new AES instance to perform the symmetric algorithm.
            using (var aesAlg = Aes.Create())
            {
                // Assign the encryption key to the AES algorithm.
                aesAlg.Key = _key;

                // Assign the initialization vector to the AES algorithm.
                aesAlg.IV = _iv;

                // Create an encryptor object to transform the data.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create a memory stream to hold the encrypted data.
                using (var msEncrypt = new MemoryStream())
                {
                    // Create a cryptographic stream that encrypts data and writes it to the memory stream.
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        // Create a stream writer to write the plain text data to the crypto stream.
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        // Convert the encrypted data from the memory stream to a base64 string.
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        // Decrypts a base64-encoded cipher text string using AES and returns the plaintext.
        private static string DecryptString(string cipherText)
        {
            // Convert the base64-encoded key into a byte array.
            byte[] _key = Convert.FromBase64String(Key);

            // Convert the base64-encoded IV into a byte array.
            byte[] _iv = Convert.FromBase64String(IV);

            // Convert the base64-encoded cipher text into a byte array.
            var buffer = Convert.FromBase64String(cipherText);

            // Create a new AES instance to perform the symmetric algorithm.
            using (var aesAlg = Aes.Create())
            {
                // Assign the decryption key to the AES algorithm.
                aesAlg.Key = _key;

                // Assign the initialization vector to the AES algorithm.
                aesAlg.IV = _iv;

                // Create a decryptor object to transform the encrypted data back into plain text.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create a memory stream to read the encrypted data.
                using (var msDecrypt = new MemoryStream(buffer))
                {
                    // Create a cryptographic stream that reads and decrypts data from the memory stream.
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        // Create a stream reader to read the decrypted plain text data from the crypto stream.
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Return the decrypted text read from the stream reader.
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        // Definition of the Employee class used to serialize and deserialize employee data.
        public class Employee
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Salary { get; set; }
        }
    }
}

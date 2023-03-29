using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Unicode;
using System.Xml.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Input string
        Console.Write("Enter text to encrypt or decrypt or File path: ");
        string input = Console.ReadLine();

        // Mode for encryption
        Console.WriteLine("Select mode :\n1. ECB\n2. CBC\n3. CFB\n4. CTS\n5. CFB");
        int modeOption = int.Parse(Console.ReadLine());
        string mode = string.Empty;
        switch (modeOption)
        {
            case 1:
                mode = "ECB";
                break;
            case 2:
                mode = "CBC";
                break;
            case 3:
                mode = "CFB";
                break;
            case 4:
                mode = "CTS";
                break;
            case 5:
                mode = "CFB";
                break;
            default:
                Console.WriteLine("Invalid mode selected.");
                break;
        }

        // Encryption/Decryption
        Console.WriteLine("Select an option:\n1. Encrypt\n2. Decrypt");
        int option = int.Parse(Console.ReadLine());

        switch (option)
        {
            case 1:
                // Encrypt the input string
                using (Aes myAes = Aes.Create())
                {
                    myAes.Mode = (CipherMode)Enum.Parse(typeof(CipherMode), mode);
                    byte[] encrypted = EncryptStringToBytes_Aes(input, myAes.Key, myAes.IV, myAes.Mode);
                    string encryptedString = Convert.ToBase64String(encrypted);

                    Console.WriteLine("Would you like to save info to file?:\n1.Yes\n2.No");
                    var saveToFileOption = int.Parse(Console.ReadLine());
                    switch (saveToFileOption)
                    {
                        case 1:
                            var path = WriteToFile(input,
                                    Convert.ToBase64String(myAes.Key),
                                    Convert.ToBase64String(myAes.IV),
                                    Convert.ToBase64String(encrypted));
                            Console.WriteLine("Info saved to file: " + path);
                            break;
                        case 2:
                            Console.WriteLine("Key: {0}", Convert.ToBase64String(myAes.Key));
                            Console.WriteLine("Iv: {0}", Convert.ToBase64String(myAes.IV));
                            Console.WriteLine("Encrypted text: {0}", encryptedString);
                            break;
                        default:
                            break;
                    }
                }
                break;
            case 2:
                Console.WriteLine("Would you like decypt from file or input text to terminal?:\n1.File\n2.Terminal");
                var fileTerminalOption = int.Parse(Console.ReadLine());

                switch (fileTerminalOption)
                {
                    case 1:
                        JsonDataObject fileValues = ParseJson(input);
                        // Decrypt the input string from file
                        using (Aes myAes = Aes.Create())
                        {
                            myAes.Mode = (CipherMode)Enum.Parse(typeof(CipherMode), mode);
                            var key = Convert.FromBase64String(fileValues.Key);

                            var iv = Convert.FromBase64String(fileValues.Iv);

                            byte[] encryptedBytes = Convert.FromBase64String(fileValues.Encypted);
                            string decryptedString = DecryptStringFromBytes_Aes(encryptedBytes, key, iv, myAes.Mode);
                            Console.Write("Decrypted text: {0}", decryptedString);
                        }

                        break;
                    case 2:
                        // Decrypt the input string
                        using (Aes myAes = Aes.Create())
                        {
                            myAes.Mode = (CipherMode)Enum.Parse(typeof(CipherMode), mode);
                            Console.Write("Enter the encryption key: ");
                            var key = Convert.FromBase64String(Console.ReadLine());

                            Console.Write("Enter the initialization vector (IV): ");
                            var iv = Convert.FromBase64String(Console.ReadLine());

                            byte[] encryptedBytes = Convert.FromBase64String(input);
                            string decryptedString = DecryptStringFromBytes_Aes(encryptedBytes, key, iv, myAes.Mode);
                            Console.Write("Decrypted text: {0}", decryptedString);
                        }
                        break;
                    default:
                        break;
                }
                break;
            default:
                Console.WriteLine("Invalid option selected.");
                break;
        }


        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV, CipherMode mode)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Mode = mode;
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }

            }
            return encrypted;
        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV, CipherMode mode)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Mode = mode;
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        static string WriteToFile(string text, string key, string iv, string encypted)
        {
            //Create my object
            JsonDataObject myData = new JsonDataObject()
            {
                Key = key,
                Iv = iv,
                Encypted = encypted
            };

            //Tranform it to Json object
            string jsonData = JsonConvert.SerializeObject(myData);

            // Set a variable to the Documents path.
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Write the specified text asynchronously to a new file
            var file = DateTime.Now.Ticks + ".json";
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, file)))
            {
                outputFile.Write(jsonData);
                return docPath + "\\" + file;
            }
        }

        static JsonDataObject ParseJson(string filePath)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filename = Path.Combine(filePath);
            if (File.Exists(filename))
            {
                var text = File.ReadAllText(filename);


                //Parse the json object
                var values = JsonArray.Parse(text);
                //Print the parsed Json object
                Console.WriteLine("Key: {0}", values["Key"]);
                Console.WriteLine("Iv: {0}",values["Iv"]);
                Console.WriteLine("Encrypted Text: {0}", values["Encypted"]);
                //Create my object
                JsonDataObject jsonData = new JsonDataObject()
                {
                    Key = (string)values["Key"],
                    Iv = (string)values["Iv"],
                    Encypted = (string)values["Encypted"]
                };
                return jsonData;
            }
            return null;
        }

    }

    public class JsonDataObject
    {
        public string? Key;
        public string? Iv;
        public string? Encypted;
    }
}






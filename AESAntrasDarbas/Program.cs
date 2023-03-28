using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;

class Program
{
    static void Main(string[] args)
    {
        // Input string
        Console.WriteLine("Enter text to encrypt or decrypt:");
        string input = Console.ReadLine();
        /*string input = "B8hlaumyJLHZRq6494xXrg==";*/

        // Mode for encryption
        Console.WriteLine("Select mode :");
        Console.WriteLine("1. ECB");
        Console.WriteLine("2. CBC");
        Console.WriteLine("3. CFB");
        Console.WriteLine("4. CTS");
        Console.WriteLine("5. CFB");
        int modeOption = int.Parse(Console.ReadLine());
        string mode = string.Empty;
        /*mode = "ECB"; // Change to "ECB", "CBC", "CFB", "CTS", or "CFB" for different modes*/
        switch (modeOption)
        {
            case 1: mode = "ECB";
                break;
            case 2: mode = "CBC";
                break;
            case 3: mode = "CFB";
                break;
            case 4: mode = "CTS";
                break;
            case 5: mode = "CFB";
                break;
            default:
                Console.WriteLine("Invalid mode selected.");
                break;
        }

        // Encryption
        Console.WriteLine("Select an option:");
        Console.WriteLine("1. Encrypt");
        Console.WriteLine("2. Decrypt");
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
                    Console.WriteLine("Encrypted text: {0}", encryptedString);
                }
                break;
            case 2:
                // Decrypt the input string
                using (Aes myAes = Aes.Create())
                {
                    myAes.Mode = (CipherMode)Enum.Parse(typeof(CipherMode), mode);
                    /*Console.WriteLine("Enter the encryption key:");
                    string key = Console.ReadLine();*/
                    var key = Convert.FromBase64String("lY191fiIRcv3URfoGAy1TOPUDQIznZQPMT8yLCAkWM0=");

                    /*Console.WriteLine("Enter the initialization vector (IV):");
                    string iv = Console.ReadLine();*/
                    var iv = Convert.FromBase64String("9h0wlzq/GvBUXwyt4mliow==");

                    byte[] encryptedBytes = Convert.FromBase64String(input);
                    string decryptedString = DecryptStringFromBytes_Aes(encryptedBytes, key, iv, myAes.Mode);
                    Console.WriteLine("Decrypted text: {0}", decryptedString);
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

                // TODO: add logic for user to select if he wants to write to the file
                // TODO: format to json
                WriteToFile(Convert.ToBase64String(aesAlg.Key) + " " + Convert.ToBase64String(aesAlg.IV));

                Console.WriteLine("Key: {0}", Convert.ToBase64String(aesAlg.Key));
                Console.WriteLine("IV: {0}", Convert.ToBase64String(aesAlg.IV));

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

        static async void WriteToFile(string text)
        {
            // Set a variable to the Documents path.
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Write the specified text asynchronously to a new file
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "WriteTextAsync.txt")))
            {
                await outputFile.WriteAsync(text);
            }
        }
    }
}






using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;

class Program
{
    static void Main(string[] args)
    {

        // Menu

        Console.WriteLine("Enter text to encrypt or decrypt:");
        /*        string input = Console.ReadLine();*/

        string input = "B8hlaumyJLHZRq6494xXrg==";


        string mode = "ECB"; // Change to "ECB", "CBC", "CFB", or "CTR" for different modes

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
                    var key = Convert.FromBase64String("5m0wa687iDyCN8GYgJFO3lFuXhxFvPAk5tt4iGKayTY=");

                    /*Console.WriteLine("Enter the initialization vector (IV):");
                    string iv = Console.ReadLine();*/
                    var iv = Convert.FromBase64String("QJz4fid0UjorpgYVCIqakA==");

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
    }
}






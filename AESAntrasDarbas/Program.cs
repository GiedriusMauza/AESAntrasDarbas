using System.IO;
using System.Security.Cryptography;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter text/cipher value: ");
        String value = Console.ReadLine();

        Console.Write("Enter key value: ");
        String key = Console.ReadLine();

        Console.Write("Would you like to: \n 1. Cipher \n 2. Decipher\n");
        String optionOne = Console.ReadLine();

        switch (optionOne)
        {
            case "1":
                Console.WriteLine("Selected to cipher");
                EncryptWithAES();
                break;
            case "2":
                Console.WriteLine("Selected to decipher");
                DecryptWithAES();
                break;
        }





    }

    private static void DecryptWithAESCFB()
    {
        return;
    }

    private static void DecryptWithAESCBC()
    {
        return;
    }

    private static void EncryptWithAESECB()
    {
        return;
    }

    private static void DecryptWithAES()
    {
        Console.WriteLine("Decrypted Text");
    }

    private static void EncryptWithAES()
    {
        Console.WriteLine();
        Console.Write("Select cipher mode: \n 1. ECB \n 2. CBC\n 3. CFB \n");
        String optionTwo = Console.ReadLine();

        switch (optionTwo)
        {
            case "1":
                Console.WriteLine("Selected to ECB");
                EncryptWithAESECB();
                break;
            case "2":
                Console.WriteLine("Selected to CBC");
                DecryptWithAESCBC();
                break;
            case "3":
                Console.WriteLine("Selected to CFB");
                DecryptWithAESCFB();
                break;
        }
    }
}






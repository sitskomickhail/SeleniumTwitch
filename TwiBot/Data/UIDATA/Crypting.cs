using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TwiBot.Data.UIDATA
{
    public static class Crypting
    {
        public static void CryptDataInFile(string lKey)
        {
            var cryptAlgorithm = new TripleDESCryptoServiceProvider();
            using (var file = new FileStream($@"{Environment.CurrentDirectory}\crypted.txt", FileMode.Create))
            {
                using (var cryptoStream = new CryptoStream(file, cryptAlgorithm.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (var streamWriter = new StreamWriter(cryptoStream, Encoding.Default))
                    {
                        foreach (var line in lKey)
                        {
                            streamWriter.WriteLine(line);
                        }
                    }
                }
            }

            using (var fileKey = new FileStream($@"{Environment.CurrentDirectory}\cryptLKey.key", FileMode.Create))
            {
                using (var binaryWriter = new BinaryWriter(fileKey))
                {
                    binaryWriter.Write(cryptAlgorithm.IV);
                    binaryWriter.Write(cryptAlgorithm.Key);
                }
            }
        }

        public static string DeCryptData()
        {
            var cryptAlgorithm = new TripleDESCryptoServiceProvider();

            try
            {
                using (FileStream fileKey = new FileStream($@"{Environment.CurrentDirectory}\cryptLKey.key", FileMode.Open))
                {
                    using (BinaryReader binaryReader = new BinaryReader(fileKey))
                    {
                        cryptAlgorithm.IV = binaryReader.ReadBytes(8);
                        cryptAlgorithm.Key = binaryReader.ReadBytes(24);
                    }
                }

                using (var file = new FileStream($@"{Environment.CurrentDirectory}\crypted.txt", FileMode.Open))
                {
                    using (var cryptoStream = new CryptoStream(file, cryptAlgorithm.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (var streamReader = new StreamReader(cryptoStream, Encoding.Default))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch { return null; }
        }
    }
}
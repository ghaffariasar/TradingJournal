namespace TradingProject.Helpers
{

    using System;
    using System.IO;
    using System.Security.Cryptography;

    public class CryptoHelper
    {
        private static readonly byte[] Key;

        static CryptoHelper()
        {
            using var sha256 = SHA256.Create();
            Key = sha256.ComputeHash("ForexTradingSecretKey"u8.ToArray());
        }

        public static string Encrypt(string plainText)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = Key;
            aesAlg.GenerateIV(); // IV تصادفی برای امنیت بیشتر

            using var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using var msEncrypt = new MemoryStream();
            msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length); // ذخیره IV در اول متن رمز شده

            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            return Convert.ToBase64String(msEncrypt.ToArray()); // تبدیل خروجی به Base64
        }

        public static string Decrypt(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            using var aesAlg = Aes.Create();
            aesAlg.Key = Key;

            var iv = new byte[aesAlg.BlockSize / 8];
            var cipherBytes = new byte[fullCipher.Length - iv.Length];

            Array.Copy(fullCipher, iv, iv.Length);
            Array.Copy(fullCipher, iv.Length, cipherBytes, 0, cipherBytes.Length);

            aesAlg.IV = iv;

            using var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using var msDecrypt = new MemoryStream(cipherBytes);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
    }



}
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MI.Library.Utils
{
    /// <summary>
    ///     Aes加解密算法工具类
    /// </summary>
    public static class AesUtils
    {
        private static readonly byte[] Iv = Encoding.UTF8.GetBytes("-o@g*m,%0!si^fo1");
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("th!s!s@p@ssw0rd;setoae$12138!@$@");
        private const CipherMode DefaultMode = CipherMode.CFB;

        /// <summary>
        ///     Aes加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Encrypt(string str)
        {
            var encrypted = EncryptStringToBytes(str,
                Key, Iv, DefaultMode);
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        ///     Aes加密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string Encrypt(string str, byte[] key, byte[] iv,
            CipherMode mode = DefaultMode)
        {
            var encrypted = EncryptStringToBytes(str,
                key, iv, mode);
            return Convert.ToBase64String(encrypted);
        }

        public static string Encrypt(string str, string key, string iv,
            CipherMode mode = DefaultMode)
        {
            return Encrypt(str,
                Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv), mode);
        }

        /// <summary>
        ///     Aes解密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Descrypt(string str)
        {
            return DecryptStringFromBytes(Convert.FromBase64String(str), Key, Iv, DefaultMode);
        }

        /// <summary>
        ///     Aes解密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string Descrypt(string str, byte[] key, byte[] iv,
            CipherMode mode = DefaultMode)
        {
            return DecryptStringFromBytes(Convert.FromBase64String(str), key, iv, mode);
        }

        /// <summary>
        ///     Aes解密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string Descrypt(string str, string key, string iv,
            CipherMode mode = DefaultMode)
        {
            return Descrypt(str, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv), mode);
        }

        static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv, CipherMode mode)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            var aesCipher = CipherUtilities.GetCipher($"AES/{mode.ToString()}/PKCS7Padding");
            aesCipher.Init(true, new ParametersWithIV(new KeyParameter(key), iv));
            var inputBytes = Encoding.UTF8.GetBytes(plainText);
            var finallyBytes = aesCipher.DoFinal(inputBytes, 0, inputBytes.Length);

            aesCipher.DoFinal(finallyBytes, 0, inputBytes.Length);
            return finallyBytes;
        }

        static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv, CipherMode mode)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            var aesCipher = CipherUtilities.GetCipher($"AES/{mode.ToString()}/PKCS7Padding");
            aesCipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));
            var finallyBytes = aesCipher.DoFinal(cipherText, 0, cipherText.Length);
            return Encoding.UTF8.GetString(finallyBytes);
        }
    }
}

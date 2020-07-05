using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.IO;
using System.Text;

namespace MI.Library.Utils
{
    public enum RsaKeyType
    {
        PublicKey,
        PrivateKey
    }

    /// <summary>
    ///     Rsa加解密工具类
    /// </summary>
    public static class RsaUtils
    {
        private const string DefaultPrivateKey = @"MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAMfAc4xrWGefEjYp39L5fqFLnftHtsxiMufyP76NRM8v7b7eqd7v4Jru22zv6sa5gB04Yvpp1FGn+Tz03UPnnDlXvTjOSb3eOGQhXepOuqFBR9D4xykodXNvA7ceGK6JOegmI+pQGzHlVK2HuC/d+8wvYxFMeI/7YADz50N04ihHAgMBAAECgYAvc/+/Pw0caMS5y07Z3t1/Uehw9oNtoHJ5eao9CXBsS/WN33W5eYEBLXdBNOmwVgciae/Rj2yaDW5/Vahu5knNRhHlIZDh0IAX3Vp5+DZ0fq59VqBeLMgZ2f+N5wiOJ2+1HSfx1u5+jbS3pE0cZYOg7pR6jxWG+b0c40GMsMzoQQJBAOocfsD7FWa0RhAPnFOQtVvDVRaG2HN7pbCQeCT8fqEePZemVGNtreElMUt5qbx0qoc8F6Xa2YzMQrW0dvWGOqcCQQDabY2VLMu7DHkBkHtrBJ4FROHgnJzaCJu9Rqu7wQZBwdiMuu8txRY9c6bniMkCsmsanwXTZ7rAhvkeeQs863lhAkEAs5E/uA3ekHRd+RvAMGiicswUi77Kb2m74P4u6U+yYSqs25D80XbjE/pPITEkfCSQWEJDcTe3/kL+OBk/1XsrqQJAUrPUPb0+Tk5EqtD3yedvpXMVSyRBR1SnEx1k/KvzIIay5WYKFXxgFVhqw5PI+Bpx7xxy6j6GOUthm6YdGS3XQQJAL4IdkRDKxSPiYiw/dGPW2RXGIFRDz2nCQ0+f0VZ9y8lL1pY4xBSDFMpnXEiMbFS8QJ0iNy277GIxCIPrFDE6uA==";
        private const string DefaultPublicKey = @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDHwHOMa1hnnxI2Kd/S+X6hS537R7bMYjLn8j++jUTPL+2+3qne7+Ca7tts7+rGuYAdOGL6adRRp/k89N1D55w5V704zkm93jhkIV3qTrqhQUfQ+McpKHVzbwO3HhiuiTnoJiPqUBsx5VSth7gv3fvML2MRTHiP+2AA8+dDdOIoRwIDAQAB";
        /// <summary>
        ///     RSA加密.
        /// </summary>
        /// <param name="str">明文</param>
        /// <param name="type">密钥类型。若为公钥，则使用默认公钥；若为私钥，则使用默认私钥</param>
        /// <returns>密文</returns>
        public static string Encrypt(string str, RsaKeyType type = RsaKeyType.PublicKey)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            switch (type)
            {
                case RsaKeyType.PublicKey:
                    return Encrypt(str, DefaultPublicKey, type);
                case RsaKeyType.PrivateKey:
                    return Encrypt(str, DefaultPrivateKey, type);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        ///     RSA加密.
        /// </summary>
        /// <param name="str">明文</param>
        /// <param name="key">密钥</param>
        /// <param name="type">密钥类型。若为公钥，则执行公钥加密；否则执行私钥加密</param>
        /// <returns>密文</returns>
        public static string Encrypt(string str, string key, RsaKeyType type = RsaKeyType.PublicKey)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }


            var engine = new Pkcs1Encoding(new RsaEngine());

            RsaKeyParameters keyParam;

            switch (type)
            {
                case RsaKeyType.PublicKey:
                    keyParam = GetPublicKeyParameter(key);
                    break;
                case RsaKeyType.PrivateKey:
                    keyParam = GetPrivateKeyParameter(key);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            engine.Init(true, keyParam);
            var strBytes = Encoding.UTF8.GetBytes(str);

            var maxEncryptBlock = keyParam.Modulus.BitLength / 8 - 11;

            byte[] cipherBytes;
            using (var stream = new MemoryStream())
            {
                for (int i = 1, offset = 0; strBytes.Length - offset > 0; i++)
                {
                    var bytes = strBytes.Length - offset > maxEncryptBlock ? engine.ProcessBlock(strBytes, offset, maxEncryptBlock) : engine.ProcessBlock(strBytes, offset, strBytes.Length - offset);
                    stream.Write(bytes, 0, bytes.Length);
                    offset = i * maxEncryptBlock;
                }

                cipherBytes = stream.ToArray();
            }

            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        ///     RSA解密. Base64密文
        /// </summary>
        /// <param name="base64String">密文</param>
        /// <param name="type">密钥类型. 若为私钥，则执行私钥解密；若为公钥，则执行公钥解密.</param>
        /// <returns>明文</returns>
        public static string Decrypt(string base64String, RsaKeyType type = RsaKeyType.PrivateKey)
        {
            switch (type)
            {
                case RsaKeyType.PublicKey:
                    return Decrypt(base64String, DefaultPublicKey, type);
                case RsaKeyType.PrivateKey:
                    return Decrypt(base64String, DefaultPrivateKey, type);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        ///     RSA解密. Base64密文
        /// </summary>
        /// <param name="base64String">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="type">密钥类型. 若为私钥，则执行私钥解密；若为公钥，则执行公钥解密.</param>
        /// <returns>明文</returns>
        public static string Decrypt(string base64String, string key, RsaKeyType type = RsaKeyType.PrivateKey)
        {
            var cipherBytes = Convert.FromBase64String(base64String);

            var plainBytes = Decrypt(cipherBytes, key, type);

            return Encoding.UTF8.GetString(plainBytes);
        }

        /// <summary>
        ///     RSA解密. 16进制密文
        /// </summary>
        /// <param name="hexString">密文</param>
        /// <param name="type">密钥类型</param>
        /// <returns>明文</returns>
        public static string DecryptHexString(string hexString, RsaKeyType type = RsaKeyType.PrivateKey)
        {
            var bytes = DecryptHexStringToBytes(hexString, type);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        ///     RSA解密. 16进制密文
        /// </summary>
        /// <param name="hexString">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="type">密钥类型</param>
        /// <returns>明文</returns>
        public static string DecryptHexString(string hexString, string key, RsaKeyType type = RsaKeyType.PrivateKey)
        {
            var bytes = DecryptHexStringToBytes(hexString, key, type);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        ///     RSA解密. 16进制密文
        /// </summary>
        /// <param name="hexString">密文</param>
        /// <param name="type">密钥类型. 若为私钥，则执行私钥解密；若为公钥，则执行公钥解密.</param>
        /// <returns>表示明文的字节数组</returns>
        public static byte[] DecryptHexStringToBytes(string hexString, RsaKeyType type = RsaKeyType.PrivateKey)
        {
            switch (type)
            {
                case RsaKeyType.PublicKey:
                    return DecryptHexStringToBytes(hexString, DefaultPublicKey, type);
                case RsaKeyType.PrivateKey:
                    return DecryptHexStringToBytes(hexString, DefaultPrivateKey, type);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        ///     RSA解密. 16进制密文
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte[] DecryptHexStringToBytes(string hexString, string key, RsaKeyType type = RsaKeyType.PrivateKey)
        {
            var cipherBytes = TextEncodeUtils.HexStringToBytes(hexString);
            return Decrypt(cipherBytes, key, type);
        }

        /// <summary>
        ///     RSA解密.
        /// </summary>
        /// <param name="inputBytes">表示密文的字节数组</param>
        /// <param name="key">密钥</param>
        /// <param name="type">表示明文的字节数组</param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] inputBytes, string key, RsaKeyType type = RsaKeyType.PrivateKey)
        {
            if (inputBytes == null || inputBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(inputBytes));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var engine = new Pkcs1Encoding(new RsaEngine());
            RsaKeyParameters keyParam;
            switch (type)
            {
                case RsaKeyType.PublicKey:
                    keyParam = GetPublicKeyParameter(key);
                    break;
                case RsaKeyType.PrivateKey:
                    keyParam = GetPrivateKeyParameter(key);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            engine.Init(false, keyParam);
            var maxDecryptBlock = keyParam.Modulus.BitLength / 8;

            byte[] plainBytes;
            using (var stream = new MemoryStream())
            {
                for (int i = 1, offset = 0; inputBytes.Length - offset > 0; i++)
                {
                    var bytes = inputBytes.Length - offset > maxDecryptBlock ? engine.ProcessBlock(inputBytes, offset, maxDecryptBlock) : engine.ProcessBlock(inputBytes, offset, inputBytes.Length - offset);
                    stream.Write(bytes, 0, bytes.Length);
                    offset = i * maxDecryptBlock;
                }

                plainBytes = stream.ToArray();
            }

            return plainBytes;
        }

        private static RsaKeyParameters GetPublicKeyParameter(string publicKey)
        {
            var publicKeyBytes = Convert.FromBase64String(publicKey);

            return (RsaKeyParameters)PublicKeyFactory.CreateKey(publicKeyBytes);
        }

        private static RsaKeyParameters GetPrivateKeyParameter(string privateKey)
        {
            var privateKeyBytes = Convert.FromBase64String(privateKey);

            return (RsaKeyParameters)PrivateKeyFactory.CreateKey(privateKeyBytes);
        }

        public static (string publicKey, string privateKey) CreateRsaKeyPair()
        {
            var generator = new RsaKeyPairGenerator();

            generator.Init(new RsaKeyGenerationParameters(
                BigInteger.Three,
                new SecureRandom(),
                1024,
                25));

            var keyPair = generator.GenerateKeyPair();
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public);
            var publicKeyBytes = publicKeyInfo.GetEncoded("UTF-8");
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);
            var privateKeyBytes = privateKeyInfo.GetEncoded("UTF-8");

            var publicKey = Convert.ToBase64String(publicKeyBytes);
            var privateKey = Convert.ToBase64String(privateKeyBytes);

            return (publicKey, privateKey);
        }
    }
}

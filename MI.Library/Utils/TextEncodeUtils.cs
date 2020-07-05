using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MI.Library.Utils
{
    public static class TextEncodeUtils
    {
        public static string Encode(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            return Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public static byte[] Decode(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            return Convert.FromBase64String(Pad(text.Replace('-', '+').Replace('_', '/')));
        }

        /// <summary>
        ///     16进制转为字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string hexString)
        {
            var regex = new Regex(@"[A-Fa-f0-9]+$");
            if (!regex.IsMatch(hexString))
                throw new ArgumentException("不是合法的16进制字符串");

            // 处理干扰，例如空格和 '-' 符号。
            var str = hexString.Replace("-", string.Empty).Replace(" ", string.Empty);

            if (str.Length % 2 != 0)
                throw new ArgumentException("字符串长度不正确");

            // 构建一个字符串长度的序列，每隔 2 个字符长度，即使用 Convert 构成一个字节。
            return Enumerable.Range(0, str.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(str.Substring(x, 2), 16))
                .ToArray();
        }

        private static string Pad(string text)
        {
            int count = 3 - (text.Length + 3) % 4;
            if (count == 0)
                return text;
            return text + new string('=', count);
        }
    }
}

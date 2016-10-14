using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QCloud.WeApp.SDK
{
    internal static class Signature
    {
        public static string ComputeSignature(this string input)
        {
            var skey = ConfigurationManager.CurrentConfiguration.TunnelSignatureKey;
            return (input + skey).HashSha1();
        }

        public static bool CheckSignature(this string input, string signature)
        {
            return input.ComputeSignature() == signature;
        }

        public static string HashSha1(this string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                return input.Hash(sha1);
            }
        }

        public static string HashMd5(this string input)
        {
            using (var md5 = MD5.Create())
            {
                return input.Hash(md5);
            }
        }

        public static string Hash(this string input, HashAlgorithm algorithm)
        {
            var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            var builder = new StringBuilder();
            foreach (byte b in hash)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}

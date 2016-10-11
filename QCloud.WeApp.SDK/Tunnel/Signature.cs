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
            var skey = ConfigurationManager.CurrentConfiguration.SecretKey;
            return Sha1Hash(input + skey);
        }

        public static bool CheckSignature(this string input, string signature)
        {
            return input.ComputeSignature() == signature;
        }

        private static string Sha1Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var builder = new StringBuilder();
                foreach (byte b in hash)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}

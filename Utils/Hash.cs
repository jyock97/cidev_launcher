using System.Security.Cryptography;
using System.Text;


namespace cidev_launcher.Utils
{
    public class Hash
    {
        private static HashAlgorithm hashAlgorithm = SHA256.Create();

        static byte[] GetHash(string inputString)
        {
            return hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}

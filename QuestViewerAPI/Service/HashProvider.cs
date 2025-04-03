using System.Security.Cryptography;
using System.Text;

namespace QuestGraph.Core
{
    public class HashProvider
    {
        public static string GetHashString(string dataBaseUrl, string databaseName, string documentId)
        {
            var docAbsoluteName = DocAbsoluteName(dataBaseUrl, databaseName, documentId);
            return GetHashString(docAbsoluteName);
        }


        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private static string DocAbsoluteName(string dataBaseUrl, string databaseName, string documentId)
        {
            return $"{dataBaseUrl}{databaseName}{documentId}";
        }
    }
}
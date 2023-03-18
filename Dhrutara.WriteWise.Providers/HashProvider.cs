using System.Security.Cryptography;
using System.Text;

namespace Dhrutara.WriteWise.Providers
{
    public class HashProvider : IHashProvider
    {
        public string ComputeSha256Hash(string text)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));
                StringBuilder builder = new();
                for (int i = 0; i < bytes.Length; i++)
                {
                    _ = builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}

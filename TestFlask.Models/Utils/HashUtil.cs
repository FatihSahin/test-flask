using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TestFlask.Models.Utils
{

    public static class HashUtil
    {
        public static string Crc32Hash(string input)
        {
            StringBuilder hash = new StringBuilder();

            Crc32 crc32 = new Utils.Crc32();
            var bytes = crc32.ComputeHash(new UTF8Encoding().GetBytes(input));
            
            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
}

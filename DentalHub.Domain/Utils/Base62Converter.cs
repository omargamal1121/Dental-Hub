using System;
using System.Text;
using System.Numerics;

namespace DentalHub.Domain.Utils
{
    public static class Base62Converter
    {
        private const string Charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static string Encode(Guid guid)
        {
            byte[] bytes = guid.ToByteArray();
    
            BigInteger number = new BigInteger(bytes, isUnsigned: true, isBigEndian: false);
            return Encode(number);
        }

        public static string Encode(BigInteger number)
        {
            if (number == 0) return Charset[0].ToString();

            var result = new StringBuilder();
            while (number > 0)
            {
                result.Insert(0, Charset[(int)(number % 62)]);
                number /= 62;
            }
            return result.ToString();
        }

        public static Guid DecodeToGuid(string base62)
        {
            BigInteger number = Decode(base62);
            byte[] bytes = number.ToByteArray(isUnsigned: true, isBigEndian: false);
            
            if (bytes.Length > 16)
            {
                throw new ArgumentException("Base62 string represents a value larger than a Guid.");
            }

            if (bytes.Length < 16)
            {
                byte[] paddedBytes = new byte[16];
                Array.Copy(bytes, paddedBytes, bytes.Length);
                return new Guid(paddedBytes);
            }

            return new Guid(bytes);
        }

        public static BigInteger Decode(string base62)
        {
            BigInteger result = 0;
            foreach (char c in base62)
            {
                int value = Charset.IndexOf(c);
                if (value == -1) throw new ArgumentException($"Invalid character in base62 string: {c}");
                result = result * 62 + value;
            }
            return result;
        }
    }
}

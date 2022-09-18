using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Com.RandomDudes.CryptoGraphy
{
    public static class AES
    {
        #region Cipher

        public static string Cipher(string textToCipher, string key)
        {
            return Cipher(ObjectToBytes(textToCipher), key);
        }

        public static string Cipher(byte[] bytesToCipher, string key)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);

            RijndaelManaged AES = new RijndaelManaged
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform transform = AES.CreateEncryptor();
            byte[] resultArray = transform.TransformFinalBlock(bytesToCipher, 0, bytesToCipher.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        #endregion

        #region Decipher

        public static T DecipherToObject<T>(string textToDecipher, string key)
        {
            return (T)ByteArrayToObject(Decipher(textToDecipher, key));
        }

        public static byte[] Decipher(string textToDecipher, string key)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] bytesToDecipher = Convert.FromBase64String(textToDecipher);

            RijndaelManaged AES = new RijndaelManaged
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform transform = AES.CreateDecryptor();

            return transform.TransformFinalBlock(bytesToDecipher, 0, bytesToDecipher.Length);
        }

        #endregion

        #region Object to Bytes conversions

        public static byte[] ObjectToBytes(object input)
        {
            if (input == null)
                return null;

            BinaryFormatter binFormatter = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();

            binFormatter.Serialize(memStream, input);

            return memStream.ToArray();
        }

        public static object ByteArrayToObject(byte[] bytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binFormatter = new BinaryFormatter();

            memStream.Write(bytes, 0, bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);

            return binFormatter.Deserialize(memStream);
        }

        #endregion
    }
}
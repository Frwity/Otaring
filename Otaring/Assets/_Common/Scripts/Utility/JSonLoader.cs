using Com.RandomDudes.CryptoGraphy;
using UnityEngine;

namespace Com.RandomDudes.Utility
{
    public static class JSonLoader
    {
        public readonly static string PATH_TO_JSONS = "JSons/";
        public readonly static string PATH_TO_CIPHERED_JSONS = "EncryptedJsons/";

        public readonly static string AES_KEY = "Ze+H8bqe7apcTYz4RGvsKA==";
        private const bool ARE_JSON_CIPHERED = true;

        public static string GetJsonTextByPath(string pPath)
        {
#pragma warning disable CS0162 // Code inaccessible détecté

            if (ARE_JSON_CIPHERED)
            {
                TextAsset lJson = (TextAsset)Resources.Load(PATH_TO_CIPHERED_JSONS + pPath);
                return AES.DecipherToObject<string>(lJson.ToString(), AES_KEY);
            }
            else
            {
                TextAsset lJson = (TextAsset)Resources.Load(PATH_TO_JSONS + pPath);
                return lJson.ToString();
            }
#pragma warning restore CS0162 // Code inaccessible détecté
        }
    }
}